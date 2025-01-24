using System;
using System.Text.Json;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class ResponseCacheService(IConnectionMultiplexer redis) : IResponseCahceService
{
    private readonly IDatabase _database = redis.GetDatabase(1);

    //response cachen in de redis database
    public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
    {
        var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

        var serializedResponse = JsonSerializer.Serialize(response, options);

        await _database.StringSetAsync(cacheKey, serializedResponse, timeToLive);
    }

    //gecachede waarden ophalen uit de redis database
    public async Task<string?> GetCachedResponseAsync(string cacheKey)
    {
        var cachedResponse = await _database.StringGetAsync(cacheKey);

        if(cachedResponse.IsNullOrEmpty) return null;

        return cachedResponse;
    }

    //redis cache pad verwijderen
    public async Task RemoveCacheByPattern(string pattern)
    {
        var server = redis.GetServer(redis.GetEndPoints().First());
        var keys = server.Keys(database: 1, pattern:$"*{pattern}*").ToArray();

        if(keys.Length != 0)
        {
            await _database.KeyDeleteAsync(keys);
        }
    }
}
