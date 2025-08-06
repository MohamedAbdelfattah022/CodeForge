using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Problems.Queries.GetProblemById;

public class GetProblemByIdQueryHandler(
	ILogger<GetProblemByIdQueryHandler> logger,
	IProblemsRepository problemsRepository) : IRequestHandler<GetProblemByIdQuery, ProblemDto> {
	public async Task<ProblemDto> Handle(GetProblemByIdQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("GetProblemByIdQueryHandler.Handle called with request: {@Request}", request);

		if (request.Id <= 0) throw new ValidationException("ID must be positive.");

		var problem = await problemsRepository.GetByIdAsync(request.Id);

		if (problem is null) throw new NotFoundException(nameof(Problem), request.Id.ToString());

		return problem.ToDto();
	}
}