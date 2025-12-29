using Assets.Scripts.FrameWork.Job;
using System;
using UnityEngine;

public class StageData : IJMData
{
    public int m_StageLevel = 1;

    public int Stage_Time = 600;

    public string GetTimer()
    {
        TimeSpan t = TimeSpan.FromSeconds(Stage_Time);

        return t.ToString(@"mm\:ss"); 
    }

    public int CompleteEXP = 150;
}

