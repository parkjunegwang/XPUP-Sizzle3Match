using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager I;
    private PlayerCloudService _service;
    public PlayerCloudData prevData { get; private set; }

    private void Awake() 
    {
        I = this;
        DontDestroyOnLoad(this);
    }

    private async void Start()
    {
        // Firebase 준비 대기
        if (FirebaseBootstrap.Instance == null)
        {
            Debug.LogError("FirebaseBootstrap is missing in scene.");
            return;
        }

        if (!FirebaseBootstrap.Instance.IsReady)
            await FirebaseBootstrap.Instance.InitAsync();


        _service = new PlayerCloudService(FirebaseBootstrap.Instance.Auth);

        prevData = await _service.GetOrCreateAsync();
        Debug.Log($"Loaded: currency={prevData.currency}, lastMissionAtUtc={prevData.lastMissionAtUtc.ToDateTime():u} , count= {prevData.count}");

    }

    public async void OnClickClaimDaily()
    {
        if (_service == null) return;

        int reward = 1;

        var result = await _service.TryClaimDailyKstAsync(reward);

        if (result.ok)
        {
            Debug.Log($"✅ Daily claimed! currencyAfter={result.currencyAfter}");

            prevData = await _service.GetOrCreateAsync();
        }
        else
        {
            Debug.Log($"⏳ Cooldown. remain={FormatRemain(result.remain)}");
        }
    }
    public async Task ClearStage(int ClearStage)
    {

        await _service.TryClearStageKstAsync(ClearStage);

        prevData = await _service.GetOrCreateAsync();

    }


    private string FormatRemain(TimeSpan t)
    {
        if (t < TimeSpan.Zero) t = TimeSpan.Zero;
        return $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}";
    }

    public int GetMaxStage()
    {
        var data = Resources.LoadAll<SaveData>("StageData");

        return data.Length;
    }
}
