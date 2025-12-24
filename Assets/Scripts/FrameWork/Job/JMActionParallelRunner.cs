using System.Collections.Generic;

namespace Assets.Scripts.FrameWork.Job
{
    public class JMActionParallelRunner : JMAction
    {
       
        public List<JMActionInfo> m_lstActionInfo = new ();

        public JMActionParallelRunner AddAction(JMAction action)
        {
            foreach (var a in m_lstActionInfo)
            {
                if (a.Action == action)
                    return this;
            }

            //아래 순서중요 (addObserver 순서)
            m_lstActionInfo.Add(new (action));

            action.SetParent(this);
            action.AddObserver(this);

            return this;
        }

        public JMActionParallelRunner AddAction<T>(params object[] args) where T : JMAction
        {
            AddAction(JobMaker.CreateAction<T>(args));
            return this;
        }
      
        public JMActionParallelRunner RemoveAction(JMAction action)
        {
            m_lstActionInfo.Add(new (action));

            foreach (var a in m_lstActionInfo)
            {
                if (a.Action == action)
                {
                    action.SetParent(null);
                    action.RemoveObserver(this);

                    a.Destroy();
                    m_lstActionInfo.Remove(a);
                    break;
                }
            }

            return this;
        }

        public override bool TriggerEvent(string strEventID)
        {
            if (strEventID == JMEvent.FINISHED) //모든 액션이 다 끝났을때만
            {
                if (IsAllFinished())
                    return base.TriggerEvent(strEventID);

                return true;
            }
            else
                return base.TriggerEvent(strEventID);
        }

        public override void Run()
        {
            base.Run();

            for (int i = 0; i < m_lstActionInfo.Count; ++i)
                m_lstActionInfo[i].RunAction();
        }
        public override void Update()
        {
            base.Update();

            for (int i = 0; i < m_lstActionInfo.Count; ++i)
                m_lstActionInfo[i].Action.Update();
          
        }
        public override void Reset()
        {
            base.Reset();

            foreach (var a in m_lstActionInfo)
                a.Reset();
        }
                
        public override void Exit()
        {
            base.Exit();

            foreach (var a in m_lstActionInfo)
                a.Action.Exit();
        }

        public bool IsAllFinished()
        {
            foreach (var a in m_lstActionInfo)
            {
                if (a.IsFinished == false)
                    return false;
            }

            return true;
        }

        public override void DestroyFSM()
        {
            base.DestroyFSM();

            foreach (var a in m_lstActionInfo)
                a.Action.DestroyFSM();
        }
    }
}