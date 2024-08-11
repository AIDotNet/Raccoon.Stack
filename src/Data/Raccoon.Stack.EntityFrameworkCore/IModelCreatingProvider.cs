using Microsoft.EntityFrameworkCore;

namespace Raccoon.Stack.EntityFrameworkCore;

public interface IModelCreatingProvider
{
    /// <summary>
    /// For building DbContext models and their mappings
    /// </summary>
    /// <param name="modelBuilder"></param>
    void Configure(ModelBuilder modelBuilder);
}
