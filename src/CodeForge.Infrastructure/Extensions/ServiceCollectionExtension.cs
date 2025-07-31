using CodeForge.Domain.Entities;
using CodeForge.Domain.Repositories;
using CodeForge.Infrastructure.Contexts;
using CodeForge.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace CodeForge.Infrastructure.Extensions;

public static class ServiceCollectionExtension {
	public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		services.AddDbContext<CodeForgeDbContext>(options => options.UseSqlServer(connectionString));

		services.AddIdentityApiEndpoints<User>()
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<CodeForgeDbContext>();

		services.AddScoped<IProblemsRepository, ProblemsRepository>();
	}
}