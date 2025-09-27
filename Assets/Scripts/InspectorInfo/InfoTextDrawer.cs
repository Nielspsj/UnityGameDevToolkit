#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InfoTextAttribute))]
public class InfoTextDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InfoTextAttribute info = (InfoTextAttribute)attribute;

        // Calculate height for wrapped text
        GUIStyle wrapStyle = new GUIStyle(EditorStyles.helpBox);
        wrapStyle.wordWrap = true;

        float textHeight = wrapStyle.CalcHeight(new GUIContent(info.text), position.width);

        // Draw the help box
        Rect helpBoxRect = new Rect(position.x, position.y, position.width, textHeight);
        EditorGUI.LabelField(helpBoxRect, info.text, wrapStyle);

        // Move rect down for the actual property
        Rect propertyRect = new Rect(position.x, position.y + textHeight + 2, position.width, EditorGUI.GetPropertyHeight(property, label, true));

        EditorGUI.PropertyField(propertyRect, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        InfoTextAttribute info = (InfoTextAttribute)attribute;

        GUIStyle wrapStyle = new GUIStyle(EditorStyles.helpBox);
        wrapStyle.wordWrap = true;

        float textHeight = wrapStyle.CalcHeight(new GUIContent(info.text), EditorGUIUtility.currentViewWidth);

        return textHeight + EditorGUI.GetPropertyHeight(property, label, true) + 4;
    }
}
#endif
