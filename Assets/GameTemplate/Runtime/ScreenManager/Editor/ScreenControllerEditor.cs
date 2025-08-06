using System.Linq;
using GameTemplate.Runtime.ScreenManager;
using UnityEditor;
using UnityEngine;

namespace GameTemplate.Runtime.ScreenManager.Editor
{
    [CustomEditor(typeof(ScreenController))]
    public class ScreenControllerEditor : UnityEditor.Editor
    {
        private SerializedProperty key;
        private SerializedProperty screenManager;
        private SerializedProperty Activated;
        private SerializedProperty Deactivated;
        private int selectedKeyIndex;

        private void OnEnable()
        {
            key = serializedObject.FindProperty("key");
            screenManager = serializedObject.FindProperty("screenManager");
            Activated = serializedObject.FindProperty("Activated");
            Deactivated = serializedObject.FindProperty("Deactivated");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(screenManager, new GUIContent("Screen Manager"));
            DisplayKeyPopup();
            EditorGUILayout.PropertyField(Activated, new GUIContent("On Active"));
            EditorGUILayout.PropertyField(Deactivated, new GUIContent("On Deactive"));
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayKeyPopup()
        {
            var handler = screenManager.objectReferenceValue as Runtime.ScreenManager.ScreenManager;
            if (!handler)
            {
                return;
            }
            var options = handler.ScreenConfig.ScreenKeyList.Select(key => key).ToList();
            selectedKeyIndex = options.IndexOf(key.stringValue);
            if(selectedKeyIndex == -1)
            {
                selectedKeyIndex = 0;
            }
            selectedKeyIndex = EditorGUILayout.Popup("Key", selectedKeyIndex, options.ToArray());
            key.stringValue = options[selectedKeyIndex];
        }
    }
}