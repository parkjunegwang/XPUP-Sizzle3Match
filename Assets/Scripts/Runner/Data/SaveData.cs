using Assets.Scripts.FrameWork.Job;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage",menuName = "XPUP/STAGE")]
public class SaveData : ScriptableObject
{
    [Header("가로")]
    public int X = 4;
    [Header("세로")]
    public int Y = 4;

    public int StageLevel = 1;
    public int StageTime = 600;

    public int StageEXP = 40;

    public int ItemKind = 4;

    public bool LockGame = false;

    public Vector2[] LockPos;

    public bool BlckGame = false;

    public bool DirtyGame = false;


}
