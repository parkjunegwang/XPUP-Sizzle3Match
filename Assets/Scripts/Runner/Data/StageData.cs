using Assets.Scripts.FrameWork.Job;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/StageData")]
public class StageData : ScriptableObject
{
    public int m_StageLevel = 0;

    public bool m_isBossStage = false;

    public int m_iHP = 10;

    public int m_ShootCount = 10;

    public List<BubblePath> paths = new List<BubblePath>();
}

