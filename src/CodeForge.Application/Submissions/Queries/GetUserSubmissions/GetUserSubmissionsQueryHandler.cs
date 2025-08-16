using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Application.Users;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Queries.GetUserSubmissions;

public class GetUserSubmissionsQueryHandler(
	ILogger<GetUserSubmissionsQueryHandler> logger,
	ISubmissionsRepository submissionsRepository) : IRequestHandler<GetUserSubmissionsQuery, List<SubmissionMetadata>> {
	public async Task<List<SubmissionMetadata>> Handle(GetUserSubmissionsQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("Retrieving submissions for user {UserId}", request.UserId);

		var submissions = await submissionsRepository.GetUserSubmissionsAsync(request.UserId);

		var results = submissions.Select(s => s.ToMetadata()).ToList();
		return results;
	}
}