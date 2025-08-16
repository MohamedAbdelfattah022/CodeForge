using Codeforge.Domain.Entities;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;
using Codeforge.Infrastructure.Messaging;
using Codeforge.Infrastructure.Repositories;
using Codeforge.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Supabase;
using SupabaseOptions = Codeforge.Domain.Options.SupabaseOptions;

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
		services.AddScoped<ISubmissionsRepository, SubmissionsRepository>();

		services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
		services.Configure<SupabaseOptions>(configuration.GetSection(SupabaseOptions.SectionName));

		services.AddScoped<IMessageProducer, MessageProducer>();
		services.AddSingleton<IMessageConsumer, MessageConsumer>();

		var supabaseOptions = configuration.GetSection(SupabaseOptions.SectionName).Get<SupabaseOptions>() ??
		                      throw new ArgumentNullException(nameof(configuration));

		services.AddSingleton<ISupabaseService, SupabaseService>();
		services.AddSingleton<Client>(_ =>
			new Client(
				supabaseOptions.Url,
				supabaseOptions.ApiKey,
				new Supabase.SupabaseOptions
					{
						AutoConnectRealtime = true,
						AutoRefreshToken = true
					}
			)
		);
	}
}