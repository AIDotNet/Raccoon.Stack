using Raccoon.Stack.Data.Options;

namespace Raccoon.Stack.Data.Uow;

public interface IUnitOfWorkManager
{
    /// <summary>
    /// Create new DbContext
    /// We create DbContext with lazy loading enabled by default
    /// </summary>
    /// <param name="lazyLoading">Deferred creation of DbContext, easy to specify tenant or environment by yourself, which is very effective for physical isolation</param>
    /// <returns></returns>
    IUnitOfWork CreateDbContext(bool lazyLoading = true);

    IUnitOfWork CreateDbContext(DbContextConnectionStringOptions connectionStringOptions);
}
