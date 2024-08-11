namespace Raccoon.Stack.Ddd.Domain.Entities;

public interface IAggregateRoot : IEntity
{

}

public interface IAggregateRoot<out TKey> : IEntity<TKey>, IAggregateRoot
{

}
