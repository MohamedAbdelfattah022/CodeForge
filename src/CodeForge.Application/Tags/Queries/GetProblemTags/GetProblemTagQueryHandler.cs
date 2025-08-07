using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Queries.GetProblemTags;

public class GetProblemTagQueryHandler(
	ILogger<GetProblemTagQueryHandler> logger,
	IProblemsRepository problemsRepository) : IRequestHandler<GetProblemTagQuery, List<TagDto>> {
	public async Task<List<TagDto>> Handle(GetProblemTagQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("Retrieving Tags for Problem {ProblemId}", request.Id);

		if (request.Id <= 0) throw new ValidationException("Problem Id must be greater than zero.");

		var problem = await problemsRepository.GetByIdAsync(request.Id);
		if (problem is null) throw new NotFoundException(nameof(Problem), request.Id.ToString());

		var results = problem.Tags.Select(t => t.ToDto()).ToList() ?? [];

		return results;
	}
}