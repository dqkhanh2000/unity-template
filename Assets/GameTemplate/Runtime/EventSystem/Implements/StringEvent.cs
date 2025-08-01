namespace GameTemplate.Runtime.EventSystem.Implements
{
    /// <summary>
    /// A simple string-based event that can be used for basic event communication.
    /// This event contains a string name that can be used to identify the event type.
    /// </summary>
    /// <remarks>
    /// StringEvent is useful for simple event communication where you only need to
    /// identify the event type by name. For more complex events, consider creating
    /// custom event structs with additional data.
    /// </remarks>
    public struct StringEvent : IEvent
    {
        /// <summary>
        /// Initializes a new instance of the StringEvent struct.
        /// </summary>
        /// <param name="name">The name/identifier of the event.</param>
        public StringEvent(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Triggers a StringEvent with the specified name.
        /// This is a convenience method that creates and triggers the event in one call.
        /// </summary>
        /// <param name="name">The name of the event to trigger.</param>
        /// <remarks>
        /// This method is equivalent to calling:
        /// <code>EventManager.TriggerEvent(new StringEvent(name));</code>
        /// </remarks>
        public static void Trigger(string name)
        {
            EventManager.TriggerEvent(new StringEvent(name));
        }

        /// <summary>
        /// Gets the name/identifier of this event.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the unique identifier for this event type.
        /// </summary>
        public string EventId => $"StringEvent_{Name}";

        /// <summary>
        /// Gets whether this event is valid and can be processed.
        /// </summary>
        /// <remarks>
        /// A StringEvent is considered valid if the Name is not null or empty.
        /// </remarks>
        public bool IsValid => !string.IsNullOrEmpty(Name);
    }
}