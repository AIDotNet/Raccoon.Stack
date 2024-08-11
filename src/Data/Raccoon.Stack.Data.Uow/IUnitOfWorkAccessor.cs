using Raccoon.Stack.Data.Options;

namespace Raccoon.Stack.Data.Uow;

public interface IUnitOfWorkAccessor
{
    RaccoonDbContextConfigurationOptions CurrentDbContextOptions { get; set; }
}