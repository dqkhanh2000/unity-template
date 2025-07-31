#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using KaneTemplate.Core.Attributes;

namespace KaneTemplate.Core.Editor.PropertyDrawers
{
    /// <summary>
    /// Property drawer for ShowIfAttribute and HideIfAttribute.
    /// Shows or hides fields based on conditions.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class ConditionalDisplayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = Helpers.GetTargetObject(property);
            if (target == null) return;
            
            // Check if we should show this property
            if (!ShouldShowProperty(property, target))
            {
                return;
            }
            
            EditorGUI.PropertyField(position, property, label, true);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var target = Helpers.GetTargetObject(property);
            if (target == null) return 0f;
            
            // Check if we should show this property
            if (!ShouldShowProperty(property, target))
                return 0f;
            
            return EditorGUI.GetPropertyHeight(property, label);
        }
        
        
        
        
        
        
        
        private bool ShouldShowProperty(SerializedProperty property, object target)
        {
            // Check ShowIf
            var showIf = Helpers.GetAttribute<ShowIfAttribute>(property, target);
            if (showIf != null)
            {
                bool condition = CheckCondition(target, showIf.ConditionField, showIf.ConditionValue);
                if (showIf.Invert) condition = !condition;
                if (!condition) return false;
            }
            
            // Check HideIf
            var hideIf = Helpers.GetAttribute<HideIfAttribute>(property, target);
            if (hideIf != null)
            {
                bool condition = CheckCondition(target, hideIf.ConditionField, hideIf.ConditionValue);
                if (hideIf.Invert) condition = !condition;
                if (condition) return false;
            }
            
            return true;
        }
        
        private bool CheckCondition(object target, string fieldName, object expectedValue)
        {
            if (string.IsNullOrEmpty(fieldName)) return true;
            
            var field = Helpers.GetField(target.GetType(), fieldName);
            if (field == null) 
            {
                return false;
            }
            
            var value = field.GetValue(target);
            
            if (expectedValue == null)
                return value != null && (bool)value;
            
            return Equals(value, expectedValue);
        }
        
        
    }
}
#endif 