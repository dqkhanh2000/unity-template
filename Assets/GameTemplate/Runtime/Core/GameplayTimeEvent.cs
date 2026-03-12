using GameTemplate.Runtime.EventSystem;

namespace GameTemplate.Runtime.Core
{
    /// <summary>
    /// Event triggered when a GameplayTime instance changes state.
    /// </summary>
    public struct GameplayTimeEvent : IEvent
    {
        public EventType Type { get; }
        
        /// <summary>
        /// The name of the GameplayTime instance that triggered this event.
        /// </summary>
        public string TimerName { get; }
        
        public string EventId => $"GameplayTimeEvent_{Type}_{TimerName}";
        public bool IsValid => true;

        public enum EventType
        {
            Started,
            Paused,
            Resumed,
            Stopped,
            Reset,
            SpeedChanged,
            CountdownComplete
        }

        public GameplayTimeEvent(EventType type, string timerName)
        {
            Type = type;
            TimerName = timerName;
        }

        public static void Trigger(EventType type, string timerName = "Global")
        {
            EventManager.TriggerEvent(new GameplayTimeEvent(type, timerName));
        }
    }
}

