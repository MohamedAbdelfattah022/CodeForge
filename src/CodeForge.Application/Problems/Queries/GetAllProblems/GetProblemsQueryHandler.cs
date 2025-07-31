using CodeForge.Application.Dtos;
using CodeForge.Application.Mappings;
using CodeForge.Application.Shared;
using CodeForge.Domain.Entities;
using CodeForge.Domain.Exceptions;
using CodeForge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeForge.Application.Problems.Queries.GetAllProblems;

public class GetProblemsQueryHandler(
	ILogger<GetProblemsQueryHandler> logger,
	IProblemsRepository problemsRepository) : IRequestHandler<GetProblemsQuery, PaginationResult<ProblemDto>> {
	public async Task<PaginationResult<ProblemDto>> Handle(GetProblemsQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("GetProblemsQueryHandler.Handle");

		var (problems, count) = await problemsRepository.GetAllAsync(request.PageNumber, request.PageSize);

		if (problems is null) throw new NotFoundException(nameof(Problem));

		var results = problems.Select(p => p.ToDto()).ToList();

		return new PaginationResult<ProblemDto>
		(
			data: results,
			totalItems: count,
			pageNumber: request.PageNumber,
			pageSize: request.PageSize
		);
	}
}