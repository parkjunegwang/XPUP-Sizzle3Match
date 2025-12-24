using System.Collections.Generic;

namespace Assets.Scripts.FrameWork.Job
{

    public class JMTransition
    {
        private JMState m_stateToTransit;          // 찾기 우선순위
        private readonly JMFSM m_fsm;
        private readonly string m_stateNameToTransit;       //stateToTransit 가 null이면 이름으로 찾는다.


        public JMTransition(JMState stateToTransit) => m_stateToTransit = stateToTransit;

        public JMTransition(JMFSM fsm, string stateNameToTransit)
        {
            m_fsm = fsm;
            m_stateNameToTransit = stateNameToTransit;
        }
        
        public JMState StateToTrasit
        {
            get
            {
                if (m_stateToTransit != null)
                    return m_stateToTransit;
                else
                    return m_fsm.GetState(m_stateNameToTransit);

            }
        }

        public void Transit() => StateToTrasit.StartState();
    }


    public class JMState : JMEntity
    {   
        protected Dictionary<string, JMTransition> m_dicTransition = new ();     //<event id, state to transit>
        
        private JMActionParallelRunner m_actionParallelRunner = new ();
        private JMState stateToTransit;


        public JMState() => InitState();

        public JMState(string name)
        {
            Name = name;
            InitState();
        }

        private void InitState()
        {
            m_dicTransition.Clear();
            m_actionParallelRunner.Reset();
            m_actionParallelRunner.SetParent(this);
            m_actionParallelRunner.AddObserver(this);
        }

        public JMState AddAction(JMAction action)
        {
            m_actionParallelRunner.AddAction(action);
            return this;
        }   
            
        public JMState AddAction<T>(params object[] args) where T : JMAction
        {
            m_actionParallelRunner.AddAction<T>(args);
            return this;
        }


        public void RemoveAction(JMAction action) => m_actionParallelRunner.RemoveAction(action);
      
        protected override bool OnEventTriggered(string strEventID)
        {
            //등록된 이벤트가 있다면 다음 state로 전이
            if (IsActive)
            {
                if (m_dicTransition.ContainsKey(strEventID))                
                {
                    // 다음 state로 전이   
                    ExitState();

                    //아래 2개의 방법중 택일

                    //1. 다음 스테이트 이동 예약 (다음 update때 이동)
                    stateToTransit = m_dicTransition[strEventID].StateToTrasit;

                    //2. 지금 바로 이동                     
                    //m_dicTransition[strEventID].transit();

                    return true;
                }
            }

            return false;
        }

        public JMState OnFinish(string stateName)
        {
            return OnEvent(JMEvent.FINISHED, stateName);
        }

        public JMState OnFinish(JMState state)
        {
            return OnEvent(JMEvent.FINISHED, state);
        }

        public JMState OnEvent(string strJMEvent, string stateName)
        {
            JMTransition transit = new (ParentFSM, stateName);
            SetTransit(strJMEvent, transit);
            return this;
        }

        public JMState OnEvent(string strJMEvent, JMState state)
        {
            JMTransition transit = new (state);
            SetTransit(strJMEvent, transit);
            return this;
        }

        protected void SetTransit(string strJMEvent, JMTransition transit)
        {
            if (m_dicTransition.ContainsKey(strJMEvent))
                m_dicTransition[strJMEvent] = transit;
            else
                m_dicTransition.Add(strJMEvent, transit);
        }


        public void RemoveTransition(string strJMEvent)
        {
            if (m_dicTransition.ContainsKey(strJMEvent))
                m_dicTransition.Remove(strJMEvent);
        }

        virtual public void StartState()
        {
            IsActive = true;
            AddObserver(m_parent);
         
            RunActions();
            OnEnter();
        }

        virtual public void ExitState()
        {
            IsActive = false;
            RemoveObserver(m_parent);

            m_actionParallelRunner.Exit();
            OnExit();
        }

        public void ClearState()
        {
            IsActive = false;
            stateToTransit = null;
        }

        protected void RunActions() => m_actionParallelRunner.Run();

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
            if (IsActive)
            {
                m_actionParallelRunner.Update();
                OnUpdate();
            }

            //다음 스테이트 이동
            if (stateToTransit != null)
            {
                JMState nextState = stateToTransit;

                stateToTransit = null;
                nextState.StartState();
            }
        }
       
        public void DestroyFSM() => m_actionParallelRunner.DestroyFSM();

    }
}
