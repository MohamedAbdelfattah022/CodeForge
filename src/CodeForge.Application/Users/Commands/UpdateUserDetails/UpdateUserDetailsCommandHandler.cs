using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Users.Commands.UpdateUserDetails;

public class UpdateUserDetailsCommandHandler(
	ILogger<UpdateUserDetailsCommandHandler> logger,
	IUserContext userContext,
	IUserStore<User> userStore) : IRequestHandler<UpdateUserDetailsCommand> {
	public async Task Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken) {
		var user = userContext.GetCurrentUser();

		logger.LogInformation("Updating user {UserId} with {@Request}", user!.Id, request);

		var existingUser = await userStore.FindByIdAsync(user.Id, cancellationToken);

		if (existingUser is null) throw new NotFoundException(nameof(User), user.Id);

		existingUser.UserName = request.UserName ?? existingUser.UserName;
		existingUser.NormalizedUserName = request.UserName?.ToUpperInvariant() ?? existingUser.NormalizedUserName;
		existingUser.Email = request.Email ?? existingUser.Email;
		existingUser.NormalizedEmail = request.Email?.ToUpperInvariant() ?? existingUser.NormalizedEmail;

		await userStore.UpdateAsync(existingUser, cancellationToken);
	}
}