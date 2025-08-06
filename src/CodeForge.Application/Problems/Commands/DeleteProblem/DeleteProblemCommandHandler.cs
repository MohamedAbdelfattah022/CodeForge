using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Problems.Commands.DeleteProblem;

public class DeleteProblemCommandHandler(
	ILogger<DeleteProblemCommandHandler> logger,
	IProblemsRepository problemsRepository) : IRequestHandler<DeleteProblemCommand> {
	public async Task Handle(DeleteProblemCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("DeleteProblemCommandHandler.Handle called with request: {@Request}", request);

		if (request.Id <= 0) throw new ValidationException("ID must be positive.");

		var problem = await problemsRepository.GetByIdAsync(request.Id);
		if (problem is null) {
			logger.LogWarning("Problem with ID {ProblemId} not found", request.Id);
			throw new NotFoundException(nameof(Problem), request.Id.ToString());
		}

		await problemsRepository.DeleteAsync(problem);
	}
}