using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Queries.GetProblemSubmissions;

public class GetProblemSubmissionsQueryHandler(
	ILogger<GetProblemSubmissionsQueryHandler> logger,
	IProblemsRepository problemsRepository,
	ISubmissionsRepository submissionsRepository) : IRequestHandler<GetProblemSubmissionsQuery, List<SubmissionMetadata>> {
	public async Task<List<SubmissionMetadata>> Handle(GetProblemSubmissionsQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("Retrieving submissions for problem {ProblemId}", request.ProblemId);

		var isProblemExists = await problemsRepository.ExistsAsync(request.ProblemId);
		if (!isProblemExists) throw new NotFoundException(nameof(Problem), request.ProblemId.ToString());

		var submissions = await submissionsRepository.GetAllSubmissons(request.ProblemId);

		var metadata = submissions.Select(s => s.ToMetadata()).ToList() ?? [];

		return metadata;
	}
}