namespace Raccoon.Stack.Service.MinimalAPIs.Core;

/// <summary>
/// 用于标记服务接口
/// </summary>
public interface IMinimalAPIService
{
    TService GetService<TService>() where TService : class;
    
    TService GetRequiredService<TService>() where TService : class;
}