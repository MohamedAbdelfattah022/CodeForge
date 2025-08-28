using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Application.Shared;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Contests.Queries.GetAllContests;

public class GetContestsQueryHandler(
	ILogger<GetContestsQueryHandler> logger,
	IContestsRepository contestsRepository) : IRequestHandler<GetContestsQuery, PaginationResult<ContestDto>> {
	public async Task<PaginationResult<ContestDto>> Handle(GetContestsQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("GetContestsQueryHandler.Handle");

		var (contests, count) = await contestsRepository.GetAllAsync(request.PageNumber, request.PageSize);
		if (contests is null) throw new NotFoundException(nameof(Contest));

		var results = contests.Select(c => c.ToDto()).ToList();

		return new PaginationResult<ContestDto>(
			results,
			count,
			request.PageNumber,
			request.PageSize
		);
	}
}