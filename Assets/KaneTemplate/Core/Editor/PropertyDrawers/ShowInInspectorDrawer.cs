using UnityEngine;
using UnityEditor;
using KaneTemplate.Core.Attributes;

namespace KaneTemplate.Core.Editor.PropertyDrawers
{
    /// <summary>
    /// Property drawer for ShowInInspectorAttribute.
    /// Forces private fields to be shown in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowInInspectorAttribute))]
    public class ShowInInspectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
