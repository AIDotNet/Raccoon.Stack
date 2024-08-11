namespace Raccoon.Stack.Ddd.Domain.Repositories;

public class PaginatedList<TEntity> : PaginatedListBase<TEntity>
    where TEntity : class, IEntity
{
}
