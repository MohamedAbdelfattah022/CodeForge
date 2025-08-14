using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Users.Commands.AssignUserRole;

public class AssignUserRoleCommandHandler(
	ILogger<AssignUserRoleCommandHandler> logger,
	UserManager<User> userManager,
	RoleManager<IdentityRole> roleManager) : IRequestHandler<AssignUserRoleCommand> {
	public async Task Handle(AssignUserRoleCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("Assign user {UserEmail} to {RoleName} Role", request.UserEmail, request.RoleName);

		var user = await userManager.FindByEmailAsync(request.UserEmail);
		if (user is null) throw new NotFoundException(nameof(User), request.UserEmail);

		var role = await roleManager.FindByNameAsync(request.RoleName);
		if (role is null) throw new NotFoundException(nameof(IdentityRole), request.RoleName);

		await userManager.AddToRoleAsync(user, role.Name!);
	}
}