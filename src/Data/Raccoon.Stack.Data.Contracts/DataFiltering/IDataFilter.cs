namespace Raccoon.Stack.Data.Contracts.DataFiltering;

public interface IDataFilter
{
    IDisposable Enable<TFilter>() where TFilter : class;

    IDisposable Disable<TFilter>() where TFilter : class;

    bool IsEnabled<TFilter>() where TFilter : class;
}
