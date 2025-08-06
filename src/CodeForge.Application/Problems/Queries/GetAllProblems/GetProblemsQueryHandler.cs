using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Application.Shared;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Problems.Queries.GetAllProblems;

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
			results,
			count,
			request.PageNumber,
			request.PageSize
		);
	}
}