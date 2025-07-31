using CodeForge.Domain.Entities;
using CodeForge.Domain.Exceptions;
using CodeForge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeForge.Application.Problems.Commands.UpdateProblem;

public class UpdateProblemCommandHandler(
	ILogger<UpdateProblemCommandHandler> logger,
	IProblemsRepository problemsRepository) : IRequestHandler<UpdateProblemCommand> {
	public async Task Handle(UpdateProblemCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("UpdateProblemCommandHandler.Handle called with request: {@Request}", request);
		var problem = await problemsRepository.GetByIdAsync(request.Id);

		if (problem is null) {
			logger.LogWarning("Problem with id {id} not found.", request.Id);
			throw new NotFoundException(nameof(Problem), request.Id.ToString());
		}
		
		problem.Title = request.Title ?? problem.Title;
		problem.Description = request.Description ?? problem.Description;
		problem.Constraints = request.Constraints ?? problem.Constraints;
		problem.Difficulty = request.Difficulty ?? problem.Difficulty;

		await problemsRepository.UpdateAsync(problem);
	}
}