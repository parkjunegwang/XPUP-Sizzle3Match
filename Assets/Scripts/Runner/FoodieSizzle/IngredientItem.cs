
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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

        string path = "Image/Ingame/Item/" + type.ToString();

        a.sprite = Resources.Load<Sprite>(path);
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

        string path = "Image/Ingame/Item/" + type.ToString();

        a.sprite = Resources.Load<Sprite>(path);
    }
}
