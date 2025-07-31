using UnityEngine;
using UnityEngine.Events;

namespace KaneTemplate.EventSystem.Implements
{
    /// <summary>
    /// A MonoBehaviour component that listens for StringEvents and invokes UnityEvents when matching events are received.
    /// This component can be attached to GameObjects to easily respond to string-based events through the Unity Inspector.
    /// </summary>
    /// <remarks>
    /// <para>
    /// StringEventListener is useful for connecting the EventSystem to Unity's UI system or other
    /// components that use UnityEvents. Simply attach this component to a GameObject, set the
    /// eventName to match the StringEvent you want to listen for, and assign UnityEvents in the Inspector.
    /// </para>
    /// <para>
    /// This component automatically:
    /// - Subscribes to StringEvents when the GameObject is enabled
    /// - Unsubscribes from StringEvents when the GameObject is disabled
    /// - Filters events to only respond to those matching the specified eventName
    /// - Invokes the assigned UnityEvent when a matching event is received
    /// </para>
    /// <para>
    /// Example usage:
    /// - Set eventName to "GameStart" to listen for game start events
    /// - Set eventName to "PlayerDeath" to listen for player death events
    /// - Assign UnityEvents in the Inspector to perform actions when events are received
    /// </para>
    /// </remarks>
    public class StringEventListener : MonoBehaviourEventListener<StringEvent>
    {
        [Header("String event")]
        [Tooltip("The name of the event to listen to. Only StringEvents with this exact name will trigger the UnityEvent.")]
        public string eventName = "load";
        
        [Tooltip("The UnityEvent to invoke when a matching StringEvent is received.")]
        public UnityEvent onEventTriggered;

        /// <summary>
        /// Called when a StringEvent is triggered. Checks if the event name matches
        /// the configured eventName and invokes the UnityEvent if it does.
        /// </summary>
        /// <param name="stringEvent">The StringEvent that was triggered.</param>
        /// <remarks>
        /// This method filters events by name to ensure only relevant events trigger
        /// the UnityEvent. The comparison is case-sensitive.
        /// </remarks>
        public override void OnEventTriggered(StringEvent stringEvent)
        {
            if (stringEvent.Name == eventName)
            {
                onEventTriggered?.Invoke();
            }
        }
    }
}