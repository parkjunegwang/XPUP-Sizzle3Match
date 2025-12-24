using UnityEngine;

public static class PrefabUtil
{
    public static GameObject Instantiate(GameObject prefab, Transform tfmParent)
    {
        if (prefab == null)
            return null;

        GameObject obj = Object.Instantiate(prefab);
        obj.name = prefab.name;

        if (tfmParent != null)
        {
            obj.layer = tfmParent.gameObject.layer;
            obj.transform.SetParent(tfmParent);
        }
            
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;

        return obj;
    }

    public static GameObject InstantiateNoLoad(GameObject objTarget, Transform tfmParent)
    {
        if (objTarget == null)
            return null;

        if (tfmParent != null)
        {
            objTarget.layer = tfmParent.gameObject.layer;
            objTarget.transform.SetParent(tfmParent);
        }

        objTarget.transform.localScale = Vector3.one;
        objTarget.transform.localPosition = Vector3.zero;

        return objTarget;
    }

    public static GameObject InstantiateNoPS(GameObject prefab, Transform tfmParent)
    {
        if (prefab == null)
            return null;

        GameObject obj = Object.Instantiate(prefab, tfmParent);

        return obj;
    }
}