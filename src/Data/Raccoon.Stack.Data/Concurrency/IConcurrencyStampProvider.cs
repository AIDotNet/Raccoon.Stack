namespace Raccoon.Stack.Data.Concurrency;

public interface IConcurrencyStampProvider
{
    string GetRowVersion();
}
