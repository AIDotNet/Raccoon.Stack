namespace Raccoon.Stack.Ddd.Domain.Entities.Auditing;

public interface IAuditEntity<out TUserId> : IEntity
{
    TUserId Creator { get; }

    DateTime CreationTime { get; }

    TUserId Modifier { get; }

    DateTime ModificationTime { get; }
}

public interface IAuditEntity<out TKey, out TUserId> : IAuditEntity<TUserId>, IEntity<TKey>
{
}
