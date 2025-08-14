using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Users.Commands.UnassignUserRole;

public class UnassignUserRoleCommandHandler(
	ILogger<UnassignUserRoleCommandHandler> logger,
	UserManager<User> userManager,
	RoleManager<IdentityRole> roleManager) : IRequestHandler<UnassignUserRoleCommand> {
	public async Task Handle(UnassignUserRoleCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("Deleting role '{Role}' from user with email '{UserEmail}'", request.RoleName, request.UserEmail);

		var user = await userManager.FindByEmailAsync(request.UserEmail);
		if (user is null) throw new NotFoundException(nameof(User), request.UserEmail);

		var role = await roleManager.FindByNameAsync(request.RoleName);
		if (role is null) throw new NotFoundException(nameof(IdentityRole), request.RoleName);

		await userManager.RemoveFromRoleAsync(user, role.Name!);
	}
}