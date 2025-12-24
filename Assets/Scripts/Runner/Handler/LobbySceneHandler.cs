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

    private GameObject m_Stages;

    private GameObject m_Player;
    void Awake()
    {
        I = this;

        m_fsmLobby = new LobbyFSM(new());

        m_Stages = GameObject.Find("Stages");

        m_Player = GameObject.Find("Player");
    }

    private void Start()
    {
        m_fsmLobby.StartFSM();

        //저장데이터는 그냥.. PlayerPrefs쓰자구
        int isClear = PlayerPrefs.GetInt("Clear", 0);
        var stages = m_Stages.transform; 

        if (isClear == 1)
        {
            int ClearStage = PlayerPrefs.GetInt("ClearStage", 0);
  
            var stage = stages.GetChild(ClearStage).GetComponent<LobbyStageContainer>();

            stage.ShowClearStage();

        }
        var MaxClear = PlayerPrefs.GetInt("MaxClearStage", -1);

        if (MaxClear == 8) MaxClear = 7;

        m_Player.transform.localPosition = stages.GetChild(MaxClear + 1).localPosition + new Vector3(200f, 0f, 0f);

        //초기화
        PlayerPrefs.SetInt("Clear", 0);
        PlayerPrefs.SetInt("ClearStage", 0);

        //시간이없어서 로비는 하드코디이잉! End
    }
    private void OnDestroy()
    {
        m_fsmLobby?.DestroyFSM();
        m_fsmLobby = null;
    }

    public void FadeInLobbyScene(Action<string> callback)
    {
        DOTween.Sequence().AppendInterval(1)
                          .Append(m_fadeCover.DOFade(1,1f))
                          .OnComplete(() => callback?.Invoke(SceneDefine.GAME_SCENE_NAME));
    }

}
