using Assets.Scripts.FrameWork.Job;
using System;
using UnityEngine.SceneManagement;

public class InGameFSM : JMFSM
{
    public InGameFSM(JMDataBox dataBox) : base(FSMDefine.FSM_GAME_SCENE) => Build();

    protected virtual void Build()
    {
        JMState INGAME_TRIGGER_CHECK = CreateState("GAME_TRIGGER_CHECK", true),
                INGAME_SHOW_SCENE_LOAD = CreateState("INGAME_SHOW_SCENE_LOAD");


        INGAME_TRIGGER_CHECK.OnEvent(EventDefine.SHOW_SCENE_LOADING, INGAME_SHOW_SCENE_LOAD);
               

        INGAME_SHOW_SCENE_LOAD.AddAction<DelegateAction>(new Action(() => SceneManager.LoadSceneAsync(SceneDefine.LOBBY_SCENE_NAME)))
              .OnFinish(EndFSM);


    }
}
