using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Raccoon.Stack.Ddd.Domain.Entities;
using Raccoon.Stack.Ddd.Domain.Repositories;

namespace Raccoon.Stack.EntityFrameworkCore;

public class RepositoryBase<TDbContext, TEntity> : IRepository<TEntity>
    where TEntity : class, IComparable, IEntity
    where TDbContext : RaccoonDbContext<TDbContext>
{
    private readonly RaccoonDbContext<TDbContext> _dbContext;

    protected RepositoryBase(RaccoonDbContext<TDbContext> dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity = (await _dbContext.AddAsync(entity, cancellationToken)).Entity;

        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddRangeAsync(entities, cancellationToken);
    }

    public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Update(entity);

        return Task.FromResult(entity);
    }

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.UpdateRange(entities);

        return Task.CompletedTask;
    }

    public Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Remove(entity);

        return Task.FromResult(entity);
    }

    public Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _dbContext.RemoveRange(entities);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        _dbContext.RemoveRange(_dbContext.Set<TEntity>().Where(predicate));

        return Task.CompletedTask;
    }

    public async Task<TEntity?> FindAsync(IEnumerable<KeyValuePair<string, object>> keyValues,
        CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Set<TEntity>().FindAsync(keyValues);

        return result;
    }

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(string sortField, bool isDescending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        query = isDescending
            ? query.OrderByDescending(x => x.GetType().GetProperty(sortField))
            : query.OrderBy(x => x.GetType().GetProperty(sortField));

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, string sortField,
        bool isDescending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<TEntity>().Where(predicate);

        query = isDescending
            ? query.OrderByDescending(x => x.GetType().GetProperty(sortField))
            : query.OrderBy(x => x.GetType().GetProperty(sortField));

        return await query.ToListAsync(cancellationToken);
    }

    public Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<TEntity>().LongCountAsync(cancellationToken);
    }

    public Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<TEntity>().LongCountAsync(predicate, cancellationToken);
    }

    public Task<List<TEntity>> GetPaginatedListAsync(int skip, int take, string sortField, bool isDescending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        query = isDescending
            ? query.OrderByDescending(x => x.GetType().GetProperty(sortField))
            : query.OrderBy(x => x.GetType().GetProperty(sortField));

        return query.Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetPaginatedListAsync(int skip, int take, Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<TEntity>().AsQueryable();

        if (sorting != null)
        {
            foreach (var (key, value) in sorting)
            {
                query = value
                    ? query.OrderByDescending(x => x.GetType().GetProperty(key))
                    : query.OrderBy(x => x.GetType().GetProperty(key));
            }
        }

        return query.Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take,
        string sortField, bool isDescending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<TEntity>().Where(predicate);

        query = isDescending
            ? query.OrderByDescending(x => x.GetType().GetProperty(sortField))
            : query.OrderBy(x => x.GetType().GetProperty(sortField));

        return query.Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take,
        Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<TEntity>().Where(predicate);

        if (sorting != null)
        {
            foreach (var (key, value) in sorting)
            {
                query = value
                    ? query.OrderByDescending(x => x.GetType().GetProperty(key))
                    : query.OrderBy(x => x.GetType().GetProperty(key));
            }
        }

        return query.Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<TEntity>> GetPaginatedListAsync(PaginatedOptions options,
        CancellationToken cancellationToken = default)
    {
        var result = await GetPaginatedListAsync(options.Page, options.PageSize, options.Sorting, cancellationToken);

        var total = await GetCountAsync(cancellationToken);

        return new PaginatedList<TEntity>()
        {
            Result = result,
            Total = total,
        };
    }

    public async Task<PaginatedList<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate,
        PaginatedOptions options,
        CancellationToken cancellationToken = default)
    {
        var result = await GetPaginatedListAsync(predicate, options.Page, options.PageSize, options.Sorting,
            cancellationToken);

        var total = await GetCountAsync(predicate, cancellationToken);

        return new PaginatedList<TEntity>()
        {
            Result = result,
            Total = total,
        };
    }
}