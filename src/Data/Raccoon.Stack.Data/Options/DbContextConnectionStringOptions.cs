namespace Raccoon.Stack.Data.Options;

public class DbContextConnectionStringOptions
{
    public string ConnectionString { get; set; }

    public DbContextConnectionStringOptions(string connectionString)
    {
        ConnectionString = connectionString;
    }
}
