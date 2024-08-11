namespace Raccoon.Stack.Ddd.Domain.Entities;

public interface IEntity
{
    IEnumerable<(string Name, object Value)> GetKeys();
}

public interface IEntity<out TKey> : IEntity
{
    TKey Id { get; }
}
