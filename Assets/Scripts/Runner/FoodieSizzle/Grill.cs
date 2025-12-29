using DG.Tweening;
using System.Collections;
using UnityEngine;
using static UnityEditor.Progress;

public class Grill : MonoBehaviour
{
    public GrillSlot[] slots = new GrillSlot[3];
    public IngredientItem[] NextItems = new IngredientItem[3];

    public GameObject prefabItem = null;
    public bool IsBusy { get; private set; }

    [Header("Timings")]
    public float placeMoveTime = 0.08f;
    public float explodeTime = 0.10f;

    // 연쇄를 “원천 차단”하려면: 한 번 Place 처리 시 여기서만 체크하고 끝.
    // (폭발로 비워져도 다른 그릴을 자동 검사하지 않음)

    private void Start()
    {
        for (int i = 0; i < NextItems.Length; i++)
        {
            NextItems[i].Shake(true);
        }
    }

    public void TryPlace(IngredientItem item , GrillSlot slot)
    {
        if (IsBusy) return; //뭔가 연출하는데 들어옴..

        StartCoroutine(CoPlaceAndCheck(item, slot));

    }
    public void RemovePlace(IngredientItem item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].Current == item)
            {
                slots[i].Remove();
                Debug.Log("삭제완");
             }
        }
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < slots.Length; i++)
            if (!slots[i].IsEmpty) return false;

        return true;
    }

    IEnumerator CoPlaceAndCheck(IngredientItem item, GrillSlot targetSlot)
    {
        IsBusy = true;
        item.Lock(true);

        // 1) targetSlot anchor로 이동(간단 lerp)
        yield return CoMoveTo(item.transform, targetSlot.anchor, placeMoveTime);

        // 2) 슬롯에 고정 배치
        targetSlot.Place(item);

        // 3) 3개 다 찼으면 판정
        if (IsFull())
        {
            if (IsAllSameType(out var sameType))
            {
                // 폭발 처리
                yield return CoExplode(sameType);
            }
            // else: 아무 일 없음(그대로 유지)
        }

        item.Lock(false);
        IsBusy = false;
    }

    bool IsFull()
    {
        for (int i = 0; i < slots.Length; i++)
            if (slots[i].IsEmpty) return false;
        return true;
    }

    bool IsAllSameType(out IngredientType type)
    {
        type = default;
        if (!IsFull()) return false;

        var a = slots[0].Current.type;
        var b = slots[1].Current.type;
        var c = slots[2].Current.type;

        type = a;
        return a == b && b == c;
    }

    IEnumerator CoExplode(IngredientType type)
    {
        // 간단 팝 + 삭제
        for (int i = 0; i < slots.Length; i++)
        {
            var it = slots[i].Current;
            if (it != null) yield return it.CoPop(explodeTime);
        }

        // 실제 삭제/비우기
        for (int i = 0; i < slots.Length; i++)
        {
            var it = slots[i].Remove();
            if (it != null) Destroy(it.gameObject);
        }

        yield return new WaitForSeconds(0.2f);


        yield return NextItemSet();
        // 여기서 점수/주문 달성 이벤트를 쏴도 됨
        // OnExploded?.Invoke(this, type);
    }
    IEnumerator NextItemSet()
    {
        for (int i = 0; i < NextItems.Length; ++i)
        {
            NextItems[i].transform.DORotate(Vector3.zero, 0.2f);
            NextItems[i].transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f);
            NextItems[i].transform.DOMove(slots[i].transform.position, 0.2f);
        }

        yield return new WaitForSeconds(0.2f);


        for (int i = 0; i < slots.Length; i++)
        {
            IngredientItem a = null;

            if (NextItems[i].type != IngredientType.None)
            {
                var item = GameObject.Instantiate(prefabItem, slots[i].transform);

                a = item.GetComponent<IngredientItem>();

                a.SetType(NextItems[i].type);

                slots[i].Current = a;
            }
            NextItems[i].transform.eulerAngles = new Vector3(0,0,-40f);
            NextItems[i].transform.localPosition = Vector3.zero;
            NextItems[i].transform.localScale = new Vector3(0.6f,0.6f,0.6f);
            NextItems[i].Shake(true);

        }
    }

    IEnumerator CoMoveTo(Transform item, Transform target, float time)
    {
        Vector3 p0 = item.position;
        Vector3 p1 = target.position;

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float u = time <= 0 ? 1f : Mathf.Clamp01(t / time);
            item.position = Vector3.Lerp(p0, p1, u);
            yield return null;
        }
        item.position = p1;
    }
}