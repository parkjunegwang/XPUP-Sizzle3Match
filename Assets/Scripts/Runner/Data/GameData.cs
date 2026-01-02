using Assets.Scripts.FrameWork.Job;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class GameData : IJMData
{
    public float BGM { get; set; } = PlayerPrefs.GetFloat("BGM", 0.2f);

    public float SFX { get; set; } = PlayerPrefs.GetFloat("SFX", 0.2f);

    public string GameVersion = "0.0.0";

   
}