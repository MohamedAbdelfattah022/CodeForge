using System.Text;
using Codeforge.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Codeforge.Api.Helpers;

public class CacheAttribute(int expirationInSeconds) : Attribute, IAsyncActionFilter {
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
		var redisCache = context.HttpContext.RequestServices.GetRequiredService<IRedisCacheService>();

		var key = GenerateKey(context.HttpContext.Request);

		var cachedResponse = await redisCache.GetAsync<object>(key);
		if (cachedResponse != null) {
			context.Result = new ObjectResult(cachedResponse);
			return;
		}

		var executedContext = await next();

		if (executedContext.Result is ObjectResult { Value: not null } objectResult) {
			var expiration = TimeSpan.FromSeconds(expirationInSeconds);
			await redisCache.SetAsync(key, objectResult.Value, expiration);
		}
	}

	private string GenerateKey(HttpRequest httpContextRequest) {
		var builder = new StringBuilder();

		builder.Append(httpContextRequest.Path);

		foreach (var (key, value) in httpContextRequest.Query.OrderBy(x => x.Key))
			builder.Append($"|{key}={value}");

		return builder.ToString();
	}
}