namespace Raccoon.Stack.Ddd.Domain.Entities;

public abstract class AggregateRoot : Entity, IAggregateRoot
{
}

public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
{
    protected AggregateRoot() : base()
    {
    }

    protected AggregateRoot(TKey id) : base(id)
    {
    }
}