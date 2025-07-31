# EventSystem - Improved Version

## Overview

The EventSystem is a robust, thread-safe event management system for Unity that provides type-safe event communication between components. This improved version addresses several key issues from the original implementation and adds new features for better performance, debugging, and maintainability.

## Key Improvements

### 1. Memory Leak Prevention
- **WeakReference Implementation**: Uses `WeakReference<IEventListener>` instead of direct references to prevent memory leaks when listeners are destroyed
- **Automatic Cleanup**: Provides `CleanupDeadReferences()` method to remove dead references
- **Null Safety**: Proper null checking and validation throughout the system

### 2. Thread Safety
- **Lock-based Synchronization**: All critical operations are protected with locks
- **Event Queuing**: Events are queued and processed asynchronously to prevent recursive calls
- **Safe Event Processing**: Events are processed outside of locks to prevent deadlocks

### 3. Performance Optimizations
- **Reflection-based Invocation**: Uses reflection to efficiently call event handlers without casting
- **Reduced Allocations**: Minimizes garbage collection pressure through better memory management
- **Efficient Lookups**: Optimized dictionary operations and list management

### 4. Error Handling & Debugging
- **Comprehensive Logging**: Detailed logging for all operations with configurable verbosity
- **Exception Safety**: Individual listener exceptions don't crash the entire system
- **Event Validation**: Events can implement validation logic through the `IsValid` property
- **Statistics Tracking**: Monitor system usage with event and listener counts

### 5. Enhanced Interface
- **Event Validation**: Added `EventId` and `IsValid` properties to `IEvent` interface
- **Better Documentation**: Comprehensive XML documentation for all public APIs
- **Convenience Methods**: Static `Trigger()` methods for easy event triggering

## Usage Examples

### Basic Event Creation
```csharp
public struct PlayerHealthEvent : IEvent
{
    public int CurrentHealth { get; }
    public int MaxHealth { get; }

    public PlayerHealthEvent(int currentHealth, int maxHealth)
    {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
    }

    public string EventId => $"PlayerHealth_{CurrentHealth}_{MaxHealth}";
    public bool IsValid => CurrentHealth >= 0 && MaxHealth > 0 && CurrentHealth <= MaxHealth;
}
```

### Event Listening
```csharp
public class HealthUI : MonoBehaviourEventListener<PlayerHealthEvent>
{
    public override void OnEventTriggered(PlayerHealthEvent e)
    {
        // Update UI with new health values
        UpdateHealthBar(e.CurrentHealth, e.MaxHealth);
    }
}
```

### Event Triggering
```csharp
// Using convenience method
StringEvent.Trigger("PlayerJump");

// Using EventManager directly
EventManager.TriggerEvent(new PlayerHealthEvent(100, 50));
```

### One-time Subscriptions
```csharp
// Subscribe to an event that should only be handled once
EventManager.SubscribeOnce<GameInitializedEvent>(this);
```

## API Reference

### EventManager

#### Core Methods
- `Subscribe<T>(IEventListener<T> listener)` - Subscribe to events
- `Unsubscribe<T>(IEventListener<T> listener)` - Unsubscribe from events
- `SubscribeOnce<T>(IEventListener<T> listener)` - Subscribe for one-time handling
- `TriggerEvent<T>(T eventRaiser)` - Trigger an event

#### Utility Methods
- `SetLoggingEnabled(bool enabled)` - Enable/disable debug logging
- `GetStatistics()` - Get usage statistics
- `CleanupDeadReferences()` - Remove dead references

### IEvent Interface
- `EventId` - Unique identifier for the event
- `IsValid` - Whether the event is valid and can be processed

### IEventListener Interface
- `OnEventTriggered(T e)` - Called when an event is triggered

## Best Practices

### 1. Event Design
- Make events structs for better performance
- Implement meaningful `EventId` and `IsValid` properties
- Keep events immutable to prevent issues with queuing

### 2. Listener Management
- Use `MonoBehaviourEventListener<T>` for automatic subscription management
- Always unsubscribe in `OnDestroy()` for manual subscriptions
- Handle exceptions gracefully in event handlers

### 3. Performance
- Call `CleanupDeadReferences()` periodically (e.g., every 30 seconds)
- Keep event handling logic lightweight
- Use one-time subscriptions for initialization events

### 4. Debugging
- Enable logging during development
- Monitor statistics to identify performance issues
- Use meaningful event names and IDs

## Migration from Original Version

The improved EventSystem maintains backward compatibility with the original API. Key changes:

1. **Automatic Memory Management**: No need to manually manage subscriptions in most cases
2. **Better Error Handling**: Exceptions in listeners won't crash the system
3. **Enhanced Logging**: More detailed debug information available
4. **Thread Safety**: Safe to use from multiple threads

## Example Implementation

See `EventSystemExample.cs` for a complete example demonstrating:
- Custom event creation
- Multiple event types
- Error handling
- Performance monitoring
- One-time subscriptions

## Performance Considerations

- **Memory**: WeakReferences prevent memory leaks but add slight overhead
- **Threading**: Lock-based synchronization adds minimal overhead
- **Reflection**: Used sparingly and cached for efficiency
- **Cleanup**: Periodic cleanup prevents accumulation of dead references

## Troubleshooting

### Common Issues
1. **Events not triggering**: Check if listeners are properly subscribed
2. **Memory leaks**: Ensure periodic cleanup is called
3. **Performance issues**: Monitor statistics and optimize event handling
4. **Threading issues**: All operations are thread-safe by default

### Debug Tips
- Enable logging to track event flow
- Use statistics to identify bottlenecks
- Check event validation for invalid events
- Monitor cleanup frequency and effectiveness 