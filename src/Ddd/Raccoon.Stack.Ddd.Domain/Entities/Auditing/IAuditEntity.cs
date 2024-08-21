namespace Raccoon.Stack.Ddd.Domain.Entities.Auditing;

public interface IAuditEntity<out TUserId> : IEntity
{
    TUserId Creator { get; }

    DateTimeOffset CreationTime { get; }

    TUserId Modifier { get; }

    DateTimeOffset ModificationTime { get; }
}

public interface IAuditEntity<out TKey, out TUserId> : IAuditEntity<TUserId>, IEntity<TKey>
{
}
