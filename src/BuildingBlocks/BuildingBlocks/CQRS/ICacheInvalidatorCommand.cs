namespace BuildingBlocks.CQRS;

public interface ICacheInvalidatorCommand
{
    string[] CacheKeys { get; }
}