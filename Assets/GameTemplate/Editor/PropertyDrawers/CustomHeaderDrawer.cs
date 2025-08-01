using GameTemplate.Runtime.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace GameTemplate.Editor.PropertyDrawers
{
    /// <summary>
    /// Property drawer for CustomHeaderAttribute.
    /// Adds custom headers above fields in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomHeaderAttribute))]
    public class CustomHeaderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = Helpers.GetTargetObject(property);
            if (target == null) return;

            var headerAttr = Helpers.GetAttribute<CustomHeaderAttribute>(property, target);
            if (headerAttr != null)
            {

                // Only draw if not already drawn
                var headerRect = new Rect(position.x, position.y, position.width, 20f);

                switch (headerAttr.Style)
                {
                    case HeaderStyle.Bold:
                        EditorGUI.LabelField(headerRect, headerAttr.HeaderText, EditorStyles.boldLabel);
                        break;
                    case HeaderStyle.Colored:
                        var originalColor = GUI.color;
                        GUI.color = Color.cyan;
                        EditorGUI.LabelField(headerRect, headerAttr.HeaderText, EditorStyles.boldLabel);
                        GUI.color = originalColor;
                        break;
                    case HeaderStyle.Separator:
                        EditorGUI.LabelField(headerRect, headerAttr.HeaderText);
                        var separatorRect = new Rect(position.x, position.y + 18f, position.width, 1f);
                        EditorGUI.DrawRect(separatorRect, Color.gray);
                        break;
                    default:
                        EditorGUI.LabelField(headerRect, headerAttr.HeaderText);
                        break;
                }

                // Move position down for the property
                position.y += 20f;
                position.height -= 20f;
            }

            // Draw the property in the adjusted position
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var target = property.serializedObject.targetObject;
            if (target == null) return EditorGUI.GetPropertyHeight(property, label);

            float height = EditorGUI.GetPropertyHeight(property, label);

            // Add header height
            height += GetHeaderHeight(property, target);

            return height;
        }

        private float GetHeaderHeight(SerializedProperty property, UnityEngine.Object target)
        {
            var headerAttr = Helpers.GetAttribute<CustomHeaderAttribute>(property, target);
            if (headerAttr == null) return 0f;

            // Create a unique key for this header
            string headerKey = $"{target.GetInstanceID()}_{property.name}_{headerAttr.HeaderText}";

            // Always add height for header (the duplication check is in OnGUI)
            return 20f;
        }
    }
}
