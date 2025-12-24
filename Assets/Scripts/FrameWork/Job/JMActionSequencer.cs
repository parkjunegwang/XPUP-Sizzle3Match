using System.Collections.Generic;

namespace Assets.Scripts.FrameWork.Job
{
    public class JMActionSequencer : JMAction
    {
        protected List<JMActionInfo> m_lstActionInfo = new ();
        protected int m_iCurPostion = 0;

        public JMActionSequencer AddAction(JMAction action)
        {
            foreach (var a in m_lstActionInfo)
            {
                if (a.Action == action)
                    return this;
            }

            //아래 순서중요 (addObserver 순서)
            m_lstActionInfo.Add(new (action));

            action.SetParent(this);

            return this;
        }

        public JMActionSequencer AddAction<T>(params object[] args) where T : JMAction
        {
            JMAction action = JobMaker.CreateAction<T>(args);
            AddAction(action);

            return this;
        }

        public JMActionSequencer RemoveAction(JMAction action)
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
            if (strEventID == JMEvent.FINISHED)
            {
                RunNextAction();
                return true;
            }
            else
                return base.TriggerEvent(strEventID);
        }

        public override void Run()
        {
            base.Run();

            m_iCurPostion = -1;
            RunNextAction();
        }

        protected void RunNextAction()
        {
            if (m_iCurPostion >= 0 && m_iCurPostion < m_lstActionInfo.Count)
                m_lstActionInfo[m_iCurPostion].Action.RemoveObserver(this);

            //move to next
            m_iCurPostion++;

            if (m_iCurPostion >= m_lstActionInfo.Count)
            {
                Exit();
                base.TriggerEvent(JMEvent.FINISHED);
            }
            else
            {
                m_lstActionInfo[m_iCurPostion].Action.AddObserver(this);
                m_lstActionInfo[m_iCurPostion].RunAction();
            }
        }

        public override void Update()
        {
            base.Update();

            foreach (var a in m_lstActionInfo)
                a.Action.Update();

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
    }
}

