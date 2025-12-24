using System.Collections.Generic;
using UnityEngine;

public enum eBubbleColorType
{
    Red,
    Green,
    Blue,
    Yellow,
    Wall,
    PreVIew,
    // 필요하면 추가
}


public enum BubbleKind
{
    Normal,
    Fairy // 요정 구슬
}

[System.Serializable]
public class BubblePath
{
    public List<Vector3Int> cells;
}

