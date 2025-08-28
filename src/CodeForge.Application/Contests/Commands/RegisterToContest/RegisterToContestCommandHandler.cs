using Codeforge.Application.Users;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Contests.Commands.RegisterToContest;

public class RegisterToContestCommandHandler(
	ILogger<RegisterToContestCommandHandler> logger,
	IContestsRepository contestsRepository,
	IUserContext userContext,
	IUserStore<User> userStore) : IRequestHandler<RegisterToContestCommand> {
	public async Task Handle(RegisterToContestCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("RegisterToContestCommandHandler.Handle called with request: {@Request}", request);

		var currentUser = userContext.GetCurrentUser();
		if (currentUser is null) throw new UnauthorizedAccessException();

		var contest = await contestsRepository.GetByIdAsync(request.ContestId);
		if (contest is null) throw new NotFoundException(nameof(Contest), request.ContestId.ToString());

		var user = await userStore.FindByIdAsync(currentUser.Id, cancellationToken);
		if (user is null) throw new NotFoundException(nameof(User), currentUser.Id);

		if (contest.Participants.Any(u => u.Id == user.Id)) return;

		contest.Participants.Add(user);
		await contestsRepository.UpdateAsync(contest);
	}
}