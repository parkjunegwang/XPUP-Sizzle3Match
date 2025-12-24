using UnityEngine;
using UnityEngine.UI;
public class PopupShop : MonoBehaviour
{
    Button Button_Close;
    void Start()
    {
        Button_Close = transform.Find("Bottom/Button_Back").GetComponent<Button>();

        Button_Close.onClick.AddListener(ClosePopup);
    }

    void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
