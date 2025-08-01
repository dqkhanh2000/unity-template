using GameTemplate.Runtime.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace GameTemplate.Editor.PropertyDrawers
{
    /// <summary>
    /// Property drawer for PropertyRangeAttribute.
    /// Adds range sliders to numeric fields.
    /// </summary>
    [CustomPropertyDrawer(typeof(PropertyRangeAttribute))]
    public class PropertyRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = Helpers.GetTargetObject(property);
            if (target == null) return;
            
            var rangeAttr = Helpers.GetAttribute<PropertyRangeAttribute>(property, target);
            if (rangeAttr != null)
            {
                
                if (property.propertyType == SerializedPropertyType.Float)
                {
                    float min = rangeAttr.Min;
                    float max = rangeAttr.Max;
                    
                    if (!string.IsNullOrEmpty(rangeAttr.MinField))
                    {
                        var minField = Helpers.GetField(target.GetType(), rangeAttr.MinField);
                        if (minField != null) min = (float)minField.GetValue(target);
                    }
                    
                    if (!string.IsNullOrEmpty(rangeAttr.MaxField))
                    {
                        var maxField = Helpers.GetField(target.GetType(), rangeAttr.MaxField);
                        if (maxField != null) max = (float)maxField.GetValue(target);
                    }
                    
                    property.floatValue = Mathf.Clamp(property.floatValue, min, max);
                }
                else if (property.propertyType == SerializedPropertyType.Integer)
                {
                    int min = (int)rangeAttr.Min;
                    int max = (int)rangeAttr.Max;
                    
                    if (!string.IsNullOrEmpty(rangeAttr.MinField))
                    {
                        var minField = Helpers.GetField(target.GetType(), rangeAttr.MinField);
                        if (minField != null) min = (int)minField.GetValue(target);
                    }
                    
                    if (!string.IsNullOrEmpty(rangeAttr.MaxField))
                    {
                        var maxField = Helpers.GetField(target.GetType(), rangeAttr.MaxField);
                        if (maxField != null) max = (int)maxField.GetValue(target);
                    }
                    
                    property.intValue = Mathf.Clamp(property.intValue, min, max);
                }
            }
            
            // Draw the property
            EditorGUI.PropertyField(position, property, label, true);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
 