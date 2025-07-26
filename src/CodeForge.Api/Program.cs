using CodeForge.Domain.Entities;
using CodeForge.Infrastructure.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.MapIdentityApi<User>()
	.WithTags("Identity");

app.UseAuthorization();

app.MapControllers();

app.Run();