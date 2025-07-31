using CodeForge.Domain.Exceptions;
using FluentValidation;

namespace CodeForge.Api.Middlewares;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware {
	public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
		try {
			await next.Invoke(context);
		}
		catch (NotFoundException e) {
			logger.LogWarning("NotFoundException: {@message}", e.Message);

			context.Response.StatusCode = 404;
			await context.Response.WriteAsync(e.Message);
		}
		catch (ValidationException e) {
			logger.LogWarning("ValidationException: {@Message}", e.Errors);
			
			context.Response.StatusCode = 500;
			await context.Response.WriteAsJsonAsync(e.Message);
		}
		catch (Exception e) {
			logger.LogError(e, e.Message);

			context.Response.StatusCode = 500;
			await context.Response.WriteAsync("Internal server error.");
		}
	}
}