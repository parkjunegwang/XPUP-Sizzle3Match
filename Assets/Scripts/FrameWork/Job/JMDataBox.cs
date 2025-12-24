using System.Collections.Generic;

namespace Assets.Scripts.FrameWork.Job
{
    public interface IJMData
    {        
    }   
    
    [System.Serializable]
    public class JMDataBox
    {
        private List<IJMData> m_lstData = new ();
        private readonly Dictionary<string, object> m_dicData = new();



        //없으면 생성
        public T GetData<T>() where T : IJMData
        {
            foreach (var d in m_lstData)
            {
                if (d is T dt)
                    return dt;
            }

            T t = (T)System.Activator.CreateInstance(typeof(T));
            m_lstData.Add(t);

            return t;
        }

        public T AddData<T>() where T : IJMData
        {
            return GetData<T>();
        }

        //없으면null 리턴
        public T FindData<T>() where T : IJMData
        {
            foreach (var d in m_lstData)
            {
                if (d is T t)
                    return t;
            }

            return default;
        }
        public void RemoveData<T>() where T : IJMData
        {
            foreach (var d in m_lstData)
            {
                if (d is T)
                {
                    m_lstData.Remove(d);
                    break;
                }
            }
        }


        public void SetValue(string key, object value)
        {
            if (m_dicData.ContainsKey(key))
                m_dicData[key] = value;
            else
                m_dicData.Add(key, value);
        }
        public T GetValue<T>(string key, T defaultvalue = default)
        {
            if (m_dicData.ContainsKey(key))
                return (T)m_dicData[key];
            else
                return defaultvalue;
        }

        public string GetDataAsString(string key, string defalut = "")
        {
            if (m_dicData.ContainsKey(key))
            {
                string s = m_dicData[key] as string;
                if (string.IsNullOrEmpty(s))
                    return s;
            }

            return defalut;
        }
    }
}

