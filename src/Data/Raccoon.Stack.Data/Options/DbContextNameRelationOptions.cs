namespace Raccoon.Stack.Data.Options;

internal class DbContextNameRelationOptions
{
    public string Name { get; }

    public Type DbContextType { get; }

    public DbContextNameRelationOptions(string name, Type dbContextType)
    {
        Name = name;
        DbContextType = dbContextType;
    }
}
