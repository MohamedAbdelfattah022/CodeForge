using Codeforge.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Codeforge.Api.Helpers;

public class InvalidateCacheAttribute(string pattern) : Attribute, IAsyncActionFilter {
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
		var resultContext = await next();
		if (resultContext.Exception == null || resultContext.ExceptionHandled) {
			var cacheService = context.HttpContext.RequestServices
				.GetRequiredService<IRedisCacheService>();

			await cacheService.RemoveByPrefixAsync(pattern);
		}
	}
}