using System;
using Assets.Scripts.FrameWork.Job;

public class DelegateAction : JMAction
{
    readonly Action m_action;

    public DelegateAction(Action action) => m_action = action;

    protected override void OnEnter()
    {
        m_action?.Invoke();

        Finish();
    }
}