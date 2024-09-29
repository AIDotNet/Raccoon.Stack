namespace Raccoon.Stack.Event;

/// <summary>
/// Represents the event handler.
/// </summary>
/// <typeparam name="TEvent"></typeparam>
public interface IEventHandler<in TEvent> where TEvent : class
{
    /// <summary>
    /// 事件回调处理事件
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    
}