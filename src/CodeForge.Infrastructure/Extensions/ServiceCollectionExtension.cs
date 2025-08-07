using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;
using Codeforge.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codeforge.Infrastructure.Extensions;

public static class ServiceCollectionExtension {
	public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		services.AddDbContext<CodeforgeDbContext>(options => options.UseSqlServer(connectionString));

		services.AddIdentityApiEndpoints<User>()
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<CodeforgeDbContext>();

		services.AddScoped<IProblemsRepository, ProblemsRepository>();
		services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
		services.AddScoped<ITestcasesRepository, TestcasesRepository>();
		services.AddScoped<ITagsRepository, TagsRepository>();
	}
}