
using Microsoft.Extensions.Caching.Distributed;

namespace BuildingBlocks.Behaviors;

public class CacheInvalidationBehavior<TRequest, TResponse>
    (IDistributedCache distributedCache, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheInvalidatorCommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next();

        foreach (var key in request.CacheKeys)
        {
            await distributedCache.RemoveAsync(key, cancellationToken);
            logger.LogInformation("[CACHE INVALIDATED] Đã xóa key khỏi Redis: {CacheKey} 🧹", key);
        }

        return response;
    }
}