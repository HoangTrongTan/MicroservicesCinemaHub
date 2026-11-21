namespace BuildingBlocks.CQRS;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}