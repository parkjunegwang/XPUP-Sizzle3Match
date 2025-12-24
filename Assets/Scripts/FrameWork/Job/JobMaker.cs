using System.Collections.Generic;
using System;

namespace Assets.Scripts.FrameWork.Job
{    
    public class JobMaker 
    {   
        private static readonly JMDataBox s_dataBox = new ();
        private static readonly List<JMFSM> s_lstFSM = new ();
        private static readonly Queue<string> s_triggerEvent = new ();
        private static readonly List<JMActionMgr> s_actionOverriders = new();

        public static JMDataBox GlobalDataBox => s_dataBox;

        static public void Destroy() => Clear();


        [Obsolete("JMFSM을 직접 선언 해서 사용 하세요. 예> JMFSM t = new tempJMFSM();", false)]
        public static JMFSM CreateFSM(string name)
        {
            return new (name);
        }

        [Obsolete("JMFSM을 직접 선언 해서 사용 하세요. 예> JMFSM t = new tempJMFSM();", false)]
        public static T CreateFSM<T>(params object[] args) where T : JMFSM
        {
            return CreateAction<T>(args) as T;
        }

        internal static void RegisterFSM(JMFSM fsm)
        {
            /*if (GetFSM(fsm.Name) != null)
            {   
            }*/

            s_lstFSM.Add(fsm);
        }
                
        public static JMFSM GetFSM(string name)
        {
            foreach (var f in s_lstFSM)
            {
                if (f.Name.Equals(name))
                    return f;
            }

            return null;      
        }

        public static bool ExistFSM<T>() where T : JMFSM
        {
            for (int i = s_lstFSM.Count - 1; i >= 0; i--)
            {
                if (s_lstFSM[i] is T)
                    return true;
            }

            return false;
        }

        public static void RemoveFSM(string name) => RemoveFSM(GetFSM(name));

        public static void RemoveFSM<T>() where T : JMFSM
        {
            for (int i = s_lstFSM.Count - 1 ; i >=0 ; i--)
            {
                if (s_lstFSM[i] is T)
                    s_lstFSM.RemoveAt(i);
            }
        }

        public static void RemoveFSM(JMFSM fsm)
        {
            if (fsm != null)
            {                
                if (!s_lstFSM.Remove(fsm))
                {   
                }
            }
        }

       
        public static void Clear() => s_lstFSM.Clear();
           
        public static void ClearEventQeue() => s_triggerEvent.Clear();
        
        public static void Update()
        {
            for (int i = 0; i < s_lstFSM.Count; i++)
            {
                if (s_triggerEvent.Count > 0)
                {
                    if (s_lstFSM[i].IsActive)
                    {
                        string eventId = s_triggerEvent.Peek();
                        if (s_lstFSM[i].TriggerGlobalEvent(eventId))
                        {
#if !LIVE
                            //로그 기록
                            UnityEngine.Debug.Log(string.Format("Event => {0}", eventId));
#endif

                            s_triggerEvent.Dequeue();
                        }
                    }
                }

                s_lstFSM[i].Update();
            }
        }


        public static void TriggerGlobalEvent(string eventName) => s_triggerEvent.Enqueue(eventName);

        public static void AddActionOverrider(JMActionMgr mgr) => s_actionOverriders.Add(mgr);

        public static void RemoveActionOverrider(JMActionMgr mgr) => s_actionOverriders.Remove(mgr);

        public static JMAction CreateAction<T>(params object[] args) where T : JMAction
        {           
            foreach(var o in s_actionOverriders)
            {
                JMAction action = o.OverrideAction<T>(args);
                if(action != null)
                    return action;
            }

            Type targetType = typeof(T);
                        
            if (targetType.Name.IndexOf("$") > 0 ||
                targetType.Name.IndexOf("`") > 0)     //우회적방법 오류
                return (T)Activator.CreateInstance(targetType, new object[] { args });
            else
                return (T)Activator.CreateInstance(targetType, args);            
        }

        //씬전환시 제거
        public static void ClearOnLoad()
        {
            for (int i = s_lstFSM.Count - 1; i >= 0; i--)
            {
                if (!s_lstFSM[i].DontDestroyOnLoad && s_lstFSM[i].AutoDestroy)
                    s_lstFSM[i].DestroyFSM();
            }
        }
      
    }


}