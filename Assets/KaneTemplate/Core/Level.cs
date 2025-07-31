using System;
using UnityEngine;
using KaneTemplate.EventSystem;
using UnityEngine.Serialization;

namespace KaneTemplate.Core
{
    /// <summary>
    /// Basic level instance that manages level state and events.
    /// This is a flexible base class that can be extended for specific game implementations.
    /// </summary>
    public class Level : MonoBehaviour
    {
        [Header("Level Configuration")]
        [SerializeField] private LevelData levelData;
        [SerializeField] private bool autoInitialize = true;
        
        [Header("Level State")]
        [SerializeField] private LevelState _currentState = LevelState.NotStarted;
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
        
        // Events
        public static event Action<Level> OnLevelStarted;
        public static event Action<Level> OnLevelCompleted;
        public static event Action<Level> OnLevelFailed;
        public static event Action<Level> OnLevelRestarted;
        
        // Properties
        public LevelData LevelData => levelData;
        
        public bool IsActive => CurrentState == LevelState.Playing;
        
        protected virtual void Awake()
        {
            if (autoInitialize)
            {
                Initialize();
            }
        }
        
        protected virtual void Start()
        {
            if (autoInitialize && CurrentState == LevelState.NotStarted)
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
            
            CurrentState = LevelState.NotStarted;
            
            Debug.Log($"Level '{levelData.LevelName}' initialized");
        }
        
        /// <summary>
        /// Starts the level.
        /// </summary>
        public virtual void StartLevel()
        {
            if (CurrentState != LevelState.NotStarted)
            {
                Debug.LogWarning("Level is already started or completed!");
                return;
            }
            
            CurrentState = LevelState.Playing;
            
            OnLevelStarted?.Invoke(this);
            
            Debug.Log($"Level '{levelData.LevelName}' started");
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
            
            Debug.Log($"Level '{levelData.LevelName}' completed");
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
            
            Debug.Log($"Level '{levelData.LevelName}' failed");
        }
        
        /// <summary>
        /// Restarts the level.
        /// </summary>
        public virtual void RestartLevel()
        {
            CurrentState = LevelState.NotStarted;
            
            OnLevelRestarted?.Invoke(this);
            
            Debug.Log($"Level '{levelData.LevelName}' restarted");
            
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
    }
    
    /// <summary>
    /// Represents the different states a level can be in.
    /// </summary>
    public enum LevelState
    {
        NotStarted,
        Playing,
        Completed,
        Failed
    }
} 