using Assets.Scripts.FrameWork.Job;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneHandler : MonoBehaviour
{
    public static LobbySceneHandler I; //빠른개발과 편의를 위해 인스턴스하자 시간이없다 크크


    private JMFSM m_fsmLobby;

    private Image m_fadeCover;

    private GameObject Popup_DailyBonus;

    private GameObject Popup_Mission;

    private GameObject Popup_Setting;

    private GameObject Popup_Shop;

    private GameObject m_Player;
    void Awake()
    {
        I = this;

        m_fsmLobby = new LobbyFSM(new());

       
        // m_Player = GameObject.Find("Player");
    }


    private void Start()
    {
        m_fsmLobby.StartFSM();

        Popup_DailyBonus = transform.Find("Middle/Daily_Bonus").gameObject;

        Popup_Mission = transform.Find("Middle/Mission_List").gameObject;

        Popup_Setting = transform.Find("Middle/Settings").gameObject;

        Popup_Shop = transform.Find("Middle/Shop").gameObject;

    }
    private void OnDestroy()
    {
        m_fsmLobby?.DestroyFSM();
        m_fsmLobby = null;
    }

    public async void Show_Daily_Bonus_30Day()
    {
       await PopupManager.Instance.OpenAsync<PopupDailyBonus>("Popup/DailyBonus/Daily_Bonus");
    }
    public void Show_Popup_Mssion()
    {
        Popup_Mission.gameObject.SetActive(true);
    }

    public void Show_Popup_Setting()
    {
        Popup_Setting.gameObject.SetActive(true);
    }

    public void Show_Popup_Shop()
    {
        Popup_Shop.gameObject.SetActive(true);
    }
    public void FadeInLobbyScene(Action<string> callback)
    {
        DOTween.Sequence().AppendInterval(1)
                          .Append(m_fadeCover.DOFade(1,1f))
                          .OnComplete(() => callback?.Invoke(SceneDefine.GAME_SCENE_NAME));
    }

}
