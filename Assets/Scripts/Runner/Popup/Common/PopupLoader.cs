using System.Threading.Tasks;
using UnityEngine;

public static class PopupLoader
{
#if ADDRESSABLES
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
#endif

    public static async Task<T> LoadAsync<T>(string key) where T : Popup
    {
#if ADDRESSABLES
        // Addressables 키로 프리팹 로드
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
        await handle.Task;
        if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null) return null;
        return handle.Result.GetComponent<T>();
#else
        // Resources/Popup/ 이하에서 로드한다고 가정
        var go = Resources.Load<GameObject>(key);
        if (go == null) return null;
        return go.GetComponent<T>();
#endif
    }
}
