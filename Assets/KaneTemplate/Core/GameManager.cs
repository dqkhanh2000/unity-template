using System;
using UnityEngine;
using KaneTemplate.EventSystem;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace KaneTemplate.Core
{
    /// <summary>
    /// Singleton game manager that handles basic game state and player data.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        public static GameManager Instance { get; private set; }
        
        [Header("Game Configuration")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private bool autoSaveOnExit = true;
        [SerializeField] private float autoSaveInterval = 30f; // seconds
        
        [Header("Game State")]
        [SerializeField] private PlayerData playerData;
        [SerializeField] private float gameStartTime;
        [SerializeField] private float lastSaveTime;
        
        // Events
        public UnityEvent onGameInitialized;
        public UnityEvent onGameStarted;
        public UnityEvent onGamePaused;
        public UnityEvent onGameResumed;
        public UnityEvent onGameEnded;
        public UnityEvent onGameSaved;
        public UnityEvent onGameLoaded;
        
        // Properties
        public PlayerData PlayerData => playerData;
        public float GameTime => Time.time - gameStartTime;
        public bool IsInitialized => CurrentState != GameState.None;
        public bool IsPlaying => CurrentState == GameState.Playing;
        public bool IsPaused => CurrentState == GameState.Paused;

        private GameState _currentState = GameState.None;
        public GameState CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState != value)
                {
                    var previousState = _currentState;
                    _currentState = value;
                    GameStateChangeEvent.Trigger(previousState, value);
                }
            }
        }

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                if (autoInitialize)
                {
                    Initialize();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Update()
        {
            if (IsPlaying)
            {
                CheckAutoSave();
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                PauseGame();
                if (autoSaveOnExit)
                {
                    SaveGame();
                }
            }
            else
            {
                ResumeGame();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                if (autoSaveOnExit)
                {
                    SaveGame();
                }
            }
        }
        
        private void OnApplicationQuit()
        {
            if (autoSaveOnExit)
            {
                SaveGame();
            }
        }
        
        /// <summary>
        /// Initializes the game manager and loads player data.
        /// </summary>
        public void Initialize()
        {
            if (CurrentState != GameState.None)
            {
                Debug.LogWarning("GameManager is already initialized!");
                return;
            }
            
            CurrentState = GameState.Initializing;
            
            // Load player data
            LoadPlayerData();
            
            CurrentState = GameState.Initialized;
            
            // Trigger initialization event
            onGameInitialized?.Invoke();
            
            Debug.Log("GameManager initialized");
        }
        
        /// <summary>
        /// Starts the game and begins gameplay.
        /// </summary>
        public void StartGame()
        {
            if (CurrentState != GameState.Initialized && CurrentState != GameState.Loaded)
            {
                Debug.LogWarning("GameManager must be initialized before starting!");
                return;
            }
            
            CurrentState = GameState.Playing;
            gameStartTime = Time.time;
            lastSaveTime = Time.time;
            
            // Start level manager
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.StartLevelManager();
            }
            
            onGameStarted?.Invoke();
            
            Debug.Log("Game started");
        }
        
        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            if (CurrentState != GameState.Playing)
                return;
                
            CurrentState = GameState.Paused;
            
            // Pause time scale
            Time.timeScale = 0f;
            
            // Trigger game paused event
            onGamePaused?.Invoke();
            
            Debug.Log("Game paused");
        }
        
        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused)
                return;
                
            CurrentState = GameState.Playing;
            
            // Resume time scale
            Time.timeScale = 1f;
            
            // Trigger game resumed event
            onGameResumed?.Invoke();
            
            Debug.Log("Game resumed");
        }
        
        /// <summary>
        /// Ends the game.
        /// </summary>
        public void EndGame()
        {
            if (CurrentState == GameState.None)
                return;
                
            CurrentState = GameState.GameOver;
            
            // Update player data
            if (playerData != null)
            {
                playerData.totalPlayTime += (int)GameTime;
                playerData.totalGamesPlayed++;
                playerData.UpdateSaveDate();
            }
            
            // Save game
            SaveGame();
            
            // Trigger game ended event
            onGameEnded?.Invoke();
            
            Debug.Log("Game ended");
        }
        
        /// <summary>
        /// Saves the current game state.
        /// </summary>
        public void SaveGame()
        {
            if (playerData == null)
            {
                Debug.LogWarning("No player data to save!");
                return;
            }
            
            try
            {
                // Update player data
                playerData.totalPlayTime += (int)GameTime;
                playerData.UpdateSaveDate();
                
                // Save to PlayerPrefs
                var json = JsonUtility.ToJson(playerData, true);
                PlayerPrefs.SetString("PlayerData", json);
                PlayerPrefs.Save();
                
                lastSaveTime = Time.time;
                
                // Trigger game saved event
                onGameSaved?.Invoke();
                
                Debug.Log("Game saved successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }
        
        /// <summary>
        /// Loads the saved game state.
        /// </summary>
        public void LoadGame()
        {
            try
            {
                if (PlayerPrefs.HasKey("PlayerData"))
                {
                    var json = PlayerPrefs.GetString("PlayerData");
                    playerData = JsonUtility.FromJson<PlayerData>(json);
                    
                    // Trigger game loaded event
                    onGameLoaded?.Invoke();
                    
                    Debug.Log("Game loaded successfully");
                }
                else
                {
                    Debug.Log("No saved game data found, creating new player data");
                    CreateNewPlayerData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                CreateNewPlayerData();
            }
        }
        
        /// <summary>
        /// Creates new player data and saves it.
        /// </summary>
        public void CreateNewPlayerData()
        {
            playerData = new PlayerData();
            SaveGame();
        }
        
        /// <summary>
        /// Resets the game to initial state.
        /// </summary>
        public void ResetGame()
        {
            // Reset player data
            CreateNewPlayerData();
            
            // Reset game state
            CurrentState = GameState.Initialized;
            gameStartTime = 0f;
            lastSaveTime = 0f;
            
            Debug.Log("Game reset to initial state");
        }
        
        /// <summary>
        /// Gets the formatted game time.
        /// </summary>
        /// <returns>Formatted game time string</returns>
        public string GetFormattedGameTime()
        {
            var totalTime = playerData?.totalPlayTime ?? 0;
            totalTime += (int)GameTime;
            
            int hours = totalTime / 3600;
            int minutes = (totalTime % 3600) / 60;
            int seconds = totalTime % 60;
            
            if (hours > 0)
                return $"{hours:00}:{minutes:00}:{seconds:00}";
            else
                return $"{minutes:00}:{seconds:00}";
        }
        
        #region Private Methods
        
        private void LoadPlayerData()
        {
            LoadGame();
        }
        
        private void CheckAutoSave()
        {
            if (Time.time - lastSaveTime >= autoSaveInterval)
            {
                SaveGame();
            }
        }
        
        #endregion
    }
} 