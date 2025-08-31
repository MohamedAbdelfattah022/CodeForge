using System.Text.Json;
using Codeforge.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Codeforge.Infrastructure.Services;

public class RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer) : IRedisCacheService {
	private readonly IDatabase _redisDatabase = connectionMultiplexer.GetDatabase();

	public async Task<T?> GetAsync<T>(string key) {
		var data = await distributedCache.GetStringAsync(key);

		if (data is null) return default(T);

		Console.WriteLine("Cache hit for key: " + key);
		return JsonSerializer.Deserialize<T>(data);
	}

	public async Task SetAsync<T>(string key, T data, TimeSpan? absoluteExpiration = null) {
		if(data is null) return;
		
		var options = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = absoluteExpiration
			};

		await distributedCache.SetStringAsync(key, JsonSerializer.Serialize(data), options);
	}

	public async Task RemoveAsync(string key) {
		await distributedCache.RemoveAsync(key);
	}

	public async Task RemoveByPrefixAsync(string pattern) {
		var endpoints = _redisDatabase.Multiplexer.GetEndPoints();
		foreach (var endpoint in endpoints) {
			var server = _redisDatabase.Multiplexer.GetServer(endpoint);
			var keys = server.Keys(pattern: $"*{pattern}*").ToArray();
			foreach (var redisKey in keys) {
				await _redisDatabase.KeyDeleteAsync(redisKey);
			}
		}
	}
}