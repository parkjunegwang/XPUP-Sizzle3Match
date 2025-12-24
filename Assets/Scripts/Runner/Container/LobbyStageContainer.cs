using Assets.Scripts.FrameWork.Job;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyStageContainer : MonoBehaviour
{
    private bool isBoss = false; //데이터 처리 할수있음 하자.. 시간이없을지도?  .to 준광 검색용
    private TextMeshPro m_StageName;
    private Image[] m_StageSexual;

    private Button m_btnStageEnter;

    private Sprite m_spriteStar; //스테이지 성적
    private Sprite m_spriteBoss; //스테이지 보스 일경우 아이콘 교체용

    public int m_StageLevel = 0;
    void Awake()
    {
        m_StageName = transform.Find("Name").GetComponent<TextMeshPro>();

        var StageSexual = transform.Find("StageSexual");

        m_StageSexual = new Image[StageSexual.transform.childCount];

        for (int i = 0; i < m_StageSexual.Length; ++i)
        {
            m_StageSexual[i] = StageSexual.transform.GetChild(i).GetComponent<Image>();
        }

        m_btnStageEnter = GetComponent<Button>();

        m_btnStageEnter.onClick.AddListener(OnClickStageEnter);

        OnLaodResourcesSprite();

        if (m_StageLevel <= PlayerPrefs.GetInt("MaxClearStage", -1))
        {
            for (int i = 0; i < m_StageSexual.Length; ++i)
            {
                m_StageSexual[i].sprite = m_spriteStar;
            }
        }
    }

    private void OnDestroy()
    {
        m_btnStageEnter.onClick.RemoveAllListeners();
    }

    void OnLaodResourcesSprite()
    {
        m_spriteStar = Resources.Load<Sprite>("Image/ItemIcon_Star");
        m_spriteBoss = Resources.Load<Sprite>("Image/ItemIcon_Skull_Boss");

    }

    public void ShowClearStage()
    {
        for (int i = 0; i < m_StageSexual.Length; ++i)
        {
            m_StageSexual[i].sprite = m_spriteStar;

            m_StageSexual[i].transform.localScale = new Vector3(2f, 2f, 2f);
        }

        DG.Tweening.Sequence seq = DOTween.Sequence();

        

        seq.Append(m_StageSexual[0].transform.DOScale(new Vector3(1f,1f,1f), 0.5f).SetEase(Ease.OutQuad))
           .Append(m_StageSexual[1].transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.OutQuad))
           .Append(m_StageSexual[2].transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.OutQuad))
            ;
        //클리어하고 나옴 연출 별 팡팡팡! .to 준광 검색용
    }

    public void OnClickStageEnter()
    {
        //팝업 켜서 스타트 만들자   .to 준광 검색용
        PlayerPrefs.SetInt("CurrentStage",m_StageLevel);
        var MaxClear = PlayerPrefs.GetInt("MaxClearStage", -1);
        var obj = Resources.Load("Popup/Popup_Notice") as GameObject;

        var popup = Instantiate(obj, GameObject.Find("UICanvas").transform);

        if (MaxClear + 1 < m_StageLevel)
        {
            popup.GetComponent<PopupNotice>().SetDesc("Clear the previous stage and come!!","O K",null );
        }
        else
        {
            popup.GetComponent<PopupNotice>().SetDesc("Here We Go!!", "G O", () => JobMaker.TriggerGlobalEvent(EventDefine.SHOW_SCENE_LOADING));
        }
     
        //스타트후 씬전환  .to 준광 검색용
    }


}
