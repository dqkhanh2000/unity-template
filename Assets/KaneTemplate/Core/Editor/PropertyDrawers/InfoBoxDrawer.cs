#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using KaneTemplate.Core.Attributes;

namespace KaneTemplate.Core.Editor.PropertyDrawers
{
    /// <summary>
    /// Property drawer for InfoBoxAttribute.
    /// Displays information boxes in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    public class InfoBoxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = Helpers.GetTargetObject(property);
            if (target == null) return;
            
            var infoBoxAttr = Helpers.GetAttribute<InfoBoxAttribute>(property, target);
            if (infoBoxAttr != null)
            {
                // Check if info box should be shown
                if (!string.IsNullOrEmpty(infoBoxAttr.ConditionField))
                {
                    bool condition = CheckCondition(target, infoBoxAttr.ConditionField, infoBoxAttr.ConditionValue);
                    if (!condition) return;
                }
                
                var infoBoxRect = new Rect(position.x, position.y, position.width, infoBoxAttr.ShowIcon ? 40f : 20f);
                
                Color boxColor = GetInfoBoxColor(infoBoxAttr.Type);
                var originalColor = GUI.color;
                GUI.color = boxColor;
                
                var messageType = GetInfoBoxMessageType(infoBoxAttr.Type);
                if (!infoBoxAttr.ShowIcon)
                {
                    messageType = MessageType.None;
                }
                
                EditorGUI.HelpBox(infoBoxRect, infoBoxAttr.Message, messageType);
                
                GUI.color = originalColor;
                position.y += infoBoxAttr.ShowIcon ? 40f : 20f;
            }
            
            // Draw the property
            EditorGUI.PropertyField(position, property, label, true);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var target = Helpers.GetTargetObject(property);
            if (target == null) return EditorGUI.GetPropertyHeight(property, label);
            
            float height = EditorGUI.GetPropertyHeight(property, label);
            
            // Add info box height
            height += GetInfoBoxHeight(property, target);
            
            return height;
        }
        
        private float GetInfoBoxHeight(SerializedProperty property, object target)
        {
            var infoBoxAttr = Helpers.GetAttribute<InfoBoxAttribute>(property, target);
            if (infoBoxAttr == null) return 0f;
            
            // Check if info box should be shown
            if (!string.IsNullOrEmpty(infoBoxAttr.ConditionField))
            {
                bool condition = CheckCondition(target, infoBoxAttr.ConditionField, infoBoxAttr.ConditionValue);
                if (!condition) return 0f;
            }
            
            return 40f;
        }
        
        private bool CheckCondition(object target, string fieldName, object expectedValue)
        {
            if (string.IsNullOrEmpty(fieldName)) return true;
            
            var field = Helpers.GetField(target.GetType(), fieldName);
            if (field == null) 
            {
                Debug.LogWarning($"[InfoBoxDrawer] Field {fieldName} not found on {target.GetType().Name}");
                return false;
            }
            
            var value = field.GetValue(target);
            
            if (expectedValue == null)
                return value != null && (bool)value;
            
            return Equals(value, expectedValue);
        }
        
        private Color GetInfoBoxColor(InfoBoxType type)
        {
            switch (type)
            {
                case InfoBoxType.Warning: return Color.yellow;
                case InfoBoxType.Error: return Color.red;
                case InfoBoxType.Success: return Color.green;
                case InfoBoxType.Info: return Color.cyan;
                default: return Color.white;
            }
        }
        
        private MessageType GetInfoBoxMessageType(InfoBoxType type)
        {
            switch (type)
            {
                case InfoBoxType.Warning: return MessageType.Warning;
                case InfoBoxType.Error: return MessageType.Error;
                case InfoBoxType.Success: return MessageType.Info;
                case InfoBoxType.Default:
                default: return MessageType.Info;
            }
        }
    }
}
#endif 