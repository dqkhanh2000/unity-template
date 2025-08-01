namespace GameTemplate.Runtime.EventSystem
{
    /// <summary>
    /// Base interface for all event listeners.
    /// This is a marker interface that doesn't contain any methods.
    /// </summary>
    /// <remarks>
    /// This interface is used for type checking and to maintain a common base type
    /// for all event listeners in the system.
    /// </remarks>
    public interface IEventListener
    {
        
    }
    
    /// <summary>
    /// Generic interface for event listeners that can handle specific event types.
    /// Implement this interface to receive notifications when events of type T are triggered.
    /// </summary>
    /// <typeparam name="T">The type of event this listener can handle. Must be a struct implementing IEvent.</typeparam>
    /// <remarks>
    /// <para>
    /// When implementing this interface:
    /// - Ensure your class properly subscribes and unsubscribes from events
    /// - Handle exceptions gracefully in OnEventTriggered
    /// - Consider using MonoBehaviourEventListener for automatic subscription management
    /// - Keep event handling logic lightweight to avoid blocking the main thread
    /// </para>
    /// <para>
    /// Example implementation:
    /// <code>
    /// public class MyListener : MonoBehaviour, IEventListener&lt;MyEvent&gt;
    /// {
    ///     void OnEnable() => EventManager.Subscribe&lt;MyEvent&gt;(this);
    ///     void OnDisable() => EventManager.Unsubscribe&lt;MyEvent&gt;(this);
    ///     
    ///     public void OnEventTriggered(MyEvent e)
    ///     {
    ///         // Handle the event
    ///     }
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    public interface IEventListener<in T> : IEventListener where T : struct, IEvent
    {
        /// <summary>
        /// Called when an event of type T is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        /// <remarks>
        /// This method is called for all subscribed listeners when an event is triggered.
        /// Implement your event handling logic here. Be mindful of performance and
        /// avoid throwing exceptions that could disrupt the event system.
        /// </remarks>
        public void OnEventTriggered(T e);
    }
}