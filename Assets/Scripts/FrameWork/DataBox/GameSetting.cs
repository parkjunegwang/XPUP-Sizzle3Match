using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.FrameWork.Job;

namespace Assets.Scripts.FrameWork.DataBox
{
    public enum ePrefabOrigin
    {
        NotInUse    ,       //사용하지 않음
        Common      ,       //공통 사용
        Individual  ,       //개별적 사용
    }

    class PrefabUseInfo
    {
        public string m_prefabName;                                       //종류
        public GameObject m_prefabLoaded = null;                          //로드된 프리팹

        public PrefabUseInfo(string _prefabName) => m_prefabName = _prefabName;
    }

    public class GameSetting : IJMData
    {   
        Dictionary<string, PrefabUseInfo> m_dicGamePrefabUserInfo = new();

        public void Clear() => m_dicGamePrefabUserInfo.Clear();

        public void SetLoadedPrefab(string prefabName, GameObject prefab)
        {
            if (!m_dicGamePrefabUserInfo.ContainsKey(prefabName))
            {
                PrefabUseInfo gamePrefabUseInfo = new (prefabName);
                m_dicGamePrefabUserInfo[prefabName] = gamePrefabUseInfo;
            }

            m_dicGamePrefabUserInfo[prefabName].m_prefabLoaded = prefab;
        }

        public GameObject GetLoadedPrefab(string prefabName)
        {
            if (!m_dicGamePrefabUserInfo.ContainsKey(prefabName))
                return null;

            return m_dicGamePrefabUserInfo[prefabName].m_prefabLoaded;
        }

        public bool IsLoadedPrefab(string prefabName)
        {
            if (!m_dicGamePrefabUserInfo.ContainsKey(prefabName))
                return false;

            return true;
        }
    }
}

