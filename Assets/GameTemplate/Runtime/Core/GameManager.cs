using System;
using System.Threading.Tasks;
using GameTemplate.Runtime.Core.Currencies;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace GameTemplate.Runtime.Core
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
        [SerializeField] private bool autoStartLevelManager = false;
        [SerializeField] private bool autoSaveOnExit = true;
        [SerializeField] private float autoSaveInterval = 30f; // seconds
        [SerializeField] private bool allowSaveGame = true;
        [SerializeField] private ApplicationSettings applicationSettings;
        [SerializeField] private bool enableDebugLogs = false;
        
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
        public ApplicationSettings ApplicationSettings => applicationSettings;
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
            
            // Load application settings first
            LoadApplicationSettings();
            
            // Load player data
            LoadPlayerData();
            
            CurrentState = GameState.Initialized;
            
            // Trigger initialization event
            onGameInitialized?.Invoke();
            
            Log("initialized");
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
            if (LevelManager.Instance != null && autoStartLevelManager)
            {
                LevelManager.Instance.StartLevelManager();
            }
            
            onGameStarted?.Invoke();
            
            Log("Game started");
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
            
            Log("Game paused");
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
            
            Log("Game resumed");
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
            
            Log("Game ended");
        }
        
        /// <summary>
        /// Saves the current game state.
        /// </summary>
        public async void SaveGame(bool useMultiThread = false)
        {
            if (!allowSaveGame)
            {
                Debug.LogWarning("Saving game is disabled!");
                return;
            }
            
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

                if (useMultiThread)
                {
                    // Serialize and save on background thread
                    await Task.Run(() =>
                    {
                        var json = JsonConvert.SerializeObject(playerData, Formatting.Indented);
                        PlayerPrefs.SetString("PlayerData", json);
                        PlayerPrefs.Save();

                        // Switch back to main thread for PlayerPrefs operations
                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            lastSaveTime = Time.time;
                            onGameSaved?.Invoke();
                            Log("Game saved successfully (multi-threaded)");
                        });
                    });
                }
                else
                {
                    // Save directly on main thread
                    var json = JsonConvert.SerializeObject(playerData, Formatting.Indented);
                    PlayerPrefs.SetString("PlayerData", json);
                    PlayerPrefs.Save();

                    lastSaveTime = Time.time;
                    onGameSaved?.Invoke();

                    Log("Game saved successfully");
                }
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
                    playerData = JsonConvert.DeserializeObject<PlayerData>(json);
                    
                    // Trigger game loaded event
                    onGameLoaded?.Invoke();
                    
                    Log("Game loaded successfully");
                }
                else
                {
                    Log("No saved game data found, creating new player data");
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
            
            Log("Game reset to initial state");
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
        
        /// <summary>
        /// Loads application settings and initializes currency system.
        /// </summary>
        private void LoadApplicationSettings()
        {
            // Try to find ApplicationSettings in Resources folder if not assigned
            if (applicationSettings == null)
            {
                applicationSettings = Resources.Load<ApplicationSettings>("ApplicationSettings");
            }
            
            // If still null, create default settings
            if (applicationSettings == null)
            {
                Debug.LogWarning("No ApplicationSettings found! Creating default settings.");
                applicationSettings = ScriptableObject.CreateInstance<ApplicationSettings>();
            }
            
            // Initialize currency system with settings
            CurrencySystem.Initialize(applicationSettings);
            
            // Apply target FPS
            Application.targetFrameRate = applicationSettings.TargetFPS;
            
            Log($"Application settings loaded - Target FPS: {applicationSettings.TargetFPS}, " +
                     $"Gem enabled: {applicationSettings.EnableGem}, " +
                     $"Diamond enabled: {applicationSettings.EnableDiamond}");
        }
        
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
        
        public void ClearAllData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Log("All player data cleared from PlayerPrefs");
        }
        
        #endregion
        
        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[GameManager] {message}");
            }
        }
    }
} 