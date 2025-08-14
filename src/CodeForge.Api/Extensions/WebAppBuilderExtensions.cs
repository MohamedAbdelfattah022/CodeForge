using Codeforge.Api.Middlewares;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Codeforge.Api.Extensions;

public static class WebAppBuilderExtensions {
	public static void AddPresentation(this WebApplicationBuilder builder) {
		builder.Services.AddAuthentication();
		
		builder.Services.AddScoped<ExceptionHandlingMiddleware>();

		builder.Host.UseSerilog((context, services, loggerConfiguration) => {
			loggerConfiguration
				.ReadFrom.Configuration(context.Configuration)
				.ReadFrom.Services(services);
		});

		builder.Services.AddControllers();
		builder.Services.AddOpenApi();
		builder.Services.AddSwaggerGen(o => {
			o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer"
				});

			o.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
							{
								Reference = new OpenApiReference
									{
										Type = ReferenceType.SecurityScheme,
										Id = "Bearer"
									}
							},
							[]
					}
				});
		});
		
	}
}