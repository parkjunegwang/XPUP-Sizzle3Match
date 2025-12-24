using UnityEngine;
using System;
using System.Threading.Tasks;

using Assets.Scripts.FrameWork.DataBox;
using Assets.Scripts.FrameWork.Job;

namespace Assets.Scripts.FrameWork.Manager.Popup
{
    public class PopupABAction : JMAction
    {
        protected float m_fFinishDelay = 0.1f;



        private string m_popupName = string.Empty;
        private Action<GameObject> m_callback; 


        public PopupABAction()
        {
        }

        public PopupABAction(string strPopupName) => m_popupName = strPopupName;

        public PopupABAction(string strPopupName, Action<GameObject> ret = null)
        {
            m_popupName = strPopupName;
            SetLoadCallback(ret);
        }

        protected override void OnEnter()
        {
            var gameSetting = JobMaker.GlobalDataBox.GetData<GameSetting>();
            GameObject prefab = gameSetting.GetLoadedPrefab(m_popupName);
            
            LoadByPrefab(prefab);
        }

        virtual protected void InitPopup()
        {
        }

        protected void SetPrefabName(string name) => m_popupName = name;

        protected string GetPrefabName() => m_popupName;

        protected void SetLoadCallback(Action<GameObject> ret) => m_callback += ret;

        protected async Task CatchLoadByPrefab(string strPrefabName)
        {
            //리소스에서 읽어온다
            GameObject prefab = Resources.Load($"Prefabs/UI/{strPrefabName}") as GameObject;

            //리소시스로 해결하자
            //prefab ??= await Addressables.LoadAssetAsync<GameObject>(strPrefabName).Task;
            //if (prefab != null)
            //{
            //    var gameSetting = JobMaker.GlobalDataBox.GetData<GameSetting>();
            //    gameSetting.SetLoadedPrefab(strPrefabName, prefab);
            //}
        }

        protected async void LoadByPrefab(GameObject prefab, bool isDoNot = false, bool isOnlyLoad = false)
        {
            if (string.IsNullOrEmpty(m_popupName) == false)
            {
                if (prefab == null)
                    prefab = Resources.Load($"Prefabs/{m_popupName}") as GameObject;

                //리소시스로 해결하자
              //  if (prefab == null)
                 //   prefab = await Addressables.LoadAssetAsync<GameObject>(m_popupName).Task;

                if (prefab != null)
                {
                    if (isOnlyLoad == false)
                    {
                        var gameSetting = JobMaker.GlobalDataBox.GetData<GameSetting>();
                        gameSetting.SetLoadedPrefab(m_popupName, prefab);
                    }


                    GameObject obj = UnityEngine.Object.Instantiate(prefab);
                    if (obj != null)
                    {
                        obj.name = prefab.name;
                        obj.SetActive(true);

                        InitPopup();
                    }

                    m_callback?.Invoke(obj);
                }
            }
        }

        public static async Task<GameObject> OnLoadPopupPrefab(string prefabName, bool isDoNot = false, bool isOnlyLoad = false)
        {
            GameObject prefab = JobMaker.GlobalDataBox.GetData<GameSetting>().GetLoadedPrefab(prefabName);

            if (prefab == null)
                //리소스에서 읽어온다
                prefab = Resources.Load($"Prefabs/UI/{prefabName}") as GameObject;

            //리소시스로 해결하자
           // if (prefab == null)
              //  prefab = await Addressables.LoadAssetAsync<GameObject>(prefabName).Task;

            if (prefab != null)
            {
                if (isOnlyLoad == false)
                {
                    var gameSetting = JobMaker.GlobalDataBox.GetData<GameSetting>();
                    gameSetting.SetLoadedPrefab(prefabName, prefab);
                }

                GameObject obj = UnityEngine.Object.Instantiate(prefab);
                if (obj != null)
                {
                    obj.name = prefab.name;
                    obj.SetActive(true);                    
                }

                return obj;
            }

            return null;
        }
    }
}

