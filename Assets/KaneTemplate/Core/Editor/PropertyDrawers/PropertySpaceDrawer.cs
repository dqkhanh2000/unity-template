#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using KaneTemplate.Core.Attributes;

namespace KaneTemplate.Core.Editor.PropertyDrawers
{
    /// <summary>
    /// Property drawer for PropertySpaceAttribute.
    /// Adds spacing in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(PropertySpaceAttribute))]
    public class PropertySpaceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = Helpers.GetTargetObject(property);
            if (target == null) return;
            
            var spaceAttr = Helpers.GetAttribute<PropertySpaceAttribute>(property, target);
            if (spaceAttr != null)
            {
                position.y += spaceAttr.Space;
            }
            
            // Draw the property
            EditorGUI.PropertyField(position, property, label, true);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var target = property.serializedObject.targetObject;
            if (target == null) return EditorGUI.GetPropertyHeight(property, label);
            
            float height = EditorGUI.GetPropertyHeight(property, label);
            
            // Add spacing height
            var spaceAttr = Helpers.GetAttribute<PropertySpaceAttribute>(property, target);
            height += spaceAttr?.Space ?? 0f;
            
            return height;
        }
    }
}
#endif 