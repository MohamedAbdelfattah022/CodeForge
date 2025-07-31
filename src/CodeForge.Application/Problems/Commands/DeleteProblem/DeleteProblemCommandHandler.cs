using CodeForge.Domain.Entities;
using CodeForge.Domain.Exceptions;
using CodeForge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeForge.Application.Problems.Commands.DeleteProblem;

public class DeleteProblemCommandHandler(
	ILogger<DeleteProblemCommandHandler> logger,
	IProblemsRepository problemsRepository) : IRequestHandler<DeleteProblemCommand> {
	public async Task Handle(DeleteProblemCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("DeleteProblemCommandHandler.Handle called with request: {@Request}", request);

		if (request.Id <= 0) throw new FluentValidation.ValidationException("ID must be positive.");

		var problem = await problemsRepository.GetByIdAsync(request.Id);
		if (problem is null) {
			logger.LogWarning("Problem with ID {ProblemId} not found", request.Id);
			throw new NotFoundException(nameof(Problem), request.Id.ToString());
		}

		await problemsRepository.DeleteAsync(problem);
	}
}