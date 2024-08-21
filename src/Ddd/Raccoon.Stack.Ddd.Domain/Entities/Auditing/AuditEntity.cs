namespace Raccoon.Stack.Ddd.Domain.Entities.Auditing;

public abstract class AuditEntity<TUserId> : Entity, IAuditEntity<TUserId>
{
    public TUserId Creator { get; protected set; } = default!;

    public DateTimeOffset CreationTime { get; protected set; }

    public TUserId Modifier { get; protected set; } = default!;

    public DateTimeOffset ModificationTime { get; set; }

    protected AuditEntity() => Initialize();

    private void Initialize()
    {
        this.CreationTime = this.GetCurrentTime();
        this.ModificationTime = this.GetCurrentTime();
    }

    protected virtual DateTime GetCurrentTime() => DateTime.UtcNow;
}

public abstract class AuditEntity<TKey, TUserId> : Entity<TKey>, IAuditEntity<TKey, TUserId>
{
    public TUserId Creator { get; protected set; } = default!;

    public DateTimeOffset CreationTime { get; protected set; }

    public TUserId Modifier { get; protected set; } = default!;

    public DateTimeOffset ModificationTime { get; protected set; }

    protected AuditEntity() : base()
    {
        Initialize();
    }

    protected AuditEntity(TKey id) : base(id)
    {
        Initialize();
    }

    private void Initialize()
    {
        this.CreationTime = this.GetCurrentTime();
        this.ModificationTime = this.GetCurrentTime();
    }

    protected virtual DateTimeOffset GetCurrentTime() => DateTime.UtcNow;
}