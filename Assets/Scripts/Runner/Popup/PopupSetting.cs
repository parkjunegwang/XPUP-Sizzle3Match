using Assets.Scripts.FrameWork.Job;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupSetting : Popup
{
    Button Dimed;
    Button Button_Close;

     private Slider bgmSlider;
     private Slider sfxSlider;

    private GameObject obj_Lobby;
    private GameObject obj_Game;

    private Button Button_Lobby;
    private Button Button_Game;
    void Initailize()
    {
        Dimed = transform.Find("Dimed").GetComponent<Button>();

        Button_Close = transform.Find("Button_Close").GetComponent<Button>();

        bgmSlider = transform.Find("Popup08_Topbar_Divided/Middle/Group_List/Music/Slider_Handle_Pink").GetComponent<Slider>();

        sfxSlider = transform.Find("Popup08_Topbar_Divided/Middle/Group_List/SFX/Slider_Handle_Pink").GetComponent<Slider>();

        obj_Lobby = transform.Find("Popup08_Topbar_Divided/Btn_Lobby").gameObject;
        obj_Game = transform.Find("Popup08_Topbar_Divided/Btn_Game").gameObject;

        Button_Lobby = transform.Find("Popup08_Topbar_Divided/Btn_Game/Button_Lobby").GetComponent<Button>();
        Button_Game = transform.Find("Popup08_Topbar_Divided/Btn_Game/Button_Return").GetComponent<Button>();

        Button_Lobby.onClick.AddListener(OnClickReturntoLobby);
        Button_Game.onClick.AddListener(OnClickReturntoGame);

        var d = JobMaker.GlobalDataBox.GetData<GameData>();
        bgmSlider.value = d.BGM;
        sfxSlider.value = d.SFX;

        bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxSlider.onValueChanged.AddListener(SetSfxVolume);

        Dimed.onClick.AddListener(Close);

        Button_Close.onClick.AddListener(Close);
    }
    public void SetBgmVolume(float value)
    {
        var d = JobMaker.GlobalDataBox.GetData<GameData>();
        {
            d.BGM = value;
        }

        PlayerPrefs.SetFloat("BGM", value);
    }
    public void OnClickReturntoGame()
    {
        InGameUIHandler.I.OnStart();
        Close();
    }
    public void OnClickReturntoLobby()
    {
        BlindsTransition.Instance.ChangeScene(() => { JobMaker.TriggerGlobalEvent(EventDefine.SHOW_SCENE_LOADING); });
        Close();
    }
    public void SetSfxVolume(float value)
    {
        var d = JobMaker.GlobalDataBox.GetData<GameData>();
        {
            d.SFX = value;
        }
        PlayerPrefs.SetFloat("SFX", value);
    }

    protected override void OnClose()
    {
        //base.Close();
    }
    protected override void OnOpen(object args = null)
    {
        Initailize();
        obj_Lobby.gameObject.SetActive(SceneManager.GetActiveScene().name == "LobbyScene");
        obj_Game.gameObject.SetActive(SceneManager.GetActiveScene().name != "LobbyScene");

        if(SceneManager.GetActiveScene().name != "LobbyScene")
        {
            InGameUIHandler.I.OnPause();
        }
        // base.Open(args);
    }

}
