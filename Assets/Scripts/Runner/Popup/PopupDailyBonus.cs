using UnityEngine;
using UnityEngine.UI;

public class PopupDailyBonus : MonoBehaviour
{
    Button Dimed;
    Button Button_Close;
    void Start()
    {
        Dimed = transform.Find("Dimed").GetComponent<Button>();

        Button_Close = transform.Find("Button_Close02").GetComponent<Button>();

        Dimed.onClick.AddListener(ClosePopup);

        Button_Close.onClick.AddListener(ClosePopup);
    }

    void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
