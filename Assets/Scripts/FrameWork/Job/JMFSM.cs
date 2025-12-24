using System;
using System.Collections.Generic;

namespace Assets.Scripts.FrameWork.Job
{

    public class JMFSM : JMAction
    {
        private readonly List<JMState> m_lstStates = new();
        private JMState m_startState;
        private JMState m_finishState;

        private bool m_autoDestory;        //startFSM()으로 생성되었을때 endFSM일경우 자동삭제
        private bool m_dontDestoryOnLoad;  //씬전환시 destroy안될 FSM 

        public JMState EndFSM => m_finishState;
        public JMState StateToFinish => m_finishState;
        public JMState StateToStart => m_startState;
        public bool AutoDestroy => m_autoDestory;

        public bool DontDestroyOnLoad
        {
            set { m_dontDestoryOnLoad = value; }
            get { return m_dontDestoryOnLoad; }
        }


        public JMFSM() => InitFSM();

        public JMFSM(string name)
        {
            Name = name;

            InitFSM();
        }

        public class JMFSMStartState : JMState
        {
            public JMFSMStartState() => Name = "STARTSTATE";

            protected override void OnEnter() => TriggerEvent(JMEvent.FSM_START);
        }

        public class JMFSMFinishState : JMState
        {
            public JMFSMFinishState() => Name = "FINISHSTATE";

            protected override void OnEnter() => DispatchEvent(JMEvent.FSM_FINISHED);
        }

        public void InitFSM()
        {
            m_startState = new JMFSMStartState();
            m_finishState = new JMFSMFinishState();
            m_finishState.AddEventListener(JMEvent.FSM_FINISHED, (arg) =>
            {                
                Exit();

                base.TriggerEvent(JMEvent.FINISHED);

                //자동파괴
                if (m_autoDestory)
                    DestroyFSM();
            });

            AddState(m_startState).AddState(m_finishState);

            JobMaker.RegisterFSM(this); //등록
        }


        [Obsolete("JMEntity 의 DataBox 를 사용하세요", false)]
        public JMFSM SetDataBox(JMDataBox dataBox)
        {
            m_dataBox = dataBox;

            return this;
        }

        public JMState CreateState(string name = "", bool bEntryState = false)
        {
            JMState state = new (name);
            AddState(state, bEntryState);

            return state;
        }

        public JMFSM AddState(JMState state, bool isEnteryState = false)
        {
            if (m_lstStates.Contains(state) == false)
            {
                state.SetParent(this);
                m_lstStates.Add(state);

                if (isEnteryState)
                    m_startState.OnEvent(JMEvent.FSM_START, state);
            }

            return this;
        }
        public void RemoveState(JMState state)
        {
            if (m_lstStates.Contains(state))
            {
                state.SetParent(null);
                m_lstStates.Remove(state);
            }

        }

        override public void Run()
        {
            for (int i = 0; i < m_lstStates.Count; ++i)
                m_lstStates[i].ClearState();

            Reset();

            IsActive = true;

            m_startState.StartState();

            OnEnter();
        }

        // bAutuDestory 가 true 일경우 FSM 이 끝나면(endFSM) 자동으로 Destroy된다.
        public void StartFSM()
        {
            m_autoDestory = true;

            Run();
        }

        public void StopFSM()
        {
            if (IsActive == true)
            {

                foreach (var s in m_lstStates)
                {
                    if (s.IsActive)
                        s.ExitState();
                }

                Exit();
            }

        }

        public override void DestroyFSM()
        {
            StopFSM();

            foreach (var s in m_lstStates)
                s.DestroyFSM();

            m_lstStates.Clear();

            JobMaker.RemoveFSM(this);

        }

        public override void Update()
        {
            if (IsActive)
            {
                base.Update();

                for (int i = 0; i < m_lstStates.Count; ++i)
                    m_lstStates[i].Update();
            }
        }

        public JMState GetState(string name)
        {
            foreach (var s in m_lstStates)
            {
                if (s.Name.Equals(name))
                    return s;
            }

            return null;
        }

        internal bool TriggerGlobalEvent(string eventName)
        {

            foreach (var s in m_lstStates)
            {
                if (s.IsActive)
                {
                    if (s.TriggerEvent(eventName))
                        return true;
                }
            }

            return false;
        }

        //finish event는 FSM 윗 단계로 올라가지 않게한다.
        public override bool TriggerEvent(string strEventID)
        {
            if (strEventID != JMEvent.FINISHED)
                return base.TriggerEvent(strEventID);

            return true;
        }
    }
}