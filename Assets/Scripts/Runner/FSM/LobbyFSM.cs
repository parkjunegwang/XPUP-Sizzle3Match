using Assets.Scripts.FrameWork.Job;
using System;
using UnityEngine.SceneManagement;


public class LobbyFSM : JMFSM
{
    public LobbyFSM(JMDataBox dataBox) : base(FSMDefine.FSM_LOBBY_SCENE) => Build();

    protected virtual void Build()
    {
        JMState LOBBY_TRIGGER_CHECK = CreateState("LOBBY_TRIGGER_CHECK", true),
                LOBBY_SHOW_SCENE_LOAD = CreateState("LOBBY_SHOW_SCENE_LOAD");


        LOBBY_TRIGGER_CHECK.OnEvent(EventDefine.SHOW_SCENE_LOADING, LOBBY_SHOW_SCENE_LOAD);

        LOBBY_SHOW_SCENE_LOAD.AddAction<DelegateAction>(new Action(()=>SceneManager.LoadSceneAsync(SceneDefine.GAME_SCENE_NAME)))
              .OnFinish(EndFSM);

    
    }
}
