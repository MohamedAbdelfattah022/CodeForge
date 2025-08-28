using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Contests.Queries.GetContestById;

public class GetContestByIdQueryHandler(
	ILogger<GetContestByIdQueryHandler> logger,
	IContestsRepository contestsRepository) : IRequestHandler<GetContestByIdQuery, ContestDto> {
	public async Task<ContestDto> Handle(GetContestByIdQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("GetContestByIdQueryHandler.Handle called with request: {@Request}", request);

		if (request.Id <= 0) throw new ValidationException("ID must be Greater than 0.");

		var contest = await contestsRepository.GetByIdAsync(request.Id);
		if (contest is null) throw new NotFoundException(nameof(Contest), request.Id.ToString());

		return contest.ToDto();
	}
}