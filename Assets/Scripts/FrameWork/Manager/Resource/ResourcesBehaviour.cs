using UnityEngine;

namespace Assets.Scripts.FrameWork.Manager.Resource
{
    [System.Serializable]
    public class ResourcesBehaviour : MonoBehaviour
    {
        protected Job.JMDataBox m_sceneDataBox;

        protected virtual void Start()
        {
        }

        virtual protected void OnDestroy() => OnResourcesDestroy();

        public virtual void OnCreate()
        {   
        }

        virtual protected void OnResourcesDestroy()
        {
        }

        virtual public void ShowObj(bool show) => gameObject.SetActive(show);

        virtual public void RemoveObj() => Destroy(gameObject);

        public void SetSceneDataBox(Job.JMDataBox dataBox) => m_sceneDataBox = dataBox;
    }
}
