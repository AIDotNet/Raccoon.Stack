namespace Raccoon.Stack.Ddd.Domain.Repositories;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class PaginatedListBase<TEntity>
    where TEntity : class
{
    public long Total { get; set; }

    public int TotalPages { get; set; }

    public List<TEntity> Result { get; set; } = default!;
}
