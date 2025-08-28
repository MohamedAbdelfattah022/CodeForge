using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Contests.Commands.DeleteContest;

public class DeleteContestCommandHandler(
	ILogger<DeleteContestCommandHandler> logger,
	IContestsRepository contestsRepository) : IRequestHandler<DeleteContestCommand> {
	public async Task Handle(DeleteContestCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("DeleteContestCommandHandler.Handle called with request: {@Request}", request);

		if (request.Id <= 0) throw new ValidationException("ID must be Greater than 0.");

		var contest = await contestsRepository.GetByIdAsync(request.Id);
		if (contest is null) throw new NotFoundException(nameof(Contest), request.Id.ToString());

		await contestsRepository.DeleteAsync(contest);
	}
}