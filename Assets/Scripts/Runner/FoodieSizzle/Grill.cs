using Assets.Scripts.FrameWork.Job;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using static UnityEditor.Progress;

public enum eTrayType
{ 
    None , //어차피 꺼져있을타입.
    Normal, // 노말타입
    Lock, //잠금 스타트
    NoMarge , //머지 안시킴
    Trailer, //계속움직이는 트레일러트레이
}
public class Grill : MonoBehaviour
{
    public bool isNoMarge = false;

    public bool isLock;
    public GameObject obj_Lock;
    public SpriteRenderer sr_LockImage;
    public IngredientType lockType;
    public bool isDirty;
    public GameObject obj_Dirty;

    public bool isBlind;
    public GameObject obj_TrayObject;
    public GrillSlot[] slots = new GrillSlot[3];
    public IngredientItem[] NextItems = new IngredientItem[3];

    public GameObject[]NextTray  = new GameObject[3];

    public GameObject prefabItem = null;
    public bool IsBusy { get; private set; }

    [Header("Timings")]
    public float placeMoveTime = 0.08f;
    public float explodeTime = 0.10f;

    public int RemaintrayCount = 0;
    // 연쇄를 “원천 차단”하려면: 한 번 Place 처리 시 여기서만 체크하고 끝.
    // (폭발로 비워져도 다른 그릴을 자동 검사하지 않음)
    public void SetType(eTrayType _type) 
    {
        var Data = JobMaker.GlobalDataBox.GetData<StageData>();
        switch (_type)
        {
            case eTrayType.Normal:
                break;
            case eTrayType.Lock:
                isLock = true;
                lockType = (IngredientType)Random.Range(0, Data.SaveData.ItemKind);
                break;
            case eTrayType.NoMarge:
                isNoMarge = true;
                break;
            case eTrayType.Trailer:
                break;
     
        }

    }
    public void InitializeSlot()
    {
        var Data = JobMaker.GlobalDataBox.GetData<StageData>();

        obj_Lock.SetActive(isLock);
        obj_TrayObject.SetActive(!isLock);
        if (isLock)
        {
            string path = "Image/Ingame/Item_Preview/" + lockType.ToString() + "_pre";

            sr_LockImage.sprite = Resources.Load<Sprite>(path);

            for (int i = 0; i < slots.Length; i++)
            {
                var it = slots[i].Remove();
                if (it != null) Destroy(it.gameObject);
            }
        }
        else
        {
           

            var empty = Random.Range(0, 4);

            switch (empty)
            {
                case 0:
                case 1:
                case 2:
                    var nextType = Data.GetNextItemData();
                    if (nextType != IngredientType.None)
                    {
                        var item = GameObject.Instantiate(prefabItem, slots[empty].transform);

                        IngredientItem a = item.GetComponent<IngredientItem>();

                        a.SetType(nextType);

                        slots[empty].Current = a;
                    }
                  
                    break;
                default:
                    int index = Random.Range(0,2) == 0 ? 0 : 1;

                    for (int i = 0; i < 2; ++i)
                    {
                        var type = Data.GetNextItemData();

                        if (type != IngredientType.None)
                        {
                            var item2 = GameObject.Instantiate(prefabItem, slots[index].transform);

                            IngredientItem a2 = item2.GetComponent<IngredientItem>();

                            a2.SetType(type);

                            slots[index].Current = a2;

                            index += 1;
                        }
                      
                    }

                    break;
            }
          

        }
        for (int i = 0; i < 3; i++)
        {
            NextItems[i].transform.eulerAngles = new Vector3(0, 0, -40f);
            NextItems[i].transform.localPosition = Vector3.zero;
            NextItems[i].transform.localScale = new Vector3(1f, 1f, 1f);
            NextItems[i].ShakeNextItem(IngredientType.None);

        }

        int nextindex = Random.Range(0, 2) == 0 ? 0 : 1;

        for (int i = 0; i < 2; i++)
        {
            NextItems[nextindex].ShakeNextItem(Data.GetNextItemData());

            nextindex++;
        }

    }
    public void SetRemaintrayCount(int count)
    {
        int emptyCount = 0;
        for (int i = 0; i < NextItems.Length; ++i)
        {
            if (NextItems[i].type == IngredientType.None)
            {
                emptyCount++;
            }
        }
        int defaultCount = 1;
        if (emptyCount == NextItems.Length)
        {
            defaultCount = 0;
        }

        RemaintrayCount = count + defaultCount;

        for (int i = 0; i < 3; ++i)
        {
            NextTray[i].SetActive(i < RemaintrayCount);
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
        if (IsFull() && !isNoMarge)
        {
            if (IsAllSameType(out var sameType))
            {
                InGameHandler.I.UnLockGrill(sameType);
                // 폭발 처리
                yield return CoExplode(sameType);
            }
            // else: 아무 일 없음(그대로 유지)
        }

        item.Lock(false);
        IsBusy = false;
    }

    public void UnLockGrill(IngredientType type)
    {
        if (!isLock) return;

        if (lockType == type)
        {
            isLock = false;
            obj_Lock.SetActive(false);
            obj_TrayObject.SetActive(true);

            EmptyItem(false);
        }
    }
    public IngredientType CompleteType()
    {
        IngredientType type = IngredientType.None;

        if (IsFull())
        {
            if (IsAllSameType(out var sameType))
            {
                type = sameType;
            }
        }
        return type;
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


        EmptyItem(false);


        InGameUIHandler.I.AddExp();
        // 여기서 점수/주문 달성 이벤트를 쏴도 됨
        // OnExploded?.Invoke(this, type);
    }
    public Vector3 MinusMoveItem(IngredientType type)
    {
        if (type == IngredientType.Bread_03
            || type == IngredientType.Cake_01
            || type == IngredientType.Cake_02
            || type == IngredientType.Cake_03
            || type == IngredientType.Cake_04)
        {
            return new Vector3(0f, +0.02f, 0f);
        }
        else
        {
            return Vector3.zero;
        }
    }


    //test
    int ClockCount = 0;
    //test
    public void AddCleanUpCount()
    {
        ClockCount += 1;
        if (ClockCount >= 3)
        {
            ClockCount = 0;
            isDirty = false;
            obj_Dirty.gameObject.SetActive(false);
            obj_TrayObject.SetActive(true);
            StartCoroutine(NextItemSet());
        }
    }

    public void EmptyItem(bool _isDirty = false)
    {
        //다음레벨이 있다면! 꼭체크해야함

        obj_Dirty.gameObject.SetActive(_isDirty);
        //없다면
        if (_isDirty)
        {
            isDirty = _isDirty;
            obj_TrayObject.SetActive(false);
        }
        else
        {
            StartCoroutine(NextItemSet());
        }
        
    }
    IEnumerator NextItemSet()
    {
        for (int i = 0; i < NextItems.Length; ++i)
        {
            NextItems[i].transform.DORotate(Vector3.zero, 0.2f);
            NextItems[i].transform.DOScale(new Vector3(1.8f, 1.8f,1.8f), 0.2f);

            Vector3 movePos = slots[i].transform.position - MinusMoveItem(NextItems[i].type);

            NextItems[i].transform.DOMove(movePos, 0.2f);
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
        }
        RemaintrayCount -= 1;

        for (int i = 0; i < 3; ++i)
        {
            NextTray[i].SetActive(i < RemaintrayCount);
        }
        for (int i = 0; i < 3; i++)
        {
            NextItems[i].transform.eulerAngles = new Vector3(0, 0, -40f);
            NextItems[i].transform.localPosition = Vector3.zero;
            NextItems[i].transform.localScale = new Vector3(1f, 1f, 1f);
            NextItems[i].ShakeNextItem(IngredientType.None);

        }

        if (RemaintrayCount <= 0)
        {
            
        }
        else
        {
            var Data = JobMaker.GlobalDataBox.GetData<StageData>();
            int nextindex = Random.Range(0, 2) == 0 ? 0 : 1;


            for (int i = 0; i < 2; i++)
            {
                NextItems[nextindex].ShakeNextItem(Data.GetNextItemData());

                nextindex++;
            }
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