namespace Codeforge.Domain.Interfaces;

public interface IRedisCacheService {
	Task<T?> GetAsync<T>(string key);
	Task SetAsync<T>(string key, T data, TimeSpan? absoluteExpiration = null);
	Task RemoveAsync(string key);
}