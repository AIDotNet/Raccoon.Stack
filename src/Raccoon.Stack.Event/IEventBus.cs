namespace Raccoon.Stack.Event;

public interface IEventBus<in TEventBus> where TEventBus : class
{
    /// <summary>
    /// 发布分布式事件
    /// </summary>
    /// <param name="eventBus"></param>
    /// <returns></returns>
    Task PublishAsync(TEventBus eventBus);
}