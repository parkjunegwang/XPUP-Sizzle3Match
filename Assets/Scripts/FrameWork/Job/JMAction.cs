
namespace Assets.Scripts.FrameWork.Job
{
    public class JMAction : JMEntity
    {
        protected bool m_validTriggerd = false;


        public virtual void Run()
        {
            Reset();
            
            IsActive = true;

            OnEnter();
        }

        public virtual void Reset()
        {
            m_validTriggerd = false;

            Exit();
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnExit()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        public virtual void Update()
        {
            if (IsActive == true)
                OnUpdate();
        }

        public virtual void Finish()
        {
            Exit();
            TriggerEvent(JMEvent.FINISHED);
        }
      
        public virtual void Exit()
        {
            if (IsActive == true)
            {
                IsActive = false;

                OnExit();
            }
        }

        public override bool TriggerEvent(string eventName)
        {
            if (m_validTriggerd == false)
            {
                if (base.TriggerEvent(eventName))
                {
                    m_validTriggerd = true;
                    return true;
                }
            }

            return false;
        }

        public virtual void DestroyFSM()
        {
        }
    };


    public class JMActionInfo : JMEntity
    {
        private bool m_isActionFSM;     //JMFSM type인가?
        private JMAction m_action;


        public JMActionInfo(JMAction action) => Action = action;

        public bool IsFinished { get; set; }

        public JMAction Action
        {
            get { return m_action; }
            set
            {
                if (m_action != null && value != m_action)
                    m_action.RemoveObserver(this);
                
                m_action = value;
                m_action?.AddObserver(this);

                m_isActionFSM = m_action is JMFSM;
            }
        }

        protected override bool OnEventTriggered(string strEventID)
        {
            if (strEventID == JMEvent.FINISHED)
                IsFinished = true;
            
            return false;
        }

        public void RunAction() => m_action.Run();

        public void Destroy() => m_action?.RemoveObserver(this);

        public void Reset()
        {
            IsFinished = false;
            m_action.Reset();
        }

        public void UpdateAction()
        {
            if (m_isActionFSM == false)
                m_action.Update();      //FSM타입이 아닐때만...// FSM타입은 자체적으로 update가 된다
        }
    }
}

