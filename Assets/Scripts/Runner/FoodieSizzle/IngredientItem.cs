
using System.Collections;
using UnityEngine;

public class IngredientItem : MonoBehaviour
{
    public IngredientType type;

    // 드래그/선택용 상태
    public bool IsLocked { get; private set; }

 

    public void Lock(bool v) => IsLocked = v;

    // 간단 애니: 스케일 팝
    public IEnumerator CoPop(float time)
    {
        Vector3 s0 = Vector3.one;
        Vector3 s1 = Vector3.one * 1.15f;

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / time);
            transform.localScale = Vector3.Lerp(s0, s1, u);
            yield return null;
        }

        t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / time);
            transform.localScale = Vector3.Lerp(s1, s0, u);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
    public void SetType(IngredientType _type)
    {
        if (_type == IngredientType.None)
        {
            Debug.LogError("들어올수없는 녀석");
        }
        type = _type;
        var a = GetComponent<SpriteRenderer>();

        switch (_type)
        {
            case IngredientType.Meat:

                a.color = Color.white;
                break;
            case IngredientType.Onion:
                a.color = Color.orange;
                break;
            case IngredientType.Pepper:
                a.color = Color.red;
                break;
            case IngredientType.Mushroom:
                a.color = Color.gray;
                break;
            case IngredientType.Tomato:
                a.color = Color.blue;
                break;
            case IngredientType.Sauce:
                a.color = Color.green;
                break;
     
        }
    }
    public void Shake(bool isNone)
    {
        if (isNone)
        {
            type = (IngredientType)Random.Range(0, 7);
        }
        else
        {
            type = (IngredientType)Random.Range(0, 6);
        }

       

        var a = GetComponent<SpriteRenderer>();

        switch (type)
        {
            case IngredientType.Meat:

                a.color = Color.white;
                break;
            case IngredientType.Onion:
                a.color = Color.orange;
                break;
            case IngredientType.Pepper:
                a.color = Color.red;
                break;
            case IngredientType.Mushroom:
                a.color = Color.gray;
                break;
            case IngredientType.Tomato:
                a.color = Color.blue;
                break;
            case IngredientType.Sauce:
                a.color = Color.green;
                break;
            case IngredientType.None:
                a.color = new Color(0,0,0,0);
                break;
        }
    }
}
