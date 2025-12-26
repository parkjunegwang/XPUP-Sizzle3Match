using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerCloudService
{
    private readonly FirebaseAuth _auth;
    private readonly FirebaseFirestore _db;

    public PlayerCloudService(FirebaseAuth auth)
    {
        _auth = auth;
        _db = FirebaseFirestore.DefaultInstance;
    }

    private string Uid
    {
        get
        {
            if (_auth.CurrentUser == null) throw new Exception("Not signed in.");
            return _auth.CurrentUser.UserId;
        }
    }

    private DocumentReference UserDoc => _db.Collection("users").Document(Uid);

    /// <summary>유저 문서가 없으면 생성하고, 있으면 읽어서 반환</summary>
    public async Task<PlayerCloudData> GetOrCreateAsync()
    {
      

        try
        {
            var snap = await UserDoc.GetSnapshotAsync();
            //  dictionary = snapshot.ToDictionary();

            if (snap.Exists)
            {
                return snap.ConvertTo<PlayerCloudData>();
            }
        }
        catch (FirestoreException)
        {

        }

        // 최초 생성: lastMissionAtUtc는 아주 과거(또는 null)로 넣어서 첫 데일리 바로 가능하게 처리
        var init = new PlayerCloudData
        {
            lastMissionAtUtc = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)),
            currency = 0,
            version = 1,
            count = 0,
        };

        await UserDoc.SetAsync(init, SetOptions.MergeAll);
        return init;
    }

    /// <summary>
    /// 데일리 보상 수령 (서버시간 기준 24시간 쿨타임)
    /// rewardAmount 만큼 currency 증가
    /// </summary>
    public async Task<ClaimDailyResult> TryClaimDailyKstAsync(int reward)
    {
        return await _db.RunTransactionAsync(async tx =>
        {
            var snap = await tx.GetSnapshotAsync(UserDoc);
            PlayerCloudData data;

            if (!snap.Exists)
            {
                data = new PlayerCloudData
                {
                    lastMissionAtUtc = Timestamp.FromDateTime(
                        DateTime.SpecifyKind(new DateTime(2000, 1, 1), DateTimeKind.Utc)),
                    currency = 0
                };
                tx.Set(UserDoc, data);
            }
            else
            {
                data = snap.ConvertTo<PlayerCloudData>();
            }

            DateTime lastClaimUtc = data.lastMissionAtUtc.ToDateTime();
            DateTime todayKstMidnightUtc = GetTodayKstMidnightUtc();

            if (lastClaimUtc >= todayKstMidnightUtc)
            {
                return ClaimDailyResult.FailAlreadyClaimed(GetNextKstMidnightUtc().TimeOfDay);
            }

            int newCurrency = data.currency + reward;
            int newCount = data.count + 1;
            tx.Update(UserDoc, new Dictionary<string, object>
        {
            { "currency", newCurrency },
            { "lastMissionAtUtc", FieldValue.ServerTimestamp },
            { "count", newCount}
        });

            return ClaimDailyResult.Success(newCurrency);
        });
    }

    public DateTime GetNextKstMidnightUtc()
    {
        DateTime utcNow = DateTime.UtcNow;
        DateTime kstNow = utcNow.AddHours(9);

        DateTime nextKstMidnight = new DateTime(
            kstNow.Year,
            kstNow.Month,
            kstNow.Day,
            0, 0, 0
        ).AddDays(1);

        return DateTime.SpecifyKind(nextKstMidnight.AddHours(-9), DateTimeKind.Utc);
    }
    public static DateTime GetTodayKstMidnightUtc()
    {
        // KST = UTC + 9
        DateTime utcNow = DateTime.UtcNow;

        // 현재 시간을 KST로 변환
        DateTime kstNow = utcNow.AddHours(9);

        // KST 기준 오늘 00:00
        DateTime kstMidnight = new DateTime(
            kstNow.Year,
            kstNow.Month,
            kstNow.Day,
            0, 0, 0,
            DateTimeKind.Unspecified
        );

        // 다시 UTC로 변환
        DateTime utcMidnight = kstMidnight.AddHours(-9);

        return DateTime.SpecifyKind(utcMidnight, DateTimeKind.Utc);
    }
}

public struct ClaimDailyResult
{
    public bool ok;
    public int currencyAfter;
    public TimeSpan remain;
    public FailReason reason;

    public enum FailReason
    {
        None,
        AlreadyClaimed
    }

    public static ClaimDailyResult Success(int currencyAfter)
    {
        return new ClaimDailyResult
        {
            ok = true,
            currencyAfter = currencyAfter,
            remain = TimeSpan.Zero,
            reason = FailReason.None
        };
    }

    public static ClaimDailyResult FailAlreadyClaimed(TimeSpan remainUntilNext)
    {
        return new ClaimDailyResult
        {
            ok = false,
            currencyAfter = -1,
            remain = remainUntilNext,
            reason = FailReason.AlreadyClaimed
        };
    }
}

