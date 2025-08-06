using Codeforge.Api.Middlewares;
using Serilog;

namespace Codeforge.Api.Extensions;

public static class WebAppBuilderExtensions {
	public static void AddPresentation(this WebApplicationBuilder builder) {
		builder.Services.AddScoped<ExceptionHandlingMiddleware>();

		builder.Host.UseSerilog((context, services, loggerConfiguration) => {
			loggerConfiguration
				.ReadFrom.Configuration(context.Configuration)
				.ReadFrom.Services(services);
		});

		builder.Services.AddControllers();
		builder.Services.AddOpenApi();
	}
}