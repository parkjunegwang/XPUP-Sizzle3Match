using Assets.Scripts.FrameWork.Job;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PopupSetting : Popup
{
    Button Dimed;
    Button Button_Close;

     private Slider bgmSlider;
     private Slider sfxSlider;
    void Start()
    {
        Dimed = transform.Find("Dimed").GetComponent<Button>();

        Button_Close = transform.Find("Button_Close").GetComponent<Button>();

        bgmSlider = transform.Find("Popup08_Topbar_Divided/Middle/Group_List/Music/Slider_Handle_Pink").GetComponent<Slider>();

        sfxSlider = transform.Find("Popup08_Topbar_Divided/Middle/Group_List/SFX/Slider_Handle_Pink").GetComponent<Slider>();
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
       // base.Open(args);
    }

}
