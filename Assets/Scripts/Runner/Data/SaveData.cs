using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "XPUP/STAGE")]
public class SaveData : ScriptableObject
{
    [Header("스테이지 레벨")]
    public int StageLevel = 1;

    [Header("가로")]
    public int X = 4;
    [Header("세로")]
    public int Y = 4;

    [Header("스테이지 시간")]
    public int StageTime = 600;
    [Header("스테이지 클리어 횟수")]
    public int StageEXP = 40;
    [Header("스테이지 오브젝트 종류")]
    public int ItemKind = 10;

    [Header("잠금 트레이가 있는가? 체크시 있음. 해제시 없음")]
    public bool LockGame = false;


    [ShowIf(nameof(LockGame), false, "잠기는 트레이 포지션")]
    public Vector2[] LockPos;

    [Header("블라인드 게임이있는가? 체크시 있음. 해제시 없음")]
    public bool BlindGame = false;

    [ShowIf(nameof(BlindGame), false, "블라인드 몇번 나오게할것인가")]
    public int BlindCount = 0;

    [Header("더러워진 트레이 작동 여부")]
    public bool DirtyGame = false;

    [ShowIf(nameof(DirtyGame), false, "클리어 터치 횟수")]
    public int DirtyGameClearCount = 5;


}




[AttributeUsage(AttributeTargets.Field)]
public class ShowIfAttribute : PropertyAttribute
{
    public readonly string conditionField;
    public readonly bool invert;
    public readonly string header; // 조건부 헤더(옵션)

    public ShowIfAttribute(string conditionField, bool invert = false, string header = null)
    {
        this.conditionField = conditionField;
        this.invert = invert;
        this.header = header;
    }
}


[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
            return;

        var showIf = (ShowIfAttribute)attribute;

        // 조건부 헤더 출력
        if (!string.IsNullOrEmpty(showIf.header))
        {
            var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(headerRect, showIf.header, EditorStyles.boldLabel);

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
            return 0f;

        var showIf = (ShowIfAttribute)attribute;

        float h = EditorGUI.GetPropertyHeight(property, label, true);

        if (!string.IsNullOrEmpty(showIf.header))
            h += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        return h;
    }

    private bool ShouldShow(SerializedProperty property)
    {
        var showIf = (ShowIfAttribute)attribute;

        SerializedProperty cond = FindRelativeProperty(property, showIf.conditionField);

        if (cond == null)
            return true;

        if (cond.propertyType != SerializedPropertyType.Boolean)
            return true;

        bool v = cond.boolValue;
        if (showIf.invert) v = !v;
        return v;
    }

    private SerializedProperty FindRelativeProperty(SerializedProperty property, string conditionField)
    {
        var direct = property.serializedObject.FindProperty(conditionField);
        if (direct != null) return direct;

        string path = property.propertyPath;
        int lastDot = path.LastIndexOf('.');
        if (lastDot >= 0)
        {
            string parentPath = path.Substring(0, lastDot);
            string condPath = parentPath + "." + conditionField;

            var relative = property.serializedObject.FindProperty(condPath);
            if (relative != null) return relative;
        }

        return null;
    }
}
