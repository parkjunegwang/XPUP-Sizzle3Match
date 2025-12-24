using Assets.Scripts.FrameWork.Job;
using UnityEngine;
using UnityEngine.UI;

public class PopupGameOver : MonoBehaviour
{
    private Button m_Button;

    private void Start()
    {
        m_Button = GetComponent<Button>();

        m_Button.onClick.AddListener(OnClickDimm);
    }

    void OnClickDimm()
    {
        JobMaker.TriggerGlobalEvent(EventDefine.SHOW_SCENE_LOADING);
    }
}
