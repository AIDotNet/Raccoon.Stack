namespace Raccoon.Stack.Ddd.Domain.Entities.Full;

public interface IFullAggregateRoot<out TUserId> : IFullEntity<TUserId>, IAuditAggregateRoot<TUserId>
{

}

public interface IFullAggregateRoot<TKey, out TUserId> : IFullEntity<TKey, TUserId>, IAuditAggregateRoot<TKey, TUserId>
{

}
