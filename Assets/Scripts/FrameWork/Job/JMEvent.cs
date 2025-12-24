using System.Collections.Generic;

namespace Assets.Scripts.FrameWork.Job
{
    public class JMEvent
    {
        public static readonly string FSM_FINISHED = "FSM_FINISHED";
        public static readonly string FSM_START = "FSM_START";
        public static readonly string ACTION_TRIGGER_EVENT = "ACTION_TRIGGER_EVENT";

        public static readonly string FINISHED = "FINISHED";
    }

    public class EventArg
    {
        public object target;
        public object objParam;

        public string ParamAsString => objParam as string;

        public int ParamAsInt => objParam == null ? 0 : (int)objParam;


        public EventArg()
        {         
        }

        public EventArg(string param) => objParam = param;

        public EventArg(int param) => objParam = param;
    }

    public class EventDispatcher
    {
        public delegate void EventHandlerDelegte(EventArg e);

        private readonly Dictionary<string, List<EventHandlerDelegte> > m_dicHandlers = new ();
               
      
        virtual public void AddEventListener(string strEventID, EventHandlerDelegte callback)
        {           

            if (m_dicHandlers.TryGetValue(strEventID, out var evtListeners))
            {
                evtListeners.Remove(callback);
                evtListeners.Add(callback);
            }
            else
            {
                evtListeners = new() { callback };

                m_dicHandlers.Add(strEventID, evtListeners);
            }
        }

        virtual public void DispatchEvent(string strEventID, EventArg e = null)
        {
            e ??= new()
            {
                target = this
            };

            if (m_dicHandlers.TryGetValue(strEventID, out var evtListeners))
            {
                for (int i = 0; i < evtListeners.Count; i++)
                    evtListeners[i](e);
            }
        }

        virtual public void RemoveEventListener(string strEventID, EventHandlerDelegte callback)
        {            
            if (m_dicHandlers.TryGetValue(strEventID, out var evtListeners))
            {
                for (int i = 0; i < evtListeners.Count; i++)
                    evtListeners.Remove(callback);
            }
        }

        virtual public void RemoveAllEventListener() => m_dicHandlers.Clear();
    }

    public class JMEntity : EventDispatcher
    {
        
        protected JMEntity m_parent;
        protected JMDataBox m_dataBox;
        
        private readonly List<JMEntity> m_lstObservers = new ();

        public bool IsActive { get; set; }

        public string Name { get; set; }

        public virtual void SetParent(JMEntity parent) => m_parent = parent;

        public virtual JMDataBox DataBox
        {
            get
            {
                if (m_dataBox != null)
                    return m_dataBox;
                else if (m_parent != null)
                    return m_parent.DataBox;
                else
                    return m_dataBox = new ();
            }

            set { m_dataBox = value; }
        }

        public virtual JMState ParentState
        {
            get
            {
                if (m_parent is JMState)
                    return m_parent as JMState;
                else if (m_parent != null)
                    return m_parent.ParentState;
                else
                    return null;
            }
        }

        public virtual JMFSM ParentFSM
        {
            get
            {
                if (m_parent is JMFSM)
                    return m_parent as JMFSM;
                else if (m_parent != null)
                    return m_parent.ParentFSM;
                else
                    return null;
            }
        }

        public virtual JMEntity TopParent
        {
            get
            {
                if (m_parent != null)
                    return m_parent.TopParent;
                else
                    return null;
            }
        }

        public virtual JMFSM TopParentFSM
        {
            get
            {
                if (m_parent is JMFSM)
                {
                    if (m_parent.ParentFSM == null)
                        return m_parent as JMFSM;
                    else
                        return m_parent.TopParentFSM;
                }
                else if (m_parent != null)
                    return m_parent.TopParentFSM;
                else
                    return null;
            }
        }

        public virtual void AddObserver(JMEntity observer) => m_lstObservers.Add(observer);

        public virtual void RemoveObserver(JMEntity observer) => m_lstObservers.Remove(observer);

        protected virtual bool OnEventTriggered(string strEventID)
        {
            DispatchEvent(strEventID);
            return false;
        }

        public virtual bool TriggerEvent(string strEventID)
        {
             if (OnEventTriggered(strEventID))
                return true;

            foreach (var o in m_lstObservers)
            {
                if (o.TriggerEvent(strEventID))
                    return true;
            }

            return false;
        }
    }
}