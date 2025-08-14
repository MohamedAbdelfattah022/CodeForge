using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler(
	ILogger<DeleteUserCommandHandler> logger,
	IUserContext userContext,
	IUserStore<User> userStore
) : IRequestHandler<DeleteUserCommand> {
	public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken) {
		var currentUser = userContext.GetCurrentUser();

		logger.LogInformation("Deleting user {UserId}", currentUser!.Id);

		var existingUser = await userStore.FindByIdAsync(currentUser.Id, cancellationToken);
		if (existingUser is null) throw new NotFoundException(nameof(User), currentUser.Id);

		await userStore.DeleteAsync(existingUser, cancellationToken);
	}
}