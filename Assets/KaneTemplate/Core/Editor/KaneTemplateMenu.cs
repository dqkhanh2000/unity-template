using UnityEngine;
using UnityEditor;
using System.IO;

namespace KaneTemplate.Core.Editor
{
    public static class KaneTemplateMenu
    {
        private const string MENU_PATH = "Tools/Kane Template/";
        private const string DELETE_EXAMPLE_MENU = MENU_PATH + "Delete Example Folder";
        private const string EXAMPLE_FOLDER_PATH = "Assets/KaneTemplate/Example";

        [MenuItem(DELETE_EXAMPLE_MENU)]
        public static void DeleteExampleFolder()
        {
            if (!Directory.Exists(EXAMPLE_FOLDER_PATH))
            {
                EditorUtility.DisplayDialog("Kane Template", "Example folder does not exist!", "OK");
                return;
            }

            bool confirmed = EditorUtility.DisplayDialog(
                "Delete Example Folder", 
                "Are you sure you want to delete the Example folder?\n\nThis action cannot be undone.", 
                "Delete", 
                "Cancel"
            );

            if (confirmed)
            {
                try
                {
                    // Delete the folder and all its contents
                    AssetDatabase.DeleteAsset(EXAMPLE_FOLDER_PATH);
                    
                    // Refresh the asset database
                    AssetDatabase.Refresh();
                    
                    Debug.Log("Kane Template: Example folder deleted successfully!");
                    
                    EditorUtility.DisplayDialog("Kane Template", "Example folder deleted successfully!", "OK");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Kane Template: Failed to delete Example folder: {e.Message}");
                    EditorUtility.DisplayDialog("Kane Template", $"Failed to delete Example folder: {e.Message}", "OK");
                }
            }
        }

        [MenuItem(DELETE_EXAMPLE_MENU, true)]
        public static bool ValidateDeleteExampleFolder()
        {
            return Directory.Exists(EXAMPLE_FOLDER_PATH);
        }

        [MenuItem(MENU_PATH + "Documentation/Core README")]
        public static void OpenCoreREADME()
        {
            string readmePath = "Assets/KaneTemplate/Core/README.md";
            if (File.Exists(readmePath))
            {
                Object readme = AssetDatabase.LoadAssetAtPath<Object>(readmePath);
                if (readme != null)
                {
                    Selection.activeObject = readme;
                    EditorGUIUtility.PingObject(readme);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Kane Template", "Core README not found!", "OK");
            }
        }

        [MenuItem(MENU_PATH + "Documentation/EventSystem README")]
        public static void OpenEventSystemReadme()
        {
            string readmePath = "Assets/KaneTemplate/EventSystem/README.md";
            if (File.Exists(readmePath))
            {
                Object readme = AssetDatabase.LoadAssetAtPath<Object>(readmePath);
                if (readme != null)
                {
                    Selection.activeObject = readme;
                    EditorGUIUtility.PingObject(readme);
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Kane Template", "EventSystem README not found!", "OK");
            }
        }

        [MenuItem(MENU_PATH + "About Kane Template")]
        public static void ShowAboutDialog()
        {
            EditorUtility.DisplayDialog(
                "Kane Template", 
                "Kane Template v1.0.0\n\nA comprehensive Unity game development template with core systems and event management.\n\nCreated by Kane", 
                "OK"
            );
        }
    }
} 