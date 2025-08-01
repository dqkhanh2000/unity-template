using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using KaneTemplate.Core.Attributes;

namespace KaneTemplate.Core.Editor.Editors
{
    /// <summary>
    /// Custom editor that handles KaneTemplate attributes for all MonoBehaviour components.
    /// This is only available in the Unity Editor.
    /// </summary>
    [UnityEditor.CustomEditor(typeof(MonoBehaviour), true)]
    public class CustomEditor : UnityEditor.Editor
    {
        private List<PropertyInfo> _orderedProperties = new List<PropertyInfo>();
        private List<MethodInfo> _buttonMethods = new List<MethodInfo>();
        private List<InfoBoxAttribute> _classInfoBoxes = new List<InfoBoxAttribute>();
        
        private void OnEnable()
        {
            var targetType = target.GetType();
            
            // Get all properties with PropertyOrder attribute
            var properties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.GetCustomAttribute<PropertyOrderAttribute>() != null)
                .OrderBy(p => p.GetCustomAttribute<PropertyOrderAttribute>().Order)
                .ToList();
            
            _orderedProperties = properties;
            
            // Get all methods with Button attribute
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<ButtonAttribute>() != null && m.GetParameters().Length == 0)
                .ToList();
            
            _buttonMethods = methods;
            
            // Get class-level InfoBox attributes
            var infoBoxes = targetType.GetCustomAttributes<InfoBoxAttribute>(true).ToList();
            _classInfoBoxes = infoBoxes;
        }
        
        public override void OnInspectorGUI()
        {
            // Draw class-level info boxes
            DrawClassInfoBoxes();
            
            // Draw default inspector
            DrawDefaultInspector();
            
            // Draw ordered properties
            DrawOrderedProperties();
            
            // Draw buttons
            DrawButtons();
        }
        
        #region Class Info Boxes
        
        private void DrawClassInfoBoxes()
        {
            foreach (var infoBox in _classInfoBoxes)
            {
                // Check if info box should be shown
                if (!string.IsNullOrEmpty(infoBox.ConditionField))
                {
                    bool condition = CheckCondition(target, infoBox.ConditionField, infoBox.ConditionValue);
                    if (!condition) continue;
                }
                
                Color boxColor = GetInfoBoxColor(infoBox.Type);
                var originalColor = GUI.color;
                GUI.color = boxColor;
                
                EditorGUILayout.HelpBox(infoBox.Message, GetInfoBoxMessageType(infoBox.Type));
                
                GUI.color = originalColor;
            }
        }
        
        private bool CheckCondition(UnityEngine.Object target, string fieldName, object expectedValue)
        {
            if (string.IsNullOrEmpty(fieldName)) return true;
            
            var field = GetField(target.GetType(), fieldName);
            if (field == null) return false;
            
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
                default: return MessageType.Info;
            }
        }
        
        #endregion
        
        #region Ordered Properties
        
        private void DrawOrderedProperties()
        {
            if (_orderedProperties.Count == 0) return;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Ordered Properties", EditorStyles.boldLabel);
            
            foreach (var property in _orderedProperties)
            {
                try
                {
                    var value = property.GetValue(target);
                    var newValue = DrawPropertyField(property, value);
                    
                    if (!Equals(value, newValue))
                    {
                        property.SetValue(target, newValue);
                        EditorUtility.SetDirty(target);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error drawing property {property.Name}: {e.Message}");
                }
            }
        }
        
        private object DrawPropertyField(PropertyInfo property, object value)
        {
            var propertyType = property.PropertyType;
            
            if (propertyType == typeof(string))
            {
                return EditorGUILayout.TextField(property.Name, (string)value);
            }
            else if (propertyType == typeof(int))
            {
                return EditorGUILayout.IntField(property.Name, (int)value);
            }
            else if (propertyType == typeof(float))
            {
                return EditorGUILayout.FloatField(property.Name, (float)value);
            }
            else if (propertyType == typeof(bool))
            {
                return EditorGUILayout.Toggle(property.Name, (bool)value);
            }
            else if (propertyType == typeof(Vector2))
            {
                return EditorGUILayout.Vector2Field(property.Name, (Vector2)value);
            }
            else if (propertyType == typeof(Vector3))
            {
                return EditorGUILayout.Vector3Field(property.Name, (Vector3)value);
            }
            else if (propertyType == typeof(Color))
            {
                return EditorGUILayout.ColorField(property.Name, (Color)value);
            }
            else if (propertyType.IsEnum)
            {
                return EditorGUILayout.EnumPopup(property.Name, (Enum)value);
            }
            
            return value;
        }
        
        #endregion
        
        #region Buttons
        
        private void DrawButtons()
        {
            if (_buttonMethods.Count == 0) return;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
            
            foreach (var method in _buttonMethods)
            {
                var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttr == null) continue;
                
                string buttonText = buttonAttr.ButtonText ?? method.Name;
                ButtonStyle style = buttonAttr.Style;
                
                if (GUILayout.Button(buttonText, GetButtonStyle(style)))
                {
                    try
                    {
                        method.Invoke(target, null);
                        EditorUtility.SetDirty(target);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error calling method {method.Name}: {e.Message}");
                    }
                }
            }
        }
        
        private GUIStyle GetButtonStyle(ButtonStyle style)
        {
            switch (style)
            {
                case ButtonStyle.Primary:
                    return GUI.skin.button;
                case ButtonStyle.Secondary:
                    var secondaryStyle = new GUIStyle(GUI.skin.button);
                    secondaryStyle.normal.textColor = Color.gray;
                    return secondaryStyle;
                case ButtonStyle.Success:
                    var successStyle = new GUIStyle(GUI.skin.button);
                    successStyle.normal.textColor = Color.green;
                    return successStyle;
                case ButtonStyle.Warning:
                    var warningStyle = new GUIStyle(GUI.skin.button);
                    warningStyle.normal.textColor = Color.yellow;
                    return warningStyle;
                case ButtonStyle.Danger:
                    var dangerStyle = new GUIStyle(GUI.skin.button);
                    dangerStyle.normal.textColor = Color.red;
                    return dangerStyle;
                default:
                    return GUI.skin.button;
            }
        }
        
        #endregion
        
        #region Reflection Helpers
        
        private FieldInfo GetField(Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
        
        #endregion
    }
}
