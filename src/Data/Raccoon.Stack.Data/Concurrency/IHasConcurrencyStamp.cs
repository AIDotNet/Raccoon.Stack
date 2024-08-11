namespace Raccoon.Stack.Data.Concurrency;

public interface IHasConcurrencyStamp
{
    string RowVersion { get; }
}
