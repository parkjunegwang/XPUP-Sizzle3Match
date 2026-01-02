using Assets.Scripts.FrameWork.Job;
using TMPro;
using UnityEngine.UI;

public class PopupStageStart : Popup
{
    Button Dimed;
    Button Button_Close;


    Button Button_Play;

    TextMeshProUGUI Text_Title;
    private void Initialize()
    {
        Dimed = transform.Find("Dimed").GetComponent<Button>();

        Button_Close = transform.Find("Popup/Button_Close").GetComponent<Button>();

        Button_Play = transform.Find("Popup/Button_Play").GetComponent<Button>();
        Button_Play.onClick.AddListener(OnStagePlay);

        Text_Title = transform.Find("Popup/Top/Text_Title").GetComponent<TextMeshProUGUI>();
        Dimed.onClick.AddListener(OnClose);

        Button_Close.onClick.AddListener(OnClose);

        gameObject.SetActive(true);

        Text_Title.text = "Level_" +( DataManager.I.prevData.CurrentStage+1);
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

    void OnStagePlay()
    {
        BlindsTransition.Instance.ChangeScene(()=> { JobMaker.TriggerGlobalEvent(EventDefine.SHOW_SCENE_LOADING); });
                

        Close();
    }
}
