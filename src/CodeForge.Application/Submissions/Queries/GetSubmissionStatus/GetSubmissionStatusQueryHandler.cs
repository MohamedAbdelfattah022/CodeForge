using Codeforge.Application.Dtos;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Queries.GetSubmissionStatus;

public class GetSubmissionStatusQueryHandler(
	ILogger<GetSubmissionStatusQueryHandler> logger,
	ISubmissionsRepository submissionsRepository)
	: IRequestHandler<GetSubmissionStatusQuery, SubmissionStatusDto> {
	public async Task<SubmissionStatusDto> Handle(GetSubmissionStatusQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("GetSubmissionStatusQueryHandler.Handle called with request: {@Request}", request);

		if (request.SubmissionId <= 0) throw new ValidationException("SubmissionId must be positive.");

		var submission = await submissionsRepository.GetByIdAsync(request.SubmissionId);

		if (submission is null) throw new NotFoundException(nameof(Submission), request.SubmissionId.ToString());

		return new SubmissionStatusDto
			{
				SubmissionId = submission.Id,
				OverallVerdict = submission.Verdict.ToString(),
				ExecutionTimeMs = submission.ExecutionTime ?? 0,
				UsedMemoryKb = submission.MemoryUsed ?? 0
			};
	}
}