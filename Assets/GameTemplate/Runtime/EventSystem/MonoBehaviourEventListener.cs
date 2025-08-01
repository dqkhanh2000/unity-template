using UnityEngine;

namespace GameTemplate.Runtime.EventSystem
{
    /// <summary>
    /// Base abstract class for MonoBehaviour components that need to listen to events.
    /// This class automatically handles subscription and unsubscription to events
    /// in OnEnable and OnDisable lifecycle methods.
    /// </summary>
    /// <typeparam name="T">The type of event this listener can handle. Must be a struct implementing IEvent.</typeparam>
    /// <remarks>
    /// <para>
    /// This class provides automatic event subscription management, eliminating the need
    /// to manually call Subscribe/Unsubscribe in your MonoBehaviour lifecycle methods.
    /// Simply inherit from this class and implement the OnEventTriggered method.
    /// </para>
    /// <para>
    /// Example usage:
    /// <code>
    /// public class MyGameObject : MonoBehaviourEventListener&lt;MyEvent&gt;
    /// {
    ///     public override void OnEventTriggered(MyEvent e)
    ///     {
    ///         // Handle the event
    ///         Debug.Log($"Received event: {e.EventId}");
    ///     }
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// The class automatically:
    /// - Subscribes to events when the GameObject is enabled
    /// - Unsubscribes from events when the GameObject is disabled
    /// - Handles cleanup when the GameObject is destroyed
    /// </para>
    /// </remarks>
    public abstract class MonoBehaviourEventListener<T> : MonoBehaviour, IEventListener<T> where T : struct, IEvent
    {
        /// <summary>
        /// Called when the GameObject is enabled. Automatically subscribes to events.
        /// </summary>
        /// <remarks>
        /// Override this method if you need custom OnEnable behavior, but remember
        /// to call base.OnEnable() to maintain event subscription functionality.
        /// </remarks>
        public virtual void OnEnable()
        {
            EventManager.Subscribe(this);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Automatically unsubscribes from events.
        /// </summary>
        /// <remarks>
        /// Override this method if you need custom OnDisable behavior, but remember
        /// to call base.OnDisable() to maintain event unsubscription functionality.
        /// </remarks>
        public virtual void OnDisable()
        {
            EventManager.Unsubscribe(this);
        }

        /// <summary>
        /// Called when an event of type T is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        /// <remarks>
        /// Implement your event handling logic in this method. This is where you'll
        /// respond to events that this listener is subscribed to.
        /// </remarks>
        public abstract void OnEventTriggered(T e);
    }

    /// <summary>
    /// Abstract class for MonoBehaviour components that need to listen to two different event types.
    /// Automatically handles subscription and unsubscription for both event types.
    /// </summary>
    /// <typeparam name="T1">The first event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T2">The second event type. Must be a struct implementing IEvent.</typeparam>
    /// <remarks>
    /// This class extends MonoBehaviourEventListener&lt;T1&gt; and adds support for a second event type.
    /// Useful when a component needs to respond to multiple different types of events.
    /// </remarks>
    public abstract class MonoBehaviourEventListener<T1, T2> : MonoBehaviourEventListener<T1>,  IEventListener<T2>
        where T1 : struct, IEvent
        where T2 : struct, IEvent
    {
        /// <summary>
        /// Called when the GameObject is enabled. Subscribes to both event types.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.Subscribe<T2>(this);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Unsubscribes from both event types.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.Unsubscribe<T2>(this);
        }

        /// <summary>
        /// Called when an event of type T2 is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        public abstract void OnEventTriggered(T2 e);
    }

    /// <summary>
    /// Abstract class for MonoBehaviour components that need to listen to three different event types.
    /// Automatically handles subscription and unsubscription for all three event types.
    /// </summary>
    /// <typeparam name="T1">The first event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T2">The second event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T3">The third event type. Must be a struct implementing IEvent.</typeparam>
    public abstract class MonoBehaviourEventListener<T1, T2, T3> : MonoBehaviourEventListener<T1, T2>, IEventListener<T3>
        where T1 : struct, IEvent
        where T2 : struct, IEvent
        where T3 : struct, IEvent
    {
        /// <summary>
        /// Called when the GameObject is enabled. Subscribes to all three event types.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.Subscribe<T3>(this);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Unsubscribes from all three event types.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.Unsubscribe<T3>(this);
        }

