using System.Text.Json;
using Codeforge.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Codeforge.Infrastructure.Services;

public class RedisCacheService(IDistributedCache distributedCache) : IRedisCacheService {
	public async Task<T?> GetAsync<T>(string key) {
		var data = await distributedCache.GetStringAsync(key);

		if (data is null) return default(T);

		return JsonSerializer.Deserialize<T>(data);
	}

	public async Task SetAsync<T>(string key, T data, TimeSpan? absoluteExpiration = null) {
		var options = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = absoluteExpiration
			};

		await distributedCache.SetStringAsync(key, JsonSerializer.Serialize(data), options);
	}

	public async Task RemoveAsync(string key) {
		await distributedCache.RemoveAsync(key);
	}
}