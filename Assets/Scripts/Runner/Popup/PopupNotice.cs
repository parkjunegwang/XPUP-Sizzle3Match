using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupNotice : MonoBehaviour
{
    Button m_btnDimm; //클릭시 꺼져버리자
    TextMeshProUGUI m_Desc;

    Button m_OK;
    TextMeshProUGUI m_textOK;

    public void Awake()
    {
        m_btnDimm = transform.Find("Dimmed").GetComponent<Button>();
        m_Desc = transform.Find("Desc").GetComponent<TextMeshProUGUI>();

        m_OK = transform.Find("Button_Ok").GetComponent<Button>();
        m_textOK = transform.Find("OKText").GetComponent<TextMeshProUGUI>();

        m_btnDimm.onClick.AddListener(() => Destroy(gameObject));

      
    }
    public void OnDestroy()
    {
        m_OK.onClick.RemoveAllListeners();
        m_btnDimm.onClick.RemoveAllListeners();
    }
    public void SetDesc(string desc , string textOk, Action act)
    {
        m_Desc.text = desc;
        m_textOK.text = textOk;

        m_OK.onClick.AddListener(() => {
            if (act != null)
                act();
            else
                Destroy(gameObject);
        });
    }

}
