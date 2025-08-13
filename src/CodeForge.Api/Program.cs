using Codeforge.Api.Extensions;
using Codeforge.Api.Middlewares;
using Codeforge.Application.Extentions;
using Codeforge.Application.Submissions.Services;
using Codeforge.Domain.Entities;
using Codeforge.Infrastructure.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapGroup("/api").MapIdentityApi<User>()
	.WithTags("Identity");

app.UseAuthorization();
app.MapControllers();

app.Run();