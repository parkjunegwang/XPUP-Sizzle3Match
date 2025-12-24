using UnityEngine;

public class GrillSlot : MonoBehaviour
{
    public Transform anchor; // 아이템이 붙을 위치(없으면 자기 자신)
    public IngredientItem Current { get; set; }

    void Awake()
    {
        if (anchor == null) anchor = transform;

        var a = transform.Find("Item");
        if (a != null)
        {
            Current = a.GetComponent<IngredientItem>();
            Current.Shake(false);
        }
        
    }

    public bool IsEmpty => Current == null;

    public void Place(IngredientItem item)
    {
        Current = item;
        item.transform.SetParent(anchor, worldPositionStays: false);
        item.transform.localScale = Vector3.one;
        item.transform.localPosition = Vector3.zero;
    }
    

    public IngredientItem Remove()
    {
        var it = Current;
        Current = null;
        return it;
    }
}
