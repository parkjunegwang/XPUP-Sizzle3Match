using System.Collections;
using UnityEngine;

public class GrillGameController : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;

    [Header("Masks")]
    public LayerMask ingredientMask; // 재료 레이어
    public LayerMask grillSlotMask;      // 그릴 슬롯 레이어
    public LayerMask grillMask;      // 그릴 레이어

    [Header("Drag")]
    public float dragStartThreshold = 8f; // 픽셀
    public float snapBackTime = 0.12f;

    IngredientItem _picked;
    Collider2D _pickedCol;
    GrillSlot _prevGrillSlot;
    Grill _prevGrill;

    Vector3 _originPos;
    Transform _originParent;
    int _originSibling;

    Vector3 _grabOffsetWorld;
    bool _pressing;
    bool _dragging;
    Vector2 _pressScreen;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) OnDown(Input.mousePosition);
        if (Input.GetMouseButton(0)) OnMove(Input.mousePosition);
        if (Input.GetMouseButtonUp(0)) OnUp(Input.mousePosition);
    }

    void OnDown(Vector2 screen)
    {
        _pressing = true;
        _dragging = false;
        _pressScreen = screen;

        var world = ScreenToWorld(screen);

        // 재료 먼저 선택
        var hit = Physics2D.Raycast(world, Vector2.zero, 0f, ingredientMask);
        var GrillHit = Physics2D.Raycast(world, Vector2.zero, 0f, grillSlotMask);
        
        var _PrevGrill = Physics2D.Raycast(world, Vector2.zero, 0f, grillMask);

        if (!hit.collider) { _picked = null; _pickedCol = null; return; }

        var item = hit.collider.GetComponent<IngredientItem>();
        if (item == null || item.IsLocked) { _picked = null; _pickedCol = null; return; }

        if(GrillHit.collider != null)
            _prevGrillSlot = GrillHit.collider.GetComponent<GrillSlot>();

        _prevGrill = _PrevGrill.collider.GetComponent<Grill>();

        _picked = item;
        _pickedCol = hit.collider;

        _originPos = _picked.transform.position;
        _originParent = _picked.transform.parent;
        _originSibling = _picked.transform.GetSiblingIndex();

        var m = ScreenToWorld(screen);
        m.z = _picked.transform.position.z;
        _grabOffsetWorld = _picked.transform.position - m;
    }

    void OnMove(Vector2 screen)
    {
        if (!_pressing || _picked == null) return;

        if (!_dragging)
        {
            // threshold 넘어가면 드래그 시작
            if (Vector2.Distance(screen, _pressScreen) < dragStartThreshold) return;

            _dragging = true;

            // 드래그 중엔 그릴 레이캐스트가 재료 콜라이더에 막히지 않게 콜라이더 끄기
            if (_pickedCol) _pickedCol.enabled = false;

            // 부모 분리 (UI면 canvas 최상단으로 올리는 로직으로 바꿔도 됨)
            _picked.transform.SetParent(null, true);
        }

        // 드래그 이동
        var m = ScreenToWorld(screen);
        m.z = _picked.transform.position.z;
        _picked.transform.position = m + _grabOffsetWorld;
    }

    void OnUp(Vector2 screen)
    {
        _pressing = false;

        if (_picked == null) return;

        if (!_dragging)
        {
            // “클릭”만 하고 끝낸 경우: (원하면 여기서 기존 클릭 로직 유지 가능)
            // 지금은 아무 것도 안 하고 해제
            _picked = null;
            _pickedCol = null;
            return;
        }

        // 드롭 처리
        var world = ScreenToWorld(screen);

        // 그릴만 정확히 뽑기 (OverlapPointAll + grillMask)
        var grillslot = FindTopGrillSlotAt(world);
        var grill = FindTopGrillAt(world);
        if (grillslot != null && grill != null)
        {
            // 드롭 성공 시: 그릴이 Place/애니 처리
            // (TryPlace 내부에서 item.Lock(true) 함)
         //   bool ok = grill.(_picked);
            bool ok = grillslot.Current == null;
        
            // 콜라이더는 다시 켜두자 (그릴 코루틴이 이동시키는 동안 콜라이더가 켜져도 상관 없음)
            if (_pickedCol) _pickedCol.enabled = true;

            if (ok)
            {
                grill.TryPlace(_picked, grillslot);
                _prevGrillSlot.Current= null;

                if (_prevGrill != grill)
                {
                    if (_prevGrill.IsEmpty())
                    {
                       // _prevGrill.NextItemSet();
                    }
                }
                _picked = null;
                _prevGrill = null;
                _pickedCol = null;
                _prevGrillSlot = null;
                return;
            }
        }

        // 실패: 스냅백
        if (_pickedCol) _pickedCol.enabled = true;
        StartCoroutine(CoSnapBack(_picked.transform, _originParent, _originSibling, _originPos, snapBackTime));

        _prevGrill = null;
        _prevGrillSlot = null;
        _picked = null;
        _pickedCol = null;
    }

    Vector3 ScreenToWorld(Vector2 screen)
    {
        var w = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, 0f));
        w.z = 0f; // 2D 기준
        return w;
    }

    GrillSlot FindTopGrillSlotAt(Vector2 world)
    {
        // grillMask만 체크하니 재료가 절대 방해 못 함
        var hits = Physics2D.OverlapPointAll(world, grillSlotMask);
        if (hits == null || hits.Length == 0) return null;

        // 여러 개 겹칠 수 있으면 “가장 위”를 고르는 기준이 필요함.
        // 기본은 SortingOrder가 높은 쪽 우선. (없으면 첫번째)
        GrillSlot best = null;
        int bestOrder = int.MinValue;

        for (int i = 0; i < hits.Length; i++)
        {
            var g = hits[i].GetComponent<GrillSlot>();
            if (g == null) continue;

            int order = 0;
            var sr = hits[i].GetComponent<SpriteRenderer>();
            if (sr != null) order = sr.sortingOrder;

            if (best == null || order > bestOrder)
            {
                best = g;
                bestOrder = order;
            }
        }
        return best;
    }


    Grill FindTopGrillAt(Vector2 world)
    {
        // grillMask만 체크하니 재료가 절대 방해 못 함
        var hits = Physics2D.OverlapPointAll(world, grillMask);
        if (hits == null || hits.Length == 0) return null;
    
        // 여러 개 겹칠 수 있으면 “가장 위”를 고르는 기준이 필요함.
        // 기본은 SortingOrder가 높은 쪽 우선. (없으면 첫번째)
        Grill best = null;
        int bestOrder = int.MinValue;
    
        for (int i = 0; i < hits.Length; i++)
        {
            var g = hits[i].GetComponent<Grill>();
            if (g == null) continue;
    
            int order = 0;
            var sr = hits[i].GetComponent<SpriteRenderer>();
            if (sr != null) order = sr.sortingOrder;
    
            if (best == null || order > bestOrder)
            {
                best = g;
                bestOrder = order;
            }
        }
        return best;
    }
    IEnumerator CoSnapBack(Transform tr, Transform parent, int sibling, Vector3 targetPos, float time)
    {
        // 원래 부모로 복귀
        if (parent != null)
        {
            tr.SetParent(parent, true);
            tr.SetSiblingIndex(sibling);
        }

        Vector3 p0 = tr.position;
        Vector3 p1 = targetPos;

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float u = time <= 0f ? 1f : Mathf.Clamp01(t / time);
            tr.position = Vector3.Lerp(p0, p1, u);
            yield return null;
        }
        tr.position = p1;
    }
}
