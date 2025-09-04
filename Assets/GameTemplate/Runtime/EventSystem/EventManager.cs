using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameTemplate.Runtime.EventSystem
{
    /// <summary>
    /// Internal MonoBehaviour component used to manage coroutines for the EventManager.
    /// This is a singleton that handles the event queue processing.
    /// </summary>
    internal class EventManagerCoroutineRunner : MonoBehaviour
    {
        private static EventManagerCoroutineRunner _instance;
        
        public static EventManagerCoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("[EventManager Coroutine Runner]");
                    _instance = go.AddComponent<EventManagerCoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// This class handles event management, and can be used to broadcast events throughout the game, to tell one class (or many) that something's happened.
    /// Events are structs, you can define any kind of events you want. This manager comes with <see cref="StringEvent"/>, which are
    /// basically just made of a string, but you can work with more complex ones if you want.
    /// </summary>
    ///
    /// <example><remarks>
    ///  To trigger a new event, from anywhere, do <code>YOUR_EVENT.Trigger(YOUR_PARAMETERS)</code>
    /// For example will trigger a Save something.<code>StringEvent.Trigger("Save");</code>
    ///
    /// <para>
    /// You can also call <para><code>EventManager.TriggerEvent(YOUR_EVENT);</code></para>
    /// For example, to broadcast an StringEvent named GameStart to all listeners.<para><code>EventManager.TriggerEvent(new StringEvent("GameStart"));</code></para>
    /// </para>
    ///
    /// To start listening to an event from any class, there are 3 things you must do:
    ///<para>
    /// 1 - tell that your class implements the IEventListener interface for that kind of event.
    /// For example:
    /// <code>public class GUIManager : Singleton&lt;GUIManager&lt;IEventListener&lt;StringEvent&lt;</code>
    /// You can have more than one of these (one per event type).
    /// </para>
    /// <para>
    /// 2 - On Enable and Disable, respectively start and stop listening to the event :
    /// <code>
    /// void OnEnable()
    /// {
    ///     EventManager.Subscribe&lt;StringEvent&lt;(this);
    /// }
    /// void OnDisable()
    /// {
    ///     EventManager.UnSubscribe&lt;StringEvent&lt;(this);
    /// }
    /// </code>
    /// </para>
    ///
    /// <para>
    /// 3 - Implement the IEventListener interface for that event. For example :
    /// <code>
    /// public void OnEventRaised(StringEvent gameEvent)
    /// {
    ///     if (gameEvent.EventName == "GameOver")
    ///     {
    ///         // DO SOMETHING
    ///     }
    /// }
    /// </code>
    /// </para>
    /// will catch all events of type MMGameEvent emitted from anywhere in the game, and do something if it's named GameOver
    /// </remarks></example>
    public static class EventManager
    {
        // Thread safety lock object
        private static readonly object _lock = new();
        
        // Main subscribers using WeakReference to prevent memory leaks
        private static readonly Dictionary<Type, List<WeakReference<IEventListener>>> _subscribersList = new();
        
        // One-time subscribers using WeakReference
        private static readonly Dictionary<Type, List<WeakReference<IEventListener>>> _oneTimeSubscribersList = new();
        
        // Event queue to prevent recursive calls and ensure thread safety
        private static readonly Queue<object> _eventQueue = new();
        private static bool _isProcessingQueue;
        
        // Debug and logging
        private static bool _enableLogging = false;
        private static int _totalEventsTriggered;
        private static int _totalListenersNotified;

        /// <summary>
        /// Enables or disables debug logging for the EventManager.
        /// </summary>
        /// <param name="enabled">Whether to enable logging.</param>
        public static void SetLoggingEnabled(bool enabled)
        {
            _enableLogging = enabled;
        }

        /// <summary>
        /// Gets statistics about the EventManager usage.
        /// </summary>
        /// <returns>A tuple containing total events triggered and total listeners notified.</returns>
        public static (int eventsTriggered, int listenersNotified) GetStatistics()
        {
            return (_totalEventsTriggered, _totalListenersNotified);
        }

        /// <summary>
        /// Cleans up dead references from the subscriber lists.
        /// This should be called periodically to prevent memory leaks.
        /// </summary>
        public static void CleanupDeadReferences()
        {
            lock (_lock)
            {
                CleanupList(_subscribersList);
                CleanupList(_oneTimeSubscribersList);
                
                if (_enableLogging)
                {
                    Debug.Log($"[EventManager] Cleaned up dead references");
                }
            }
        }

        /// <summary>
        /// Subscribes the specified event listener to the specified event type.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="listener">The event listener to subscribe.</param>
        /// <remarks>
        /// This method is thread-safe and will automatically handle weak references
        /// to prevent memory leaks when listeners are destroyed.
        /// </remarks>
        public static void Subscribe<T>(IEventListener<T> listener) where T : struct, IEvent
        {
            if (listener == null)
            {
                Debug.LogWarning("[EventManager] Attempted to subscribe null listener");
                return;
            }

            lock (_lock)
            {
                var type = typeof(T);
                if (!_subscribersList.ContainsKey(type))
                {
                    _subscribersList.Add(type, new List<WeakReference<IEventListener>>());
                }
                
                var weakRef = new WeakReference<IEventListener>(listener);
                _subscribersList[type].Add(weakRef);
                
                if (_enableLogging)
                {
                    Debug.Log($"[EventManager] Subscribed {listener.GetType().Name} to {type.Name}");
                }
            }
        }

        /// <summary>
        /// Subscribes the specified event listener to the specified event type on a one-time basis.
        /// The listener will be automatically unsubscribed after the first event is triggered.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="listener">The event listener to subscribe.</param>
        /// <remarks>
        /// One-time subscriptions are useful for events that should only be handled once,
        /// such as initialization events or one-shot notifications.
        /// </remarks>
        public static void SubscribeOnce<T>(IEventListener<T> listener) where T : struct, IEvent
        {
            if (listener == null)
            {
                Debug.LogWarning("[EventManager] Attempted to subscribe null listener for one-time subscription");
                return;
            }

            lock (_lock)
            {
                var type = typeof(T);
                if (!_oneTimeSubscribersList.ContainsKey(type))
                {
                    _oneTimeSubscribersList.Add(type, new List<WeakReference<IEventListener>>());
                }
                
                var weakRef = new WeakReference<IEventListener>(listener);
                _oneTimeSubscribersList[type].Add(weakRef);
                
                if (_enableLogging)
                {
                    Debug.Log($"[EventManager] Subscribed {listener.GetType().Name} to {type.Name} (one-time)");
                }
            }
        }

        /// <summary>
        /// Unsubscribes the specified event listener from the specified event type.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="listener">The event listener to unsubscribe.</param>
        /// <remarks>
        /// This method is thread-safe and will remove the listener from both regular
        /// and one-time subscription lists.
        /// </remarks>
        public static void Unsubscribe<T>(IEventListener<T> listener) where T : struct, IEvent
        {
            if (listener == null) return;

            lock (_lock)
            {
                var type = typeof(T);
                
                // Remove from regular subscribers
                if (_subscribersList.TryGetValue(type, out var subscribers))
                {
                    subscribers.RemoveAll(weakRef => 
                        weakRef.TryGetTarget(out var target) && target == listener);
                }
                
                // Remove from one-time subscribers
                if (_oneTimeSubscribersList.TryGetValue(type, out var oneTimeSubscribers))
                {
                    oneTimeSubscribers.RemoveAll(weakRef => 
                        weakRef.TryGetTarget(out var target) && target == listener);
                }
                
                if (_enableLogging)
                {
                    Debug.Log($"[EventManager] Unsubscribed {listener.GetType().Name} from {type.Name}");
                }
            }
        }

        /// <summary>
        /// Triggers the specified event and notifies all subscribed event listeners.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <param name="eventRaiser">The event to trigger.</param>
        /// <remarks>
        /// This method is thread-safe and includes error handling. If an exception occurs
        /// in a listener, it will be logged but won't prevent other listeners from being notified.
        /// Events are queued to prevent recursive calls and ensure proper processing order.
        /// </remarks>
        public static void TriggerEvent<T>(T eventRaiser) where T : struct, IEvent
        {

            lock (_lock)
            {
                _eventQueue.Enqueue(eventRaiser);
                
                if (!_isProcessingQueue)
                {
                    // Start processing the queue using the coroutine runner
                    EventManagerCoroutineRunner.Instance.StartCoroutine(ProcessEventQueue());
                }
            }
        }

        /// <summary>
        /// Processes the event queue to ensure thread safety and prevent recursive calls.
        /// </summary>
        private static IEnumerator ProcessEventQueue()
        {
            _isProcessingQueue = true;
            
            while (true)
            {
                object eventToProcess;
                
                lock (_lock)
                {
                    if (_eventQueue.Count == 0)
                    {
                        _isProcessingQueue = false;
                        yield break;
                    }
                    
                    eventToProcess = _eventQueue.Dequeue();
                }
                
                // Process the event outside the lock to prevent deadlocks
                ProcessSingleEvent(eventToProcess);
                
                // Yield to allow other operations
                yield return null;
            }
        }

        /// <summary>
        /// Processes a single event and notifies all relevant listeners.
        /// </summary>
        /// <param name="eventObj">The event object to process.</param>
        private static void ProcessSingleEvent(object eventObj)
        {
            var eventType = eventObj.GetType();
            
            if (_enableLogging)
            {
                Debug.Log($"[EventManager] Processing event: {eventType.Name}");
            }
            
            _totalEventsTriggered++;
            
            // Notify regular subscribers
            NotifyListeners(eventObj, eventType, _subscribersList);
            
            // Notify one-time subscribers and clear them
            NotifyListeners(eventObj, eventType, _oneTimeSubscribersList);
            
            lock (_lock)
            {
                if (_oneTimeSubscribersList.TryGetValue(eventType, out var value))
                {
                    value.Clear();
                }
            }
        }

        /// <summary>
        /// Notifies all listeners in the specified list about the event.
        /// </summary>
        /// <param name="eventObj">The event object.</param>
        /// <param name="eventType">The type of the event.</param>
        /// <param name="subscribersList">The list of subscribers to notify.</param>
        private static void NotifyListeners(object eventObj, Type eventType, Dictionary<Type, List<WeakReference<IEventListener>>> subscribersList)
        {
            if (!subscribersList.ContainsKey(eventType)) return;
            
            var listenersToNotify = new List<IEventListener>();
            
            // Collect valid listeners
            lock (_lock)
            {
                foreach (var weakRef in subscribersList[eventType])
                {
                    if (weakRef.TryGetTarget(out var listener))
                    {
                        listenersToNotify.Add(listener);
                    }
                }
            }
            
            // Notify listeners outside the lock
            foreach (var listener in listenersToNotify)
            {
                try
                {
                    // Use reflection to call the generic method efficiently
                    var method = listener.GetType().GetMethod("OnEventTriggered", new[] { eventType });
                    if (method == null) continue;
                    method.Invoke(listener, new[] { eventObj });
                    _totalListenersNotified++;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventManager] Error in listener {listener.GetType().Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Cleans up dead references from a subscriber list.
        /// </summary>
        /// <param name="subscribersList">The list to clean up.</param>
        private static void CleanupList(Dictionary<Type, List<WeakReference<IEventListener>>> subscribersList)
        {
            foreach (var kvp in subscribersList.ToList())
            {
                var type = kvp.Key;
                var list = kvp.Value;
                
                // Remove dead references
                list.RemoveAll(weakRef => !weakRef.TryGetTarget(out _));
                
                // Remove empty lists
                if (list.Count == 0)
                {
                    subscribersList.Remove(type);
                }
            }
        }
    }
}
