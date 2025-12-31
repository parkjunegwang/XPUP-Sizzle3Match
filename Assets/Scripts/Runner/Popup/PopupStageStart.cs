using Assets.Scripts.FrameWork.Job;
using System;
using TreeEditor;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;

public class PopupStageStart : Popup
{
    Button Dimed;
    Button Button_Close;


    Button Button_Play;
    private void Initialize()
    {
        Dimed = transform.Find("Dimed").GetComponent<Button>();

        Button_Close = transform.Find("Popup/Button_Close").GetComponent<Button>();

        Button_Play = transform.Find("Popup/Button_Play").GetComponent<Button>();
        Button_Play.onClick.AddListener(OnStagePlay);
        Dimed.onClick.AddListener(OnClose);

        Button_Close.onClick.AddListener(OnClose);

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

    void OnStagePlay()
    {
        JobMaker.TriggerGlobalEvent(EventDefine.SHOW_SCENE_LOADING);

        Close();
    }
}
