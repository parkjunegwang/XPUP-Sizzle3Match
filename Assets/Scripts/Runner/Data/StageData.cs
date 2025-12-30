using Assets.Scripts.FrameWork.Job;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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

    public SaveData Data;

    private List<IngredientType> StageItemList = new List<IngredientType>();

    public void insertStageItem()
    {
        int count = 3 * Data.StageEXP;

        int kind = 0;
        int kindCount = 0;

        for (int i = 0; i < count; ++i)
        {
            StageItemList.Add((IngredientType)kind);
            kindCount += 1;

            if (kindCount == 3)
            {
                kind += 1;
                kindCount = 0;
                if (kind == Data.ItemKind)
                {
                    kind = 0;
                }
            }
        }

        for (int i = StageItemList.Count - 1; i > 0; i--)
        {
            int rand = UnityEngine.Random.Range(0, i + 1);
            (StageItemList[i], StageItemList[rand]) = (StageItemList[rand], StageItemList[i]);
        }
    }
    private int itemcount = -1;
    public IngredientType GetNextItemData()
    {
        itemcount+= 1;
        if (itemcount >= StageItemList.Count)
        {
            return IngredientType.None;
        }

        return StageItemList[itemcount];
    }

    public int GetRemainTrayCount()
    {
        int count = StageItemList.Count - itemcount;

        return count % 2 == 0 ? count / 2 : (count / 2) + 1;
    }
}

