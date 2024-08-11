namespace Raccoon.Stack.Ddd.Domain.Entities.Full;


public interface IFullEntity<out TUserId> : IAuditEntity<TUserId>, ISoftDelete
{

}

public interface IFullEntity<TKey, out TUserId> : IAuditEntity<TKey, TUserId>, ISoftDelete
{

}
