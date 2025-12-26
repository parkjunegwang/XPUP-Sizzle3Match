using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("UI Root")]
    [SerializeField] private Canvas canvasRoot;
    [SerializeField] private Transform popupRoot;
    [SerializeField] private int sortingOrder = 5000;

    // 열려있는 팝업 스택(맨 위가 Top)
    private readonly List<Popup> _stack = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureRoot();
        SceneManager.sceneLoaded += (_, __) => EnsureRoot(); // 씬이 바뀌어도 Root 유지 확인
    }

    private void EnsureRoot()
    {
        if (canvasRoot != null && popupRoot != null) return;

        // 루트가 없으면 자동 생성
        if (canvasRoot == null)
        {
            var go = new GameObject("PopupCanvasRoot");
            go.transform.SetParent(transform, false);

            canvasRoot = go.AddComponent<Canvas>();
            canvasRoot.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasRoot.sortingOrder = sortingOrder;

            go.AddComponent<UnityEngine.UI.CanvasScaler>();
            go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        if (popupRoot == null)
        {
            var root = canvasRoot.transform;
       
            popupRoot = root;
        }
    }

    public bool HasOpenPopup => _stack.Count > 0;
    public Popup Top => _stack.Count > 0 ? _stack[^1] : null;

    /// <summary>
    /// 프리팹을 받아서 Open (씬 어디서든 호출 가능)
    /// </summary>
    public T Open<T>(T prefab, object args = null) where T : Popup
    {
        EnsureRoot();

        var instance = Instantiate(prefab, popupRoot);
        instance.Owner = this;
        instance.Open(args);
        _stack.Add(instance);
        RefreshSorting();
        return instance;
    }

    /// <summary>
    /// 문자열 key로 로드 후 Open (Addressables 또는 Resources)
    /// </summary>
    public async Task<T> OpenAsync<T>(string key, object args = null) where T : Popup
    {
        EnsureRoot();

        var prefab = await PopupLoader.LoadAsync<T>(key);
        if (prefab == null)
            throw new Exception($"Popup prefab not found. key={key}");

        return Open(prefab, args);
    }

    public void CloseTop()
    {
        if (_stack.Count == 0) return;
        Close(_stack[^1]);
    }

    public void Close(Popup popup)
    {
        if (popup == null) return;

        int idx = _stack.IndexOf(popup);
        if (idx < 0) return;

        // 스택 중간 팝업 닫을 경우: 위에 있는 것들도 같이 닫을지 정책 선택
        // 여기서는 "해당 팝업 위에 있는 것까지 모두 닫기"로 통일(안전).
        for (int i = _stack.Count - 1; i >= idx; i--)
        {
            var p = _stack[i];
            _stack.RemoveAt(i);
            if (p != null)
            {
                p.Close();
                Destroy(p.gameObject);
            }
        }

        RefreshSorting();
    }

    public void CloseAll()
    {
        for (int i = _stack.Count - 1; i >= 0; i--)
        {
            var p = _stack[i];
            if (p != null)
            {
                p.Close();
                Destroy(p.gameObject);
            }
        }
        _stack.Clear();
    }

    private void RefreshSorting()
    {
        // 필요 시 팝업 별로 sorting order를 증가시키고 싶으면 Canvas 추가해서 제어.
        // 여기서는 단순히 형제 순서를 스택 순서로 맞춤.
        for (int i = 0; i < _stack.Count; i++)
        {
            if (_stack[i] != null)
                _stack[i].transform.SetSiblingIndex(i);
        }
    }
}
