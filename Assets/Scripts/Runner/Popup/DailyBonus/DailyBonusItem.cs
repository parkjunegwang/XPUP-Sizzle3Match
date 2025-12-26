using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusItem : MonoBehaviour
{
    //¿À´Ã ¹ÞÀ»³ðµé
    GameObject Foucus;
    Image FoucusTop;

    //ÀÌ¹Ì ¹ÞÀº³ðµé
    GameObject DividerLine;
    GameObject Check;

    //ÇÊ¼ö 
    Image image_Icon;
    Image image_BG;
    TextMeshProUGUI text_Count;
    TextMeshProUGUI text_Day;

    public void Initailize()
    {
        Foucus = transform.Find("Focus").gameObject;
        FoucusTop = transform.Find("Bg/FocusTop").GetComponent<Image>();

        DividerLine = transform.Find("Bg/DividerLine").gameObject;
        Check = transform.Find("Icon_Check").gameObject;

        image_BG = transform.Find("Bg").GetComponent<Image>();

        image_Icon = transform.Find("ItemIcon").GetComponent<Image>();
        text_Count = transform.Find("Text_Num").GetComponent<TextMeshProUGUI>();
        text_Day = transform.Find("Text_Day").GetComponent<TextMeshProUGUI>();
    }

    public void SetToday()
    {
        text_Day.color = Color.green;
        image_BG.color = Color.green;
        FoucusTop.color = Color.white;
        text_Day.text = "TODAY";
        Foucus.SetActive(true);
        FoucusTop.gameObject.SetActive(true);

        DividerLine.SetActive(false);
        Check.SetActive(false);
    }

    public void SetGet()
    {
        text_Day.color = Color.gray;
        Foucus.SetActive(false);
        FoucusTop.gameObject.SetActive(false);

        image_BG.color = GetHexColor("0787FF");
        FoucusTop.color = GetHexColor("0787FF");

        DividerLine.SetActive(true);
        Check.SetActive(true);
    }
    public void SetNext()
    {
        text_Day.color = Color.black;
        Foucus.SetActive(false);
        FoucusTop.gameObject.SetActive(false);

        DividerLine.SetActive(true);
        Check.SetActive(false);
    }

    public UnityEngine.Color GetHexColor(string hex)
    {
        UnityEngine.Color color;
        ColorUtility.TryParseHtmlString(hex, out color);

        return color;
    }

    public void GetItemAni()
    {
        Check.SetActive(true);
        DOTween.Sequence().Append(Check.transform.DOScale(3, 0f))
                         .Append(Check.transform.DOScale(1, 0.5f))
                        .OnComplete(() => SetGet());
    }
}   
