using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Commands.RemoveTagFromProblem;

public class RemoveTagFromProblemCommandHandler(
	ILogger<RemoveTagFromProblemCommandHandler> logger,
	IProblemsRepository problemsRepository,
	ITagsRepository tagsRepository) : IRequestHandler<RemoveTagFromProblemCommand> {
	public async Task Handle(RemoveTagFromProblemCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("RemoveTagFromProblemCommandHandler.Handle called with request: {@Request}", request);

		if (request.ProblemId <= 0) throw new ValidationException("ProblemId must be a positive integer.");
		if (request.TagId <= 0) throw new ValidationException("TagId must be a positive integer.");

		var problem = await problemsRepository.GetByIdWithTagsAsync(request.ProblemId);
		if (problem is null) throw new NotFoundException(nameof(Problem), request.ProblemId.ToString());

		var tag = await tagsRepository.GetByIdAsync(request.TagId);
		if (tag is null) throw new NotFoundException(nameof(Tag), request.TagId.ToString());

		await problemsRepository.RemoveTagFromProblemAsync(problem, request.TagId);
	}
}