using Assets.Scripts.FrameWork.Job;

using UnityEngine.UI;
public class PopupStageClear : Popup
{

    Button Dimed;
    
    Button Button_Ok;
    Button Button_Ads_Ok;
    private void Initialize()
    {
        Dimed = transform.Find("Dimed").GetComponent<Button>();


        Button_Ok = transform.Find("Button01_Claim").GetComponent<Button>();
        Button_Ads_Ok = transform.Find("Button01_AdClaim").GetComponent<Button>();

        Button_Ok.onClick.AddListener(OnStageNextStage);
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
        PopupManager.Instance.Close(this);
        //gameObject.SetActive(false);
    }

    void OnStageNextStage()
    {
        InGameHandler.I.NextStage();

        Close();
    }
    void OnAdsPlay()
    { 
    
    }
}
