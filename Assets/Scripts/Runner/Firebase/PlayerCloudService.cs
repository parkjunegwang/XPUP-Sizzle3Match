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
            version = 1
        };

        await UserDoc.SetAsync(init, SetOptions.MergeAll);
        return init;
    }

    /// <summary>
    /// 데일리 보상 수령 (서버시간 기준 24시간 쿨타임)
    /// rewardAmount 만큼 currency 증가
    /// </summary>
    public async Task<ClaimDailyResult> TryClaimDailyAsync(int rewardAmount, TimeSpan cooldown)
    {
        // 트랜잭션으로 원자 처리(해킹/중복 클릭/경합 방지)
        return await _db.RunTransactionAsync(async transaction =>
        {
            var snap = await transaction.GetSnapshotAsync(UserDoc);

            PlayerCloudData data;
            if (!snap.Exists)
            {
                data = new PlayerCloudData
                {
                    lastMissionAtUtc = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)),
                    currency = 0,
                    version = 1
                };
                transaction.Set(UserDoc, data, SetOptions.MergeAll);
                // 아래 로직 계속 진행
            }
            else
            {
                data = snap.ConvertTo<PlayerCloudData>();
            }

            // 서버가 찍어줄 timestamp (commit 시점에 결정됨)
            var serverNow = FieldValue.ServerTimestamp;

            // 비교용: 현재는 snap에 있는 lastMissionAtUtc만 믿고 쿨타임 계산.
            // "정확한 서버 현재시간"을 읽어 비교하려면 별도 serverTime 문서 패턴이 필요하지만,
            // 이 정도면 데일리 안정적으로 굴러감(쿨타임 기준은 lastMissionAt 기준).
            var last = data.lastMissionAtUtc.ToDateTime(); // UTC DateTime
            if (last.Kind != DateTimeKind.Utc) last = DateTime.SpecifyKind(last, DateTimeKind.Utc);

            var nowClientUtc = DateTime.UtcNow; // 비교용 힌트(최종 시간은 서버가 찍음)
            var elapsed = nowClientUtc - last;

            if (elapsed < cooldown)
            {
                var remain = cooldown - elapsed;
                return ClaimDailyResult.FailCooldown(remain);
            }

            var newCurrency = data.currency + rewardAmount;

            // 업데이트: lastMissionAtUtc는 서버가 확정
            transaction.Update(UserDoc, new System.Collections.Generic.Dictionary<string, object>
            {
                { "currency", newCurrency },
                { "lastMissionAtUtc", serverNow },
            });

            return ClaimDailyResult.Success(newCurrency);
        });
    }
}

public struct ClaimDailyResult
{
    public bool ok;
    public int currencyAfter;
    public TimeSpan remain;

    public static ClaimDailyResult Success(int currencyAfter) =>
        new ClaimDailyResult { ok = true, currencyAfter = currencyAfter, remain = TimeSpan.Zero };

    public static ClaimDailyResult FailCooldown(TimeSpan remain) =>
        new ClaimDailyResult { ok = false, currencyAfter = -1, remain = remain };
}
