using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Codeforge.Application.Extentions;

public static class ServiceCollectionExtensions {
	public static void AddApplication(this IServiceCollection services) {
		var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

		services.AddValidatorsFromAssembly(applicationAssembly)
			.AddFluentValidationAutoValidation(cfg => cfg.EnablePathBindingSourceAutomaticValidation = true);
	}
}