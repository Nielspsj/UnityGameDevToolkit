#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;

// This editor only applies to classes marked with [InfoHeaderClass]
[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
public class InfoHeaderClassEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var targetType = target.GetType();

        // Look for the attribute
        var infoAttr = (InfoHeaderClassAttribute)Attribute.GetCustomAttribute(
            targetType, typeof(InfoHeaderClassAttribute));

        if (infoAttr != null)
        {
            EditorGUILayout.HelpBox(infoAttr.message, MessageType.Info);
        }

        // Draw the rest of the inspector normally
        DrawDefaultInspector();
    }
}
#endif
