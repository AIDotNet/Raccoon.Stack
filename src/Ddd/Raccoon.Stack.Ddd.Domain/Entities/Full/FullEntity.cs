namespace Raccoon.Stack.Ddd.Domain.Entities.Full;

public abstract class FullEntity<TUserId>
    : AuditEntity<TUserId>, IFullEntity<TUserId>
{
    public bool IsDeleted { get; protected set; }
}

public abstract class FullEntity<TKey, TUserId>
    : AuditEntity<TKey, TUserId>, IFullEntity<TKey, TUserId>
{
    public bool IsDeleted { get; protected set; }

    protected FullEntity() : base()
    {
    }

    protected FullEntity(TKey id) : base(id)
    {
    }
}
