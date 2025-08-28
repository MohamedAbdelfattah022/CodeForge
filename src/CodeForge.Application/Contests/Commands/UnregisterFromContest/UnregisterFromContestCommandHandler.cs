using Codeforge.Application.Users;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Contests.Commands.UnregisterFromContest;

public class UnregisterFromContestCommandHandler(
	ILogger<UnregisterFromContestCommandHandler> logger,
	IContestsRepository contestsRepository,
	IUserContext userContext,
	IUserStore<User> userStore) : IRequestHandler<UnregisterFromContestCommand> {
	public async Task Handle(UnregisterFromContestCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("UnregisterFromContestCommandHandler.Handle called with request: {@Request}", request);

		var currentUser = userContext.GetCurrentUser();
		if (currentUser is null) throw new UnauthorizedAccessException();

		var contest = await contestsRepository.GetByIdAsync(request.ContestId);
		if (contest is null) throw new NotFoundException(nameof(Contest), request.ContestId.ToString());

		var user = await userStore.FindByIdAsync(currentUser.Id, cancellationToken);
		if (user is null) throw new NotFoundException(nameof(User), currentUser.Id);

		var participant = contest.Participants.FirstOrDefault(u => u.Id == user.Id);
		if (participant is null) return;

		contest.Participants.Remove(participant);
		await contestsRepository.UpdateAsync(contest);
	}
}