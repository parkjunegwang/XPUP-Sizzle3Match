using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupDailyBonus : Popup
{
    Button Dimed;
    Button Button_Close;

    Button GetItem;

    bool isGetItem = false;


    private DailyBonusItem[] dailyBonusItems;



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
        var prevData = DataManager.I.prevData;

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
    protected override void OnOpen(object args)
    {
        gameObject.SetActive(false);
      

        Initialize();

    }
    protected override void OnClose()
    {
        PopupManager.Instance.Close(this);
        //gameObject.SetActive(false);
    }
    public void OnClickClaimDaily()
    {
        DataManager.I.OnClickClaimDaily();
    }

    public void OnClickGetItem()
    {
        if (isGetItem) return;
        isGetItem = true;

        GetItem.gameObject.SetActive(false);
        todayItem.GetItemAni();
    }

 
  
}
