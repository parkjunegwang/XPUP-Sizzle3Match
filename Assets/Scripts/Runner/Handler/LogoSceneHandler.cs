using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoSceneHandler : MonoBehaviour
{
    private Slider m_LoadingBar;

    private TextMeshProUGUI m_textLoading;



    private Image m_LoadingCircle;
    private void Awake()
    {
        m_LoadingBar = GameObject.Find("LoadingBar").GetComponent<Slider>();
        m_textLoading = GameObject.Find("LoadingText").GetComponent<TextMeshProUGUI>();
        m_LoadingCircle = GameObject.Find("Loading_Circle").GetComponent<Image>();
    }

    private async void Start()
    {
        m_LoadingCircle.transform.DORotate(new Vector3(0, 0, 90f), 4f);


        DOTween.Sequence().AppendInterval(1)
                        .Append(DOTween.To(() => m_LoadingBar.value, x => m_LoadingBar.value = x, 1f, 1f).SetEase(Ease.Linear))
                        .AppendInterval(1)
                        .OnComplete(() => SceneManager.LoadSceneAsync(SceneDefine.LOBBY_SCENE_NAME));
    }

    private void Update()
    {
        if(m_textLoading != null)
        m_textLoading.text = m_LoadingBar.value * 100 + "%";
    }

}
