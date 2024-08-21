namespace Raccoon.Stack.Data.Concurrency;

public class DefaultConcurrencyStampProvider : IConcurrencyStampProvider
{
    public string GetRowVersion() => Guid.NewGuid().ToString();
}