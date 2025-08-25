using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameTemplate.Runtime.Core
{
    /// <summary>
    /// Singleton level manager that handles level progression, unlocking, and reload functionality.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        // Singleton instance
        public static LevelManager Instance { get; private set; }

        [Header("Level Management")] [SerializeField]
        private Transform levelContainer;

        [SerializeField] private bool autoStartFirstLevel = true;
        [SerializeField] private float levelTransitionDelay = 0f;
        [SerializeField] private LevelDataCollection levelDataCollection;
        [SerializeField] private bool isLoopingLevels = false;
        [SerializeField] private LevelLoader levelLoader;

        // State
        private bool _isLoading = false;
        private bool _isTransitioning = false;

        // Events
        
        public UnityEvent OnLevelManagerInitializing;
        public UnityEvent OnLevelManagerInitialized;
        public UnityEvent<Level> OnLevelStarted;
        public UnityEvent<Level> OnLevelCompleted;
        public UnityEvent<Level> OnLevelFailed;
        public UnityEvent<Level> OnLevelReloaded;
        public UnityEvent<int> OnLevelUnlocked;

        public Level CurrentLevel { get; set; }
        public LevelData CurrentLevelData { get; set; }
        public bool IsLoading => _isLoading;
        public bool IsTransitioning => _isTransitioning;
        public bool HasCurrentLevel => CurrentLevel != null;
        
        public LevelDataCollection LevelDataCollection => levelDataCollection;

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                // Clean up event subscriptions
                if (CurrentLevel != null)
                {
                    UnsubscribeFromLevelEvents(CurrentLevel);
                }
            }
        }

        /// <summary>
        /// Initializes the level manager.
        /// </summary>
        private void Initialize()
        {
            if (levelContainer == null)
                levelContainer = transform;

            if (levelDataCollection == null)
            {
                Debug.LogError("LevelDataCollection is not assigned in LevelManager!");
                return;
            }
            
            if (levelLoader == null)
            {
                Debug.LogError("LevelLoader is not assigned in LevelManager!");
                return;
            }
            
            if(autoStartFirstLevel)
            {
                StartLevelManager();
                OnLevelManagerInitialized?.Invoke();
            }
            else
            {
                OnLevelManagerInitializing?.Invoke();
            }

            Debug.Log("LevelManager initialized");
        }

        /// <summary>
        /// Starts the level manager and loads the first level or the level from player data.
        /// </summary>
        public void StartLevelManager()
        {
            if (_isLoading)
            {
                Debug.LogWarning("LevelManager is already loading!");
                return;
            }

            StartCoroutine(StartLevelManagerCoroutine());
        }

        /// <summary>
        /// Starts a specific level by ID if it's unlocked.
        /// </summary>
        /// <param name="levelId">The level ID to start</param>
        /// <returns>True if the level was started, false if it's locked</returns>
        public bool StartLevel(int levelId)
        {
            if (_isLoading || _isTransitioning)
            {
                Debug.LogWarning("Cannot start level while loading or transitioning!");
                return false;
            }

            // Check if level is unlocked
            if (GameManager.Instance?.PlayerData != null)
            {
                if (!GameManager.Instance.PlayerData.IsLevelUnlocked(levelId))
                {
                    Debug.LogWarning($"Level {levelId} is not unlocked yet!");
                    return false;
                }
            }

            CurrentLevelData = levelDataCollection.GetLevelById(levelId);
            StartCoroutine(StartLevelCoroutine(levelId));
            return true;
        }

        /// <summary>
        /// Reloads the current level.
        /// </summary>
        public void ReloadCurrentLevel()
        {
            if (CurrentLevel == null)
            {
                Debug.LogWarning("No current level to reload!");
                return;
            }

            if (_isLoading || _isTransitioning)
            {
                Debug.LogWarning("Cannot reload level while loading or transitioning!");
                return;
            }

            StartCoroutine(ReloadLevelCoroutine());
        }

        /// <summary>
        /// Loads the next level if it's unlocked.
        /// </summary>
        public void LoadNextLevel()
        {
            if (_isLoading || _isTransitioning)
            {
                Debug.LogWarning("Cannot load next level while loading or transitioning!");
                return;
            }

            var nextLevelIndex = CurrentLevel.LevelData.LevelId + 1;
            if (nextLevelIndex > levelDataCollection.levels.Length - 1)
            {
                if (isLoopingLevels)
                {
                    nextLevelIndex = 0; // Loop back to the first level
                }
                else
                {
                    Debug.LogWarning("No more levels available!");
                    return; // No next level available
                }
            }

            var nextLevelData = levelDataCollection.levels[nextLevelIndex];
            if (nextLevelData != null)
            {
                StartLevel(nextLevelData.LevelId);
            }
            else
            {
                Debug.Log("No next level available!");
            }
        }

        /// <summary>
        /// Loads the previous level.
        /// </summary>
        public void LoadPreviousLevel()
        {
            if (_isLoading || _isTransitioning)
            {
                Debug.LogWarning("Cannot load previous level while loading or transitioning!");
                return;
            }

            var previousLevelIndex = CurrentLevel.LevelData.LevelId - 1;
            if (previousLevelIndex < 0)
            {
                if (isLoopingLevels)
                {
                    previousLevelIndex = levelDataCollection.levels.Length - 1; // Loop back to the last level
                }
                else
                {
                    Debug.LogWarning("No previous level available!");
                    return; // No previous level available
                }
            }

            var previousLevelData = levelDataCollection.levels[previousLevelIndex];
            if (previousLevelData != null)
            {
                StartLevel(previousLevelData.LevelId);
            }
            else
            {
                Debug.Log("No previous level available!");
            }
        }

        /// <summary>
        /// Gets all unlocked levels for the current player.
        /// </summary>
        /// <returns>List of unlocked level data</returns>
        public System.Collections.Generic.List<LevelData> GetUnlockedLevels()
        {
            var unlockedLevels = new System.Collections.Generic.List<LevelData>();
            if (GameManager.Instance?.PlayerData != null)
            {
                foreach (var levelData in levelDataCollection.levels)
                {
                    if (GameManager.Instance.PlayerData.IsLevelUnlocked(levelData.LevelId))
                    {
                        unlockedLevels.Add(levelData);
                    }
                }
            }

            return unlockedLevels;
        }

        /// <summary>
        /// Checks if a specific level is unlocked.
        /// </summary>
        /// <param name="levelId">The level ID to check</param>
        /// <returns>True if the level is unlocked, false otherwise</returns>
        public bool IsLevelUnlocked(int levelId)
        {
            if (GameManager.Instance?.PlayerData != null)
            {
                return GameManager.Instance.PlayerData.IsLevelUnlocked(levelId);
            }

            return false;
        }

        /// <summary>
        /// Manually unlocks a level.
        /// </summary>
        /// <param name="levelId">The level ID to unlock</param>
        public void UnlockLevel(int levelId)
        {
            if (GameManager.Instance?.PlayerData != null)
            {
                GameManager.Instance.PlayerData.UnlockLevel(levelId);

                // Trigger level unlocked event
                OnLevelUnlocked?.Invoke(levelId);
                LevelManagerEvent.Trigger(LevelManagerEvent.EventType.LevelUnlocked, levelId);

                Debug.Log($"Level {levelId} manually unlocked!");
            }
        }

        /// <summary>
        /// Manually unlocks multiple levels.
        /// </summary>
        /// <param name="levelIds">Array of level IDs to unlock</param>
        public void UnlockLevels(params int[] levelIds)
        {
            if (GameManager.Instance?.PlayerData != null)
            {
                foreach (var levelId in levelIds)
                {
                    UnlockLevel(levelId);
                }
            }
        }

        #region Coroutines

        private IEnumerator StartLevelManagerCoroutine()
        {
            _isLoading = true;

            // Get the level to start from player data or default to first unlocked level
            int levelToStart = 0;
            if (GameManager.Instance != null && GameManager.Instance.PlayerData != null)
            {
                levelToStart = GameManager.Instance.PlayerData.CurrentLevelId;

                // Ensure the level is unlocked
                if (!IsLevelUnlocked(levelToStart))
                {
                    // Find the first unlocked level
                    var unlockedLevels = GetUnlockedLevels();
                    if (unlockedLevels.Count > 0)
                    {
                        levelToStart = unlockedLevels[0].LevelId;
                    }
                }
            }

            yield return StartCoroutine(StartLevelCoroutine(levelToStart));

            _isLoading = false;
        }

        private IEnumerator StartLevelCoroutine(int levelId)
        {
            _isLoading = true;

            // Clean up current level
            yield return StartCoroutine(CleanupCurrentLevel());

            // Load level data
            var levelData = levelDataCollection.GetLevelById(levelId);
            if (levelData == null)
            {
                Debug.LogError($"Failed to load level with ID {levelId}!");
                _isLoading = false;
                yield break;
            }

            var task = levelLoader.LoadLevel(levelData, levelContainer);
            yield return new WaitUntil(() => task.IsCompleted);
            var level = task.Result;
            if (level)
            {
                CurrentLevel = level;
                CurrentLevelData = levelData;

                // Subscribe to level events
                SubscribeToLevelEvents(CurrentLevel);

                // Trigger level started event
                OnLevelStarted?.Invoke(CurrentLevel);
                LevelManagerEvent.Trigger(LevelManagerEvent.EventType.LevelStarted, CurrentLevelData.LevelId);

                Debug.Log($"Started level: {CurrentLevelData.LevelName}");
            }
            else
            {
                Debug.LogError($"Failed to start level: {levelId}");
            }

            _isLoading = false;
        }

        private IEnumerator ReloadLevelCoroutine()
        {
            _isTransitioning = true;

            // Wait for transition delay
            yield return new WaitForSeconds(levelTransitionDelay);

            // Clean up current level
            yield return StartCoroutine(CleanupCurrentLevel());

            // Create new level with same data
            var levelTask = levelLoader.LoadLevel(CurrentLevelData, levelContainer);
            // wait for level to load
            yield return new WaitUntil(() => levelTask.IsCompleted);
            var level = levelTask.Result;
            if (level)
            {
                CurrentLevel = level;

                // Subscribe to level events
                SubscribeToLevelEvents(CurrentLevel);

                // Trigger level reloaded event
                OnLevelReloaded?.Invoke(CurrentLevel);
                LevelManagerEvent.Trigger(LevelManagerEvent.EventType.LevelStarted, CurrentLevelData.LevelId);

                Debug.Log($"Reloaded level: {CurrentLevelData.LevelName}");
            }

            _isTransitioning = false;
        }

        private IEnumerator CleanupCurrentLevel()
        {
            if (CurrentLevel != null)
            {
                // Unsubscribe from events
                UnsubscribeFromLevelEvents(CurrentLevel);

                // Destroy the level
                if (Application.isPlaying)
                {
                    Destroy(CurrentLevel.gameObject);
                }
                else
                {
                    DestroyImmediate(CurrentLevel.gameObject);
                }

                CurrentLevel = null;
            }

            yield return null;
        }

        #endregion

        #region Event Handlers

        private void SubscribeToLevelEvents(Level level)
        {
            if (!level) return;

            Level.OnLevelCompleted += HandleLevelCompleted;
            Level.OnLevelFailed += HandleLevelFailed;
        }

        private void UnsubscribeFromLevelEvents(Level level)
        {
            if (!level) return;

            Level.OnLevelCompleted -= HandleLevelCompleted;
            Level.OnLevelFailed -= HandleLevelFailed;
        }

        private void HandleLevelCompleted(Level level)
        {
            if (level != CurrentLevel) return;

            // Update player data
            if (GameManager.Instance != null && GameManager.Instance.PlayerData != null)
            {
                GameManager.Instance.PlayerData.CompleteLevel(CurrentLevelData.LevelId);

                // Check if next level was unlocked
                var nextLeveId = CurrentLevelData.LevelId + 1;
                GameManager.Instance.PlayerData.SetCurrentLevel(nextLeveId);
                GameManager.Instance.SaveGame();
                var nextLevelData = levelDataCollection.GetLevelById(nextLeveId);
                if (nextLevelData != null && !GameManager.Instance.PlayerData.IsLevelUnlocked(nextLevelData.LevelId))
                {
                    // Unlock the next level
                    GameManager.Instance.PlayerData.UnlockLevel(nextLevelData.LevelId);

                    // Trigger level unlocked event
                    OnLevelUnlocked?.Invoke(nextLevelData.LevelId);
                    LevelManagerEvent.Trigger(LevelManagerEvent.EventType.LevelUnlocked, nextLevelData.LevelId);

                    Debug.Log($"Level {nextLevelData.LevelId} unlocked!");
                }
            }

            // Trigger level completed event
            OnLevelCompleted?.Invoke(level);
            LevelManagerEvent.Trigger(LevelManagerEvent.EventType.LevelCompleted, CurrentLevelData.LevelId);

            Debug.Log($"Level completed: {CurrentLevelData.LevelName}");
        }

        private void HandleLevelFailed(Level level)
        {
            if (level != CurrentLevel) return;

            // Update player data
            if (GameManager.Instance != null && GameManager.Instance.PlayerData != null)
            {
                GameManager.Instance.PlayerData.totalGamesLost++;
            }

            // Trigger level failed event
            OnLevelFailed?.Invoke(level);
            LevelManagerEvent.Trigger(LevelManagerEvent.EventType.LevelFailed, CurrentLevelData.LevelId);

            Debug.Log($"Level failed: {CurrentLevelData.LevelName}");
        }

        #endregion
    }
}