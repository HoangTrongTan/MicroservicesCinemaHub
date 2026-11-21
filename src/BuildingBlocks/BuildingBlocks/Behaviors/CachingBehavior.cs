using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BuildingBlocks.Behaviors;

public class CachingBehavior<TRequest, TResponse>
    (IDistributedCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cachedData = await cache.GetStringAsync(request.CacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedData))
        {
            logger.LogInformation("[CACHE HIT] Lấy từ Redis với key: {CacheKey} ⚡",
                request.CacheKey);

            return JsonSerializer.Deserialize<TResponse>(cachedData)!;
        }

        logger.LogInformation("[CACHE MISS] Xuống DB lấy dữ liệu cho key: {CacheKey} 🐘 ", request.CacheKey);

        var response = await next();

        if (response is not null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = request.Expiration ?? TimeSpan.FromMinutes(5)
            };

            await cache.SetStringAsync(
                request.CacheKey,
                JsonSerializer.Serialize(response),
                options,
                cancellationToken
                );
        }

        return response;
    }
}