using Assets.Scripts.FrameWork.Job;
using UnityEngine;

public class JobMakerBehaviour : MonoBehaviour
{
    private void Awake() => DontDestroyOnLoad(this);

    private void FixedUpdate() => JobMaker.Update();
}
