using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class BlindsTransition : MonoBehaviour
{
    public static BlindsTransition Instance { get; private set; }

    [Header("Layout")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform container;
    [SerializeField, Range(3, 30)] private int stripCount = 12;
    [SerializeField] private float duration = 0.45f;
    [SerializeField] private float stagger = 0.03f;     // 박스가 순차로 움직이는 간격
    [SerializeField] private Ease ease = Ease.OutCubic;

    [Header("Visual")]
    [SerializeField] private Color stripColor = Color.black;

    private readonly List<RectTransform> strips = new();
    private bool busy;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureCanvas();
        BuildStrips();
        SetOpenInstant(); // 시작은 “열린 상태”
    }

    private void EnsureCanvas()
    {
        // canvas / container가 없으면 자동 생성
        if (canvas == null)
        {
            var cgo = new GameObject("BlindsCanvas");
            cgo.transform.SetParent(transform, false);

            canvas = cgo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;

            cgo.AddComponent<CanvasScaler>();
            cgo.AddComponent<GraphicRaycaster>();
        }

        if (container == null)
        {
            var rgo = new GameObject("BlindsContainer");
            rgo.transform.SetParent(canvas.transform, false);
            container = rgo.AddComponent<RectTransform>();
            container.anchorMin = Vector2.zero;
            container.anchorMax = Vector2.one;
            container.offsetMin = Vector2.zero;
            container.offsetMax = Vector2.zero;
        }
    }

    private void BuildStrips()
    {
        // 기존 제거
        for (int i = strips.Count - 1; i >= 0; i--)
        {
            if (strips[i] != null) Destroy(strips[i].gameObject);
        }
        strips.Clear();

        // 가로 스트립(위→아래) 생성
        for (int i = 0; i < stripCount; i++)
        {
            var go = new GameObject($"Strip_{i}");
            go.transform.SetParent(container, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, (float)i / stripCount);
            rt.anchorMax = new Vector2(1, (float)(i + 1) / stripCount);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var img = go.AddComponent<Image>();
            img.color = stripColor;

            strips.Add(rt);
        }

        // 입력 막기용 CanvasGroup
        var cg = container.GetComponent<CanvasGroup>();
        if (cg == null) cg = container.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }

    // 열림 상태: 각 스트립이 X축 스케일 0 (안 보임)
    private void SetOpenInstant()
    {
        foreach (var rt in strips)
            rt.localScale = new Vector3(0f, 1f, 1f);

        container.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    // 닫힘 상태: 각 스트립이 X축 스케일 1 (전체 덮음)
    private void SetClosedInstant()
    {
        foreach (var rt in strips)
            rt.localScale = Vector3.one;

        container.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    Action action = null;
    public void ChangeScene(Action _action)
    {
        if (busy) return;

        action = _action;

        StartCoroutine(CoChange());
    }

    private System.Collections.IEnumerator CoChange()
    {
        busy = true;
        yield return Close();

        yield return new WaitForSeconds(1f);

        action?.Invoke();

        yield return new WaitForSeconds(1f);
        
        
        
        yield return Open();
        busy = false;
    }

    public Tween Close()
    {
        container.GetComponent<CanvasGroup>().blocksRaycasts = true;

        // 닫기: 스트립이 0 → 1로 커지며 덮기
        Sequence seq = DOTween.Sequence().SetUpdate(true);
        for (int i = 0; i < strips.Count; i++)
        {
            int idx = strips.Count - 1 - i; // 역순
            var rt = strips[idx];
            rt.DOKill();

            rt.localScale = new Vector3(0f, 1f, 1f);

            seq.Insert(i * stagger, rt.DOScaleX(1f, duration).SetEase(ease));
        }
        return seq;
    }

    public Tween Open()
    {
        // 열기: 1 → 0으로 줄어들며 시야 열기 (역방향이 멋있음)
        Sequence seq = DOTween.Sequence().SetUpdate(true);
        for (int i = 0; i < strips.Count; i++)
        {
            int idx = strips.Count - 1 - i; // 역순
            var rt = strips[idx];
            rt.DOKill();

            rt.localScale = Vector3.one;
            seq.Insert(i * stagger, rt.DOScaleX(0f, duration).SetEase(ease));
        }

        seq.OnComplete(() =>
        {
            container.GetComponent<CanvasGroup>().blocksRaycasts = false;
        });

        return seq;
    }

#if UNITY_EDITOR
    // 인스펙터에서 값 바꾸면 재생성
    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        if (container == null) return;

        BuildStrips();
        SetOpenInstant();
    }
#endif
}
