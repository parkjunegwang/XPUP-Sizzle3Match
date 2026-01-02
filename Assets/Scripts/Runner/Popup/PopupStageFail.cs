using Assets.Scripts.FrameWork.Job;
using UnityEngine;
using UnityEngine.UI;

public class PopupStageFail : Popup
{
    Button Button_Ok;
    Button Button_Ads_Ok;
    private void Initialize()
    {

        Button_Ok = transform.Find("Button01_Claim").GetComponent<Button>();
        Button_Ads_Ok = transform.Find("Button01_AdClaim").GetComponent<Button>();

        Button_Ok.onClick.AddListener(OnGotoLobby);
        Button_Ads_Ok.onClick.AddListener(OnAdsPlay);

        // Dimed.onClick.AddListener(OnClose);

        gameObject.SetActive(true);
    }

    protected override void OnOpen(object args)
    {
        gameObject.SetActive(false);


        Initialize();

    }

    protected override void OnClose()
    {
        //PopupManager.Instance.Close(this);
        //gameObject.SetActive(false);
    }

    void OnGotoLobby()
    {

        BlindsTransition.Instance.ChangeScene(() => { JobMaker.TriggerGlobalEvent(EventDefine.SHOW_SCENE_LOADING); });

        Close();
    }
    void OnAdsPlay()
    {

    }
}
