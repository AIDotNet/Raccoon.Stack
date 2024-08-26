using Raccoon.Stack.Data.Options;
using Raccoon.Stack.Data.Uow;

namespace Raccoon.Stack.Uow.EntityFrameworkCore;

public class UnitOfWorkAccessor : IUnitOfWorkAccessor
{
    public RaccoonDbContextConfigurationOptions CurrentDbContextOptions { get; set; }

    public UnitOfWorkAccessor()
    {
        CurrentDbContextOptions = new();
    }
}