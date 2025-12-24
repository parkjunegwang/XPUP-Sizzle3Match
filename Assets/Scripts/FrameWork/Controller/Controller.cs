using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.SceneManagement;
//using UnityEngine.AddressableAssets;




namespace Assets.Scripts.FrameWork.Controller
{
    public static class Controller
    {
        static public void InitController() => OnControllerInit();

        static public void OnControllerInit()
        {
            DOTween.Init();
            DOTween.SetTweensCapacity(500, 50);         
        }
        
        static public void LoadScene(string scene, LoadSceneMode mode, Action callback)
        {
            var asyncop = SceneManager.LoadSceneAsync(scene, mode);
            asyncop.completed += operation => callback?.Invoke();
        }

        static public void LoadSceneBundle(string scene, LoadSceneMode mode, Action callback)
        {
            //Addressables.LoadSceneAsync(scene, mode).Completed += r =>
            //{
            //    callback?.Invoke();
            //};
        }

        static public void UnloadUnusedAssets()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }

        static public void OnApplicationPause(bool pauseStatus)
        {            
        }
    }
}
