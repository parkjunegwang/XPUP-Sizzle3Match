using Assets.Scripts.FrameWork.Job;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIHandler : MonoBehaviour
{
    public static InGameUIHandler I;

    Slider slider_exp;

    TextMeshProUGUI text_Level;
    TextMeshProUGUI text_EXP;
    TextMeshProUGUI text_Timer;

    int NowEXP = 0;
    int NowTime = 0;

    Button btn_Pause;
    void Awake()
    {
        I = this;
    }

    public void AddExp(int a = 1)
    {
        var data = JobMaker.GlobalDataBox.GetData<StageData>();

        NowEXP += a;

        text_EXP.text = NowEXP + " / "+ data.SaveData.StageEXP;

        slider_exp.value = (float)NowEXP / (float)data.SaveData.StageEXP;
    }

    public void Initialize()
    {
        slider_exp = transform.Find("Top/Text_Level/Slider").GetComponent<Slider>();

        text_Level = transform.Find("Top/Text_Level").GetComponent<TextMeshProUGUI>();
        text_EXP = transform.Find("Top/Text_Level/Text_EXP").GetComponent<TextMeshProUGUI>();
        text_Timer = transform.Find("Top/Text_Timer").GetComponent<TextMeshProUGUI>();

        btn_Pause = transform.Find("Top/Button_Pause").GetComponent<Button>();

        btn_Pause.onClick.AddListener(Show_Popup_Setting);

        var data = JobMaker.GlobalDataBox.GetData<StageData>();

        NowTime = data.SaveData.StageTime;

        NowEXP = 0;
        slider_exp.value = 0;
        text_EXP.text = "0 / " + data.SaveData.StageEXP;

        text_Level.text = "Level " + data.SaveData.StageLevel.ToString();

        text_Timer.text = data.GetTimer();

        isPause = false;
    }

    public async void Show_Popup_Setting()
    {
        await PopupManager.Instance.OpenAsync<PopupSetting>("Popup/Setting/Settings");
    }

    private float _pointTime = 1.0f; //1초마다 실행
    private float _nextTime = 0.0f; //다음번 실행할 시간

    private bool isPause = false;
    void FixedUpdate()
    {
        if (isPause) return;

        if (Time.time > _nextTime)
        {
            _nextTime = Time.time + _pointTime; //다음번 실행할 시간

            NowTime -= 1;

            TimeSpan t = TimeSpan.FromSeconds(NowTime);

            text_Timer.text = t.ToString(@"mm\:ss");

            if (NowTime <= 0)
            {
                isPause = true;
                OpenClearPopup();
            }
        }

    }

    private async void OpenClearPopup()
    { 
        await PopupManager.Instance.OpenAsync<PopupStageClear>("Popup/Stage/Popup_StageClear"); 
    }
    
}
