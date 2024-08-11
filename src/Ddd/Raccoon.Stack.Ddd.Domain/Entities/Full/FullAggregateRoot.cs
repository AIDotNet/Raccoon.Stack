namespace Raccoon.Stack.Ddd.Domain.Entities.Full;

public abstract class FullAggregateRoot<TUserId>
    : AuditAggregateRoot<TUserId>, IFullAggregateRoot<TUserId>
{
    public bool IsDeleted { get; protected set; }
}

public abstract class FullAggregateRoot<TKey, TUserId>
    : AuditAggregateRoot<TKey, TUserId>, IFullAggregateRoot<TKey, TUserId>
{
    public bool IsDeleted { get; protected set; }

    protected FullAggregateRoot() : base()
    {
    }

    protected FullAggregateRoot(TKey id) : base(id)
    {
    }
}