        /// <summary>
        /// Called when an event of type T3 is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        public abstract void OnEventTriggered(T3 e);
    }

    /// <summary>
    /// Abstract class for MonoBehaviour components that need to listen to four different event types.
    /// Automatically handles subscription and unsubscription for all four event types.
    /// </summary>
    /// <typeparam name="T1">The first event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T2">The second event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T3">The third event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T4">The fourth event type. Must be a struct implementing IEvent.</typeparam>
    public abstract class MonoBehaviourEventListener<T1, T2, T3, T4> : MonoBehaviourEventListener<T1, T2, T3>, IEventListener<T4>
        where T1 : struct, IEvent
        where T2 : struct, IEvent
        where T3 : struct, IEvent
        where T4 : struct, IEvent
    {
        /// <summary>
        /// Called when the GameObject is enabled. Subscribes to all four event types.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.Subscribe<T4>(this);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Unsubscribes from all four event types.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.Unsubscribe<T4>(this);
        }

        /// <summary>
        /// Called when an event of type T4 is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        public abstract void OnEventTriggered(T4 e);
    }

    /// <summary>
    /// Abstract class for MonoBehaviour components that need to listen to five different event types.
    /// Automatically handles subscription and unsubscription for all five event types.
    /// </summary>
    /// <typeparam name="T1">The first event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T2">The second event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T3">The third event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T4">The fourth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T5">The fifth event type. Must be a struct implementing IEvent.</typeparam>
    public abstract class MonoBehaviourEventListener<T1, T2, T3, T4, T5> : MonoBehaviourEventListener<T1, T2, T3, T4>, IEventListener<T5>
        where T1 : struct, IEvent
        where T2 : struct, IEvent
        where T3 : struct, IEvent
        where T4 : struct, IEvent
        where T5 : struct, IEvent
    {
        /// <summary>
        /// Called when the GameObject is enabled. Subscribes to all five event types.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.Subscribe<T5>(this);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Unsubscribes from all five event types.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.Unsubscribe<T5>(this);
        }

        /// <summary>
        /// Called when an event of type T5 is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        public abstract void OnEventTriggered(T5 e);
    }

    /// <summary>
    /// Abstract class for MonoBehaviour components that need to listen to six different event types.
    /// Automatically handles subscription and unsubscription for all six event types.
    /// </summary>
    /// <typeparam name="T1">The first event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T2">The second event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T3">The third event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T4">The fourth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T5">The fifth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T6">The sixth event type. Must be a struct implementing IEvent.</typeparam>
    public abstract class MonoBehaviourEventListener<T1, T2, T3, T4, T5, T6> : MonoBehaviourEventListener<T1, T2, T3, T4, T5>, IEventListener<T6>
        where T1 : struct, IEvent
        where T2 : struct, IEvent
        where T3 : struct, IEvent
        where T4 : struct, IEvent
        where T5 : struct, IEvent
        where T6 : struct, IEvent
    {
        /// <summary>
        /// Called when the GameObject is enabled. Subscribes to all six event types.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.Subscribe<T6>(this);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Unsubscribes from all six event types.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.Unsubscribe<T6>(this);
        }

        /// <summary>
        /// Called when an event of type T6 is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        public abstract void OnEventTriggered(T6 e);
    }

    /// <summary>
    /// Abstract class for MonoBehaviour components that need to listen to seven different event types.
    /// Automatically handles subscription and unsubscription for all seven event types.
    /// </summary>
    /// <typeparam name="T1">The first event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T2">The second event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T3">The third event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T4">The fourth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T5">The fifth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T6">The sixth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T7">The seventh event type. Must be a struct implementing IEvent.</typeparam>
    public abstract class MonoBehaviourEventListener<T1, T2, T3, T4, T5, T6, T7> : MonoBehaviourEventListener<T1, T2, T3, T4, T5, T6>, IEventListener<T7>
        where T1 : struct, IEvent
        where T2 : struct, IEvent
        where T3 : struct, IEvent
        where T4 : struct, IEvent
        where T5 : struct, IEvent
        where T6 : struct, IEvent
        where T7 : struct, IEvent
    {
        /// <summary>
        /// Called when the GameObject is enabled. Subscribes to all seven event types.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.Subscribe<T7>(this);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Unsubscribes from all seven event types.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.Unsubscribe<T7>(this);
        }

        /// <summary>
        /// Called when an event of type T7 is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        public abstract void OnEventTriggered(T7 e);
    }

    /// <summary>
    /// Abstract class for MonoBehaviour components that need to listen to eight different event types.
    /// Automatically handles subscription and unsubscription for all eight event types.
    /// </summary>
    /// <typeparam name="T1">The first event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T2">The second event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T3">The third event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T4">The fourth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T5">The fifth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T6">The sixth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T7">The seventh event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T8">The eighth event type. Must be a struct implementing IEvent.</typeparam>
    public abstract class MonoBehaviourEventListener<T1, T2, T3, T4, T5, T6, T7, T8> : MonoBehaviourEventListener<T1, T2, T3, T4, T5, T6, T7>, IEventListener<T8>
        where T1 : struct, IEvent
        where T2 : struct, IEvent
        where T3 : struct, IEvent
        where T4 : struct, IEvent
        where T5 : struct, IEvent
        where T6 : struct, IEvent
        where T7 : struct, IEvent
        where T8 : struct, IEvent
    {
        /// <summary>
        /// Called when the GameObject is enabled. Subscribes to all eight event types.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.Subscribe<T8>(this);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Unsubscribes from all eight event types.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.Unsubscribe<T8>(this);
        }

        /// <summary>
        /// Called when an event of type T8 is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        public abstract void OnEventTriggered(T8 e);
    }

    /// <summary>
    /// Abstract class for MonoBehaviour components that need to listen to nine different event types.
    /// Automatically handles subscription and unsubscription for all nine event types.
    /// </summary>
    /// <typeparam name="T1">The first event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T2">The second event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T3">The third event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T4">The fourth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T5">The fifth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T6">The sixth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T7">The seventh event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T8">The eighth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T9">The ninth event type. Must be a struct implementing IEvent.</typeparam>
    public abstract class MonoBehaviourEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9> : MonoBehaviourEventListener<T1, T2, T3, T4, T5, T6, T7, T8>, IEventListener<T9>
        where T1 : struct, IEvent
        where T2 : struct, IEvent
        where T3 : struct, IEvent
        where T4 : struct, IEvent
        where T5 : struct, IEvent
        where T6 : struct, IEvent
        where T7 : struct, IEvent
        where T8 : struct, IEvent
        where T9 : struct, IEvent
    {
        /// <summary>
        /// Called when the GameObject is enabled. Subscribes to all nine event types.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.Subscribe<T9>(this);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Unsubscribes from all nine event types.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.Unsubscribe<T9>(this);
        }

        /// <summary>
        /// Called when an event of type T9 is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        public abstract void OnEventTriggered(T9 e);
    }

    /// <summary>
    /// Abstract class for MonoBehaviour components that need to listen to ten different event types.
    /// Automatically handles subscription and unsubscription for all ten event types.
    /// </summary>
    /// <typeparam name="T1">The first event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T2">The second event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T3">The third event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T4">The fourth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T5">The fifth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T6">The sixth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T7">The seventh event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T8">The eighth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T9">The ninth event type. Must be a struct implementing IEvent.</typeparam>
    /// <typeparam name="T10">The tenth event type. Must be a struct implementing IEvent.</typeparam>
    public abstract class MonoBehaviourEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : MonoBehaviourEventListener<T1, T2, T3, T4, T5, T6, T7, T8, T9>, IEventListener<T10>
        where T1 : struct, IEvent
        where T2 : struct, IEvent
        where T3 : struct, IEvent
        where T4 : struct, IEvent
        where T5 : struct, IEvent
        where T6 : struct, IEvent
        where T7 : struct, IEvent
        where T8 : struct, IEvent
        where T9 : struct, IEvent
        where T10 : struct, IEvent
    {
        /// <summary>
        /// Called when the GameObject is enabled. Subscribes to all ten event types.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.Subscribe<T10>(this);
        }

        /// <summary>
        /// Called when the GameObject is disabled. Unsubscribes from all ten event types.
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.Unsubscribe<T10>(this);
        }

        /// <summary>
        /// Called when an event of type T10 is triggered.
        /// </summary>
        /// <param name="e">The event that was triggered.</param>
        public abstract void OnEventTriggered(T10 e);
    }
}