using Assets.Scripts.FrameWork.DataBox;
using Assets.Scripts.FrameWork.Job;
using System;
using System.Threading.Tasks;
using UnityEngine;
//using UnityEngine.AddressableAssets;

namespace Assets.Scripts.FrameWork.Manager.Resource
{
    public class ResourcesABAction : JMAction
    {
        protected ResourcesBehaviour m_resourcesBehaviour;

        private readonly string m_resourcesPath = string.Empty;
        private string m_resourcesName = string.Empty;

        public ResourcesABAction()
        {
        }

        public ResourcesABAction(string strPrefabPath, string strPrefabName)
        {
            m_resourcesPath = strPrefabPath;
            m_resourcesName = strPrefabName;
        }

        protected override void OnEnter()
        {
            var gameSetting = JobMaker.GlobalDataBox.GetData<GameSetting>();
            GameObject prefab = gameSetting.GetLoadedPrefab(m_resourcesName);

            var obj = LoadByPrefab(prefab);
            if (obj != null)
                obj.SetActive(true);
        }

        [Obsolete("직접 구현해서 사용하세요", false)]
        protected void CatchLoadByPrefab(string strPrefabPath, string strPrefabName)
        {
            if (m_resourcesPath.Contains("UI"))
            {
#if UNITY_EDITOR
                Debug.Log("Use PopupABAction Class!");
#endif
                Finish();
                return;
            }

            //리소스에서 읽어온다
            GameObject prefab = Resources.Load($"Prefabs/{strPrefabName}") as GameObject;

            if (prefab == null)
            {
                Finish();
                return;
            }
            else
            {
                var gameSetting = JobMaker.GlobalDataBox.GetData<GameSetting>();
                gameSetting.SetLoadedPrefab(strPrefabName, prefab);
            }
        }

        [Obsolete("직접 구현해서 사용하세요", false)]
        protected async Task CatchLoadByBundlePrefab(string strPrefabName)
        {
            var gameSetting = JobMaker.GlobalDataBox.GetData<GameSetting>();

            if (gameSetting.IsLoadedPrefab(strPrefabName))
                return;

            //리소시스로 해결하자
            //var prefab = await Addressables.LoadAssetAsync<GameObject>(strPrefabName).Task;                        
           // gameSetting.SetLoadedPrefab(strPrefabName, prefab);
        }

        [Obsolete("직접 구현해서 사용하세요", false)]
        protected GameObject LoadByPrefab(GameObject prefab)
        {
            if (prefab == null)
            {
                if (m_resourcesPath.Contains("UI"))
                {
#if UNITY_EDITOR
                    Debug.Log("Use PopupABAction Class!");
#endif
                    return null;
                }

                //리소스에서 읽어온다
                prefab = Resources.Load($"Prefabs/{m_resourcesName}") as GameObject;

                if (prefab == null)
                {
                    Finish();
                    return null;
                }
                else
                {
                    var gameSetting = JobMaker.GlobalDataBox.GetData<GameSetting>();
                    gameSetting.SetLoadedPrefab(m_resourcesName, prefab);
                }
            }

            GameObject obj = GameObject.Instantiate(prefab);
                        
            if (obj != null)
            {
                obj.name = prefab.name;
                
                if (obj.TryGetComponent(out m_resourcesBehaviour))
                {
                    m_resourcesBehaviour.OnCreate();
                    m_resourcesBehaviour.SetSceneDataBox(m_dataBox);
                }
                else
                    Finish();
            }
            else
                Finish();

            return obj;
        }

        [Obsolete("직접 구현해서 사용하세요", false)]
        protected GameObject LoadByPrefab(string resourceName)
        {
            var gameSetting = JobMaker.GlobalDataBox.GetData<GameSetting>();
            GameObject prefab = gameSetting.GetLoadedPrefab(resourceName);

            GameObject obj = GameObject.Instantiate(prefab);
            if (obj != null)
            {                
                obj.name = prefab.name;
             
                if (obj.TryGetComponent(out m_resourcesBehaviour))
                {
                    m_resourcesBehaviour.OnCreate();
                    m_resourcesBehaviour.SetSceneDataBox(m_dataBox);
                }
                else
                    Finish();
            }
            else
                Finish();

            return obj;
        }
    }
}

