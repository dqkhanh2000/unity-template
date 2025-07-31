using KaneTemplate.EventSystem;

namespace KaneTemplate.Core
{
    #region Game State Events
    
    public struct GameStateChangeEvent : IEvent
    {
        public GameState PreviousState { get; }
        public GameState NewState { get; }
        
        public GameStateChangeEvent(GameState previousState, GameState newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }
        
        public string EventId => $"GameStateEvent_{NewState}";
        public bool IsValid => true;
        
        public static void Trigger(GameState previousState, GameState newState)
        {
            EventManager.TriggerEvent(new GameStateChangeEvent(previousState, newState));
        }
    }
    
    #endregion
    
    #region Level Events
    
    /// <summary>
    ///  Event triggered when a level state changes.
    /// </summary>
    public struct LevelStateChangeEvent : IEvent
    {
        public Level Level { get; }
        public LevelState PreviousState { get; }
        public LevelState NewState { get; }
        
        public LevelStateChangeEvent(Level level, LevelState previousState, LevelState newState)
        {
            Level = level;
            PreviousState = previousState;
            NewState = newState;
        }
        
        public string EventId => $"LevelEvent_{Level?.GetInstanceID()}";
        public bool IsValid => Level != null;
        
        public static void Trigger(Level level, LevelState previousState, LevelState newState)
        {
            EventManager.TriggerEvent(new LevelStateChangeEvent(level, previousState, newState));
        }
    }
    
    #endregion

    #region Level Manager Event

    public struct LevelManagerEvent : IEvent
    {
        
        public EventType Type { get; }
        public int LevelId { get; }
        private LevelManagerEvent(EventType eventType, int levelId)
        {
            Type = eventType;
            LevelId = levelId;
        }
        
        public string EventId => $"LevelManagerEvent_{Type}_{new System.Random().Next()}";
        public bool IsValid => true;

        public enum EventType
        {
            LevelUnlocked,
            LevelStarted,
            LevelCompleted,
            LevelFailed,
            LevelRestarted
        }
        
        public static void Trigger(EventType eventType, int levelId = 0)
        {
            var eventInstance = new LevelManagerEvent(eventType, levelId);
            EventManager.TriggerEvent(eventInstance);
        }
    }

    #endregion
} 