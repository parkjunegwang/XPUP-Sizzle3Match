using System;
using Firebase.Firestore;

[Serializable]
[FirestoreData]
public class PlayerCloudData
{
    [FirestoreProperty] public Timestamp lastMissionAtUtc { get; set; } // 서버시간 저장
    [FirestoreProperty] public int currency { get; set; }              // 재화
    [FirestoreProperty] public int version { get; set; } = 1;          // 확장용

    [FirestoreProperty] public int count { get; set; } = 0;          // 카운트에따라 보상지급 7초과시 0으로다시돌아가는
}
