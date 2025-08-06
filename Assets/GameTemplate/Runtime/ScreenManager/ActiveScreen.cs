using UnityEngine;

namespace GameTemplate.Runtime.ScreenManager
{
    public class ActiveScreen : MonoBehaviour
    {
        [SerializeField] private ScreenManager screenManager;
        [SerializeField] private string key;

        public bool activeOnStart;
        public bool saveHistory = true;

        private void Start()
        {
            if (activeOnStart)
            {
                Active();
            }
        }

        public void Active()
        {
            screenManager.DeactiveAllScreen();
            screenManager.ActiveScreen(key ,saveHistory);
        }
    }
}