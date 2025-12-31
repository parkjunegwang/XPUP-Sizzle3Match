using System;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "XPUP/STAGE")]
public class SaveData : ScriptableObject
{
    [Header("스테이지 레벨")]
    public int StageLevel = 1;

    [Header("가로")]
    [Min(1)]public int width = 4;
    [Header("세로")]
    [Min(1)] public int height = 4;

    [SerializeField] private int[] types;

    [Header("트레일러 게임 여부")]
    public bool trailerGame = false;

    [Header("트레일러 세로 ")]
    public int[] trailCells;
    [Header("트레일러 트레이 갯수 ")]
    public int trailTrayCount = 0;

    [Header("스테이지 시간")]
    public int StageTime = 600;
    [Header("스테이지 클리어 횟수")]
    public int StageEXP = 40;
    [Header("스테이지 오브젝트 종류")]
    public int ItemKind = 10;

    [Header("잠금 트레이가 있는가? 체크시 있음. 해제시 없음")]
    public bool LockGame = false;

    [Header("잠기는 트레이 포지션")]
    public Vector2[] LockPos;

    [Header("블라인드 게임이있는가? 체크시 있음. 해제시 없음")]
    public bool BlindGame = false;

    [Header("블라인드 몇번 나오게할것인가")]
    public int BlindCount = 0;

    [Header("더러워진 트레이 작동 여부")]

    public bool DirtyGame = false;
    [Header("클리어 터치 횟수")]
    public int DirtyGameClearCount = 5;

   

    public int Count => width * height;

    public int GetType(int x, int y)
    {
        if (!InRange(x, y)) return -1;
        EnsureSize();
        return types[y * width + x];
    }
    public void SetType(int x, int y, string value)
    {
        if (!InRange(x, y)) return;
        EnsureSize();
        types[y * width + x] = int.Parse(value);
    }
    public void ClearAll()
    {
        EnsureSize();
        Array.Fill(types, 0);
    }

    public bool InRange(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    private void OnValidate()
    {
        if (width < 1) width = 1;
        if (height < 1) height = 1;
        EnsureSize();
    }

    private void EnsureSize()
    {
        int n = width * height;
 
        if (types == null || types.Length != n)
            types = ResizeKeep(types, n);
    }


    private static T[] ResizeKeep<T>(T[] src, int newSize)
    {
        var dst = new T[newSize];
        if (src != null) Array.Copy(src, dst, Mathf.Min(src.Length, dst.Length));
        return dst;
    }

    // 체크된 좌표 목록이 필요하면 이거 쓰면 됨
    public Vector2Int[] GetCheckedPositions()
    {
        EnsureSize();
        var list = new System.Collections.Generic.List<Vector2Int>();
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (types[y * width + x] != 0) list.Add(new Vector2Int(x, y));
        return list.ToArray();
    }
}




[CustomEditor(typeof(SaveData))]
public class GridMaskDataEditor : Editor
{
    SerializedProperty widthProp;
    SerializedProperty heightProp;

//    SerializedProperty stageLevelProp;

    private void OnEnable()
    {
        widthProp = serializedObject.FindProperty("width");
        heightProp = serializedObject.FindProperty("height");

        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("StageLevel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("StageTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("StageEXP"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ItemKind"));
        EditorGUILayout.PropertyField(widthProp);
        EditorGUILayout.PropertyField(heightProp);

        var data = (SaveData)target;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Grid Check Table", EditorStyles.boldLabel);

     

        // 버튼들
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear")) { Undo.RecordObject(data, "Clear Grid"); data.ClearAll(); EditorUtility.SetDirty(data); }
        if (GUILayout.Button("Invert"))
        {
            Undo.RecordObject(data, "Invert Grid");
            for (int y = 0; y < data.height; y++)
                for (int x = 0; x < data.width; x++)
                    data.SetType(x, y, 1.ToString());
            EditorUtility.SetDirty(data);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        // 표 그리기
        DrawGrid(data);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("trailerGame"));


        if (data.trailerGame)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trailCells"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trailTrayCount"));

            //   data.trailCells = new bool[data.height];
        }



        EditorGUILayout.PropertyField(serializedObject.FindProperty("LockGame"));
        if (data.LockGame)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LockPos"));
        }
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("BlindGame"));

        if (data.BlindGame)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BlindCount"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("DirtyGame"));
        if (data.DirtyGame)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DirtyGameClearCount"));
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGrid(SaveData data)
    {
   
        EditorGUILayout.BeginVertical("box");
        GUILayout.TextArea("0. 빈공간 , 1.일반 트레이 2. 시작시 잠금 트레이 3. 머지안하는 트레이 4.돔 트레이  5. 트레일러트레이" );

        EditorGUILayout.Space(12);
        for (int y = data.height - 1; y >= 0; y--) // 위에서 아래로 보이게(원하면 0->height로 바꿔도 됨)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < data.width; x++)
            {
                int prev = data.GetType(x, y);

                // 작은 토글(체크박스)
                string next = GUILayout.TextField(prev.ToString(), GUILayout.Width(40));
                if (next != prev.ToString())
                {
                    Undo.RecordObject(data, "Toggle Cell");
                    data.SetType(x, y, next);
                    EditorUtility.SetDirty(data);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

    }
}
