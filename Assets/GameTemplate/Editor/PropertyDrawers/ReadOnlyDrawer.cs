using GameTemplate.Runtime.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace GameTemplate.Editor.PropertyDrawers
{
    /// <summary>
    /// Property drawer for ReadOnlyAttribute.
    /// Makes fields read-only in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            // Disable GUI
            GUI.enabled = false;
            
            // Draw the property
            EditorGUI.PropertyField(position, property, label, true);
            
            // Re-enable GUI
            GUI.enabled = true;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
