using Assets.Scripts.FrameWork.Manager.Popup;
using System;
using UnityEngine;

public abstract class Popup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup; // 없으면 자동으로 붙여줌

    public bool IsOpen { get; private set; }

    // PopupManager가 세팅
    internal PopupManager Owner { get; set; }

    protected virtual void Reset()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    protected virtual void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // 기본은 비활성
        SetVisible(false, instant: true);
    }

    public virtual void Open(object args = null)
    {
        IsOpen = true;
        gameObject.SetActive(true);
        SetVisible(true, instant: true);
        OnOpen(args);
    }

    public virtual void Close()
    {
        if (!IsOpen) return;

        IsOpen = false;
        OnClose();

        SetVisible(false, instant: true);
        gameObject.SetActive(false);
    }

    /// <summary>팝업 외부 클릭 등으로 닫히게 하고 싶으면 PopupManager에서 호출</summary>
    public void RequestClose() => Owner?.Close(this);

    protected abstract void OnOpen(object args);
    protected abstract void OnClose();

    private void SetVisible(bool on, bool instant)
    {
        canvasGroup.alpha = on ? 1f : 0f;
        canvasGroup.interactable = on;
        canvasGroup.blocksRaycasts = on;
    }
}
