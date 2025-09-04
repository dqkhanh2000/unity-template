namespace GameTemplate.Runtime.EventSystem
{
    /// <summary>
    /// Base interface for all events in the EventSystem.
    /// All events must implement this interface to be compatible with the EventManager.
    /// Events should be structs for better performance and to avoid heap allocations.
    /// </summary>
    /// <remarks>
    /// When implementing custom events, consider:
    /// - Making them structs for better performance
    /// - Including an EventId for debugging and validation
    /// - Implementing IsValid to ensure event data integrity
    /// - Keeping them immutable to prevent issues with event queuing
    /// </remarks>
    public interface IEvent
    {
        /// <summary>
        /// Gets a unique identifier for this event type.
        /// Used for debugging, logging, and event validation.
        /// </summary>
        string EventId { get; }
    }
}