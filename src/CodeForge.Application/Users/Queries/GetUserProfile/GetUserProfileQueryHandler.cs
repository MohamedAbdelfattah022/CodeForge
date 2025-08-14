using Codeforge.Application.Dtos;
using Codeforge.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Users.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
	ILogger<GetUserProfileQueryHandler> logger,
	IUserStore<User> userStore,
	IUserContext userContext) : IRequestHandler<GetUserProfileQuery, UserProfileDto> {
	public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken) {
		var currentUser = userContext.GetCurrentUser();

		logger.LogInformation("Retrieving user:{UserID} profile", currentUser!.Id);

		var user = await userStore.FindByIdAsync(currentUser.Id, cancellationToken);

		if (user is null) throw new UnauthorizedAccessException();

		return new UserProfileDto(user.UserName!, user.Email!);
	}
}