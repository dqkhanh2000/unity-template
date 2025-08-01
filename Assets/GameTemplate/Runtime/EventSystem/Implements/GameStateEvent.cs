using GameTemplate.Runtime.Core;

namespace GameTemplate.Runtime.EventSystem.Implements
{
    public struct GameStateEvent : IEvent
    {
        private string _eventId;
        
        public string EventId => _eventId ??= $"GameStateEvent_{State}_{LastState}";
        
        public GameState State { get; }
        
        public GameState LastState { get; }

        public bool IsValid => true;
        
        public GameStateEvent(GameState state, GameState lastState = GameState.None, string eventId = null)
        {
            _eventId = eventId ?? $"GameStateEvent_{state}_{lastState}_{System.Guid.NewGuid()}";
            State = state;
            LastState = lastState;
        }
        
        public static void Trigger(GameState state, GameState lastState = GameState.None)
        {
            EventManager.TriggerEvent(new GameStateEvent(state, lastState));
        }
    }
}