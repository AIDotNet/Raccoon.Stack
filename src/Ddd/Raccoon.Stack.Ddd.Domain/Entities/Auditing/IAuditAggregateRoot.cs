namespace Raccoon.Stack.Ddd.Domain.Entities.Auditing;

public interface IAuditAggregateRoot<out TUserId> : IAuditEntity<TUserId>, IAggregateRoot
{

}

public interface IAuditAggregateRoot<TKey, out TUserId> : IAuditEntity<TUserId>, IAggregateRoot<TKey>
{

}
