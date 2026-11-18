using Microsoft.Extensions.Caching.Distributed;

namespace SmartPark.Data;

public sealed class ParkirisceCache(IDistributedCache cache)
{
    public const string Key = "parkirisca:v1";

    public Task InvalidateAsync() => cache.RemoveAsync(Key);
}
