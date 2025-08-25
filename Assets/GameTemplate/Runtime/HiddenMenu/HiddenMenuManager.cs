using System.Collections;
using UnityEngine;
using GameTemplate.Runtime.Core;
using GameTemplate.Runtime.EventSystem;
using GameTemplate.Runtime.EventSystem.Implements;
using UnityEngine.Events;

namespace GameTemplate.Runtime.HiddenMenu
{
    public class HiddenMenuManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private HiddenMenuConfig config;
        [SerializeField] private GameObject hiddenPasswordUIPrefab;
        [SerializeField] private GameObject hiddenMenuUIPrefab;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = true;

        public UnityEvent<Level> OnLevelLoaded;
        
        private int clickCount = 0;
        private float lastClickTime = 0f;
        private bool isHiddenMenuOpen = false;
        private Coroutine clickResetCoroutine;
        
        public static HiddenMenuManager Instance { get; private set; }

        public bool IsHiddenMenuOpen
        {
            get => isHiddenMenuOpen;
            set => isHiddenMenuOpen = value;
        }
        public HiddenMenuConfig Config => config;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            if (config == null)
            {
                LogWarning("HiddenMenuConfig not assigned! Using default values.");
                CreateDefaultConfig();
            }
        }
        
        /// <summary>
        /// Call this method from any button to trigger hidden menu detection
        /// </summary>
        public void OnHiddenMenuTrigger()
        {
            clickCount++;
            lastClickTime = Time.time;
            
            LogDebug($"Hidden menu trigger: {clickCount}/{config.ClickThreshold}");
            
            if (clickCount >= config.ClickThreshold)
            {
                if (Time.time - lastClickTime <= config.ClickTimeWindow)
                {
                    OpenHiddenMenu();
                }
                else
                {
                    ResetClickCount();
                }
            }
            
            // Reset click count after time window
            if (clickResetCoroutine != null)
                StopCoroutine(clickResetCoroutine);
            clickResetCoroutine = StartCoroutine(ResetClickCountAfterDelay());
        }
        
        public void OpenHiddenMenu(string password = null)
        {
            if(config.UsePassword)
            {
                if (!string.IsNullOrEmpty(password) && password == config.Password)
                {
                    LogDebug("Correct password entered. Opening hidden menu.");
                    ShowHiddenMenuWithPrefab();
                }
                else
                {
                    ShowPasswordInput();
                }
            }
            else
            {
                ShowHiddenMenuWithPrefab();
            }
        }
        
        private void ShowPasswordInput()
        {
            ResetClickCount();
            if (hiddenPasswordUIPrefab != null)
            {
                Instantiate(hiddenPasswordUIPrefab);
            }
            else
            {
                LogWarning("No hidden menu UI prefab assigned! Please assign one in the inspector.");
            }
        }
        
        
        public void ShowHiddenMenuWithPrefab()
        {
            if (isHiddenMenuOpen)
            {
                LogDebug("Hidden menu is already open.");
                return;
            }
            isHiddenMenuOpen = true;
            if (hiddenMenuUIPrefab != null)
            {
                Instantiate(hiddenMenuUIPrefab);
            }
            else
            {
                LogWarning("No hidden menu UI prefab assigned! Please assign one in the inspector.");
            }
        }
        
        private void ResetClickCount()
        {
            clickCount = 0;
            lastClickTime = 0f;
        }
        
        private IEnumerator ResetClickCountAfterDelay()
        {
            yield return new WaitForSeconds(config.ClickTimeWindow);
            ResetClickCount();
        }
        
        private void CreateDefaultConfig()
        {
            config = ScriptableObject.CreateInstance<HiddenMenuConfig>();
        }
        
        private void LogDebug(string message)
        {
            if (showDebugLogs)
                Debug.Log($"[HiddenMenu] {message}");
        }
        
        private void LogWarning(string message)
        {
            Debug.LogWarning($"[HiddenMenu] {message}");
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
        
        public void ClearPlayerData()
        {
            GameManager.Instance.ClearAllData();
        }
        
        public void SetPlayerLevel(LevelData selectedLevel)
        {
            if (!LevelManager.Instance.IsLevelUnlocked(selectedLevel.levelId))
            {
                LevelManager.Instance.UnlockLevel(selectedLevel.levelId);
            }
            
            LevelManager.Instance.OnLevelStarted.AddListener(LevelLoaded);
            LevelManager.Instance.StartLevel(selectedLevel.levelId);
        }
        
        private void LevelLoaded(Level level)
        {
            LevelManager.Instance.OnLevelStarted.RemoveListener(LevelLoaded);
            LogDebug($"Level {level.LevelData.levelId} loaded via hidden menu.");
        }
    }
} 