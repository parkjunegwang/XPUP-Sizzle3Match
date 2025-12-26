using JetBrains.Annotations;
using System;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class PopupDailyBonus : Popup
{
    Button Dimed;
    Button Button_Close;

    Button GetItem;

    bool isGetItem = false;

    private PlayerCloudService _service;

    private DailyBonusItem[] dailyBonusItems;

    private PlayerCloudData prevData;

    private DailyBonusItem todayItem;
    private void Initialize()
    {
        Dimed = transform.Find("Dimed").GetComponent<Button>();

        Button_Close = transform.Find("Button_Close02").GetComponent<Button>();

        GetItem = transform.Find("GetItem").GetComponent<Button>();

        Dimed.onClick.AddListener(OnClose);

        Button_Close.onClick.AddListener(OnClose);

        GetItem.onClick.AddListener(OnClickGetItem);

        var ItemParent = transform.Find("Group_DailyFrame");

        dailyBonusItems = new DailyBonusItem[ItemParent.transform.childCount];

        for (int i = 0; i < ItemParent.transform.childCount; ++i)
        {
            dailyBonusItems[i] = ItemParent.transform.GetChild(i).GetComponent<DailyBonusItem>();
            dailyBonusItems[i].Initailize();
        }
        if (prevData.lastMissionAtUtc.ToDateTime().Day == DateTime.Now.Day)
        {
            prevData.count--;
        }

        for (int i = 0; i < dailyBonusItems.Length; ++i)
        {
            if (i < prevData.count)
            {
                dailyBonusItems[i].SetGet();
            }
            else if (i == prevData.count)
            {
                todayItem = dailyBonusItems[i];

                if (prevData.lastMissionAtUtc.ToDateTime().Day != DateTime.Now.Day)
                {
                    dailyBonusItems[i].SetToday();
                    GetItem.gameObject.SetActive(true);
                }
                else
                {
                    isGetItem = true;
                    GetItem.gameObject.SetActive(false);
                    todayItem.SetGet();
                }
                break;   
            }
            else
            {
              //  dailyBonusItems[i].SetNext();
            }
            // ;
        }
        OnClickClaimDaily();

        gameObject.SetActive(true);
    }
    protected override async void OnOpen(object args)
    {
        gameObject.SetActive(false);
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

        

        Initialize();

    }
    protected override void OnClose()
    {
        PopupManager.Instance.Close(this);
        //gameObject.SetActive(false);
    }
    public async void OnClickClaimDaily()
    {
        if (_service == null) return;

        int reward = 1;

        var result = await _service.TryClaimDailyKstAsync(reward);

        if (result.ok)
        {
            Debug.Log($"✅ Daily claimed! currencyAfter={result.currencyAfter}");


        }
        else
        {
            Debug.Log($"⏳ Cooldown. remain={FormatRemain(result.remain)}");
        }


    }

    public void OnClickGetItem()
    {
        if (isGetItem) return;
        isGetItem = true;

        GetItem.gameObject.SetActive(false);
        todayItem.GetItemAni();
    }

    private string FormatRemain(TimeSpan t)
    {
        if (t < TimeSpan.Zero) t = TimeSpan.Zero;
        return $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}";
    }
  
}
