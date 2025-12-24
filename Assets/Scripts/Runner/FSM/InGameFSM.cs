using Assets.Scripts.FrameWork.Job;
using System;
using UnityEngine.SceneManagement;

public class InGameFSM : JMFSM
{
    public InGameFSM(JMDataBox dataBox) : base(FSMDefine.FSM_GAME_SCENE) => Build();

    protected virtual void Build()
    {
        JMState INGAME_TRIGGER_CHECK = CreateState("GAME_TRIGGER_CHECK", true),
                INGAME_START = CreateState("INGAM_START"),
                INGAME_BURST = CreateState("INGAME_BURST"),
                INGAME_DROP = CreateState("INGAME_DROP"),
                INGAME_BOSSATTACK = CreateState("INGAME_BOSSATTACK"),
                INGAME_REFILLBUBBLE = CreateState("INGAME_REFILLBUBBLE"),
                INGAME_GAMEOVER = CreateState("INGAME_GAMEOVER"),
                INGAME_CLEAR = CreateState("INGAME_CLEAR"),
                INGAME_SHOW_SCENE_LOAD = CreateState("INGAME_SHOW_SCENE_LOAD");


        INGAME_TRIGGER_CHECK.OnEvent(EventDefine.SHOW_SCENE_LOADING, INGAME_SHOW_SCENE_LOAD)
                           .OnEvent(EventDefine.INGAME_START, INGAME_START)
                           .OnEvent(EventDefine.INGAME_BURST, INGAME_BURST)
                           .OnEvent(EventDefine.INGAME_DROP, INGAME_DROP)
                           .OnEvent(EventDefine.INGAME_BOSSATTACK, INGAME_BOSSATTACK)
                           .OnEvent(EventDefine.INGAME_REFILLBUBBLE, INGAME_REFILLBUBBLE)
                           .OnEvent(EventDefine.INGAME_GAMEOVER, INGAME_GAMEOVER)
                           .OnEvent(EventDefine.INGAME_CLEAR, INGAME_CLEAR);
 
        //INGAME_START.AddAction<DelegateAction>(new Action(InGameSceneHandler.I.GameStart)).OnFinish(INGAME_TRIGGER_CHECK);
        //INGAME_BURST.AddAction<DelegateAction>(new Action(InGameSceneHandler.I.PlayClusterBubble)).OnFinish(INGAME_TRIGGER_CHECK);
        //INGAME_DROP.AddAction<DelegateAction>(new Action(InGameSceneHandler.I.DropBubble)).OnFinish(INGAME_TRIGGER_CHECK);
        //INGAME_REFILLBUBBLE.AddAction<DelegateAction>(new Action(InGameSceneHandler.I.PlayRefill)).OnFinish(INGAME_TRIGGER_CHECK);

        //INGAME_GAMEOVER.AddAction<DelegateAction>(new Action(InGameUIHandler.I.OpenGameEndPopup)).OnFinish(INGAME_TRIGGER_CHECK);
        //INGAME_CLEAR.AddAction<DelegateAction>(new Action(InGameUIHandler.I.OpenGameEndPopup)).OnFinish(INGAME_TRIGGER_CHECK);


        INGAME_SHOW_SCENE_LOAD.AddAction<DelegateAction>(new Action(() => SceneManager.LoadSceneAsync(SceneDefine.LOBBY_SCENE_NAME)))
              .OnFinish(EndFSM);


    }
}
