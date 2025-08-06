using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameTemplate.Runtime.ScreenManager
{
    public class ScreenManager : MonoBehaviour
    {
        [SerializeField] private ScreenConfig screenConfig;
        [SerializeField] private List<ScreenController> screenControllerList = new();
        private Stack<string> historyStack = new();
        public ScreenConfig ScreenConfig => screenConfig;

        public void ActiveScreen(string key, bool saveHistory = true)
        {
            var screenController = FindScreen(key);
            if (screenController)
            {
                screenController.Active();
                if (saveHistory)
                {
                    if (historyStack.Count > 0)
                    {
                        var latestScreenKey = historyStack.Peek();
                        if (!latestScreenKey.Equals(key))
                        {
                            historyStack.Push(key);
                        }
                    }
                    else
                    {
                        historyStack.Push(key);
                    }
                }
            }
        }

        public ScreenController FindScreen(string key)
        {
            var checkList = screenControllerList.FindAll(s => s.Key.Equals(key));
            return checkList.Count > 0 ? checkList[0] : null;
        }

        public void DeactiveAllScreen()
        {
            foreach (var screenController in screenControllerList)
            {
                screenController.Deactive();
            }
        }

        public void BackToPreviousScreen()
        {
            DeactiveAllScreen();
            if (historyStack.Count == 0)
            {
                return;
            }
            historyStack.Pop(); // pop current screen

            if (historyStack.Count == 0)
            {
                return;
            }
            var key = historyStack.Peek(); // get previous screen
            ActiveScreen(key, false);
        }

        public void ReloadCurrentScreen()
        {
            var currentActiveScreen = screenControllerList.Find(screen => screen.IsActive);
            if (currentActiveScreen != null)
            {
                return;
            }

            ActiveScreen(currentActiveScreen.Key, false);
        }
    }
}