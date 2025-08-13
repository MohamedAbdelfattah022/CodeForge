using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Queries.GetSubmissionById;

public class GetSubmissionByIdQueryHandler(
	ILogger<GetSubmissionByIdQueryHandler> logger,
	IProblemsRepository problemsRepository,
	ISubmissionsRepository submissionsRepository) : IRequestHandler<GetSubmissionByIdQuery, SubmissionDto> {
	public async Task<SubmissionDto> Handle(GetSubmissionByIdQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("Retrieving submission with id {SubmissionId} for problem {ProblemId}", request.SubmissionId, request.ProblemId);

		if (request.ProblemId <= 0 || request.SubmissionId <= 0) throw new ValidationException("Invalid problem or submission ID.");

		var isProblemExists = await problemsRepository.ExistsAsync(request.ProblemId);
		if (!isProblemExists) throw new NotFoundException(nameof(Problem), request.ProblemId.ToString());

		var submission = await submissionsRepository.GetByIdAsync(request.SubmissionId);
		if (submission is null) throw new NotFoundException(nameof(Submission), request.SubmissionId.ToString());

		return submission.ToDto();
	}
}