using Microsoft.Extensions.Caching.Hybrid;

namespace Arcora.Api.Services.Implementations
{
    internal static class HybridCacheCompatibilityExtensions
    {
        public static TItem? Get<TItem>(this HybridCache cache, string key)
        {
            return cache.GetOrCreateAsync(
                key,
                static _ => ValueTask.FromResult(default(TItem)),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(1) })
                .AsTask()
                .GetAwaiter()
                .GetResult();
        }

        public static TItem Set<TItem>(this HybridCache cache, string key, TItem value, DateTime absoluteExpiration)
        {
            var expiration = absoluteExpiration.ToUniversalTime() - DateTime.UtcNow;
            if (expiration < TimeSpan.Zero)
                expiration = TimeSpan.Zero;

            cache.SetAsync(key, value, new HybridCacheEntryOptions { Expiration = expiration })
                .AsTask()
                .GetAwaiter()
                .GetResult();

            return value;
        }

        public static TItem Set<TItem>(this HybridCache cache, string key, TItem value, DateTimeOffset absoluteExpiration)
        {
            var expiration = absoluteExpiration.UtcDateTime - DateTime.UtcNow;
            if (expiration < TimeSpan.Zero)
                expiration = TimeSpan.Zero;

            cache.SetAsync(key, value, new HybridCacheEntryOptions { Expiration = expiration })
                .AsTask()
                .GetAwaiter()
                .GetResult();

            return value;
        }

        public static void Remove(this HybridCache cache, string key)
        {
            cache.RemoveAsync(key)
                .AsTask()
                .GetAwaiter()
                .GetResult();
        }
    }
}
