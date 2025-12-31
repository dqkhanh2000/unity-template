using System;
using UnityEngine;
using UnityEngine.Events;

namespace GameTemplate.Runtime.Core
{
    /// <summary>
    /// Basic level instance that manages level state and events.
    /// This is a flexible base class that can be extended for specific game implementations.
    /// </summary>
    public class Level : MonoBehaviour
    {
        [Header("Level Configuration")]
        [SerializeField] protected LevelData levelData;
        [SerializeField] protected bool autoInitialize = true;
        [SerializeField] protected bool enableDebugLogs = false;
        
        [Header("Level State")]
        [SerializeField] protected LevelState _currentState = LevelState.Unknown;
        
        // Events
        public UnityEvent<Level> OnLevelStarted;
        public UnityEvent<Level> OnLevelCompleted;
        public UnityEvent<Level> OnLevelFailed;
        public UnityEvent<Level> OnLevelRestarted;
        
        // Properties
        public LevelData LevelData => levelData;
        
        public bool IsActive => CurrentState == LevelState.Playing;
        
        
        public LevelState CurrentState
        {
            get => _currentState;
            private set
            {
                if (_currentState != value)
                {
                    var previousState = _currentState;
                    _currentState = value;
                    LevelStateChangeEvent.Trigger(this, previousState, value);
                }
            }
        }
        
        protected virtual void Awake()
        {
            if (autoInitialize)
            {
                Initialize();
            }
        }
        
        protected virtual void Start()
        {
            if (autoInitialize && CurrentState == LevelState.Ready)
            {
                StartLevel();
            }
        }
        
        /// <summary>
        /// Initializes the level with the provided level data.
        /// </summary>
        /// <param name="data">The level data to use for this level</param>
        public virtual void Initialize(LevelData data = null)
        {
            if (data != null)
            {
                levelData = data;
            }
            
            if (levelData == null)
            {
                Debug.LogError("LevelData is required to initialize a level!");
                return;
            }
            
            CurrentState = LevelState.Loading;
            
            PrepareLevel();
            
            CurrentState = LevelState.Loaded;
            
            Log($"Level '{levelData.LevelName}' initialized");
        }

        /// <summary>
        /// Internal method to prepare the level before starting.
        /// </summary>
        protected virtual void PrepareLevel()
        {
            CurrentState = LevelState.Ready;
            Log($"Level '{levelData.LevelName}' is ready");
        }
        
        /// <summary>
        /// Starts the level.
        /// </summary>
        public virtual void StartLevel()
        {
            if (CurrentState != LevelState.Loaded && CurrentState != LevelState.Ready)
            {
                Debug.LogWarning("Level is already started or completed!");
                return;
            }
            
            CurrentState = LevelState.Playing;
            
            OnLevelStarted?.Invoke(this);
            
            Log($"Level '{levelData.LevelName}' started");
        }
        
        public virtual void Revive()
        {
            if (CurrentState != LevelState.Failed)
            {
                Debug.LogWarning("Level is not in a failed state!");
                return;
            }
            
            CurrentState = LevelState.Playing;
            
            Log($"Level '{levelData.LevelName}' revived");
        }

        public virtual void Update()
        {
            if (CurrentState == LevelState.Playing)
            {
                // Check win/lose conditions
                if (IsMatchWinCondition())
                {
                    CompleteLevel();
                }
                else if (IsMatchFailCondition())
                {
                    FailLevel();
                }
                else
                {
                    LevelUpdate();
                }
            }
        }
        
        protected virtual void LevelUpdate()
        {
            // Override in derived classes for per-frame updates
        }

        /// <summary>
        /// Checks if the level meets the win condition.
        /// </summary>
        public virtual bool IsMatchWinCondition()
        {
            // Override this method in derived classes to implement specific win conditions
            return false;
        }
        
        /// <summary>
        /// Checks if the level meets the fail condition.
        /// </summary>
        public virtual bool IsMatchFailCondition()
        {
            // Override this method in derived classes to implement specific fail conditions
            return false;
        }
        
        /// <summary>
        /// Completes the level successfully.
        /// </summary>
        public virtual void CompleteLevel()
        {
            if (CurrentState != LevelState.Playing)
                return;
                
            CurrentState = LevelState.Completed;
            
            OnLevelCompleted?.Invoke(this);
            
            Log($"Level '{levelData.LevelName}' completed");
        }
        
        /// <summary>
        /// Fails the level.
        /// </summary>
        public virtual void FailLevel()
        {
            if (CurrentState != LevelState.Playing)
                return;
                
            CurrentState = LevelState.Failed;
            
            OnLevelFailed?.Invoke(this);
            
            Log($"Level '{levelData.LevelName}' failed");
        }
        
        /// <summary>
        /// Restarts the level.
        /// </summary>
        public virtual void RestartLevel()
        {
            CurrentState = LevelState.Loaded;
            
            OnLevelRestarted?.Invoke(this);
            
            Log($"Level '{levelData.LevelName}' restarted");
            
            // Start the level again
            StartLevel();
        }
        
        /// <summary>
        /// Gets whether the level is completed successfully.
        /// </summary>
        /// <returns>True if the level is completed, false otherwise</returns>
        public virtual bool IsCompleted()
        {
            return CurrentState == LevelState.Completed;
        }
        
        /// <summary>
        /// Gets whether the level has failed.
        /// </summary>
        /// <returns>True if the level has failed, false otherwise</returns>
        public virtual bool IsFailed()
        {
            return CurrentState == LevelState.Failed;
        }
        
        /// <summary>
        /// Gets whether the level is finished (completed or failed).
        /// </summary>
        /// <returns>True if the level is finished, false otherwise</returns>
        public virtual bool IsFinished()
        {
            return CurrentState == LevelState.Completed || CurrentState == LevelState.Failed;
        }
        
        protected virtual void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log(message);
            }
        }
    }
    
    /// <summary>
    /// Represents the different states a level can be in.
    /// </summary>
    public enum LevelState
    {
        Unknown,
        Loading,
        Loaded,
        Ready,
        Playing,
        Completed,
        Failed
    }
} 