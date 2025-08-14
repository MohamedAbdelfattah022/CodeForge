using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Submissions.Queries.GetUserSubmissions;

public class GetUserSubmissionsQuery(string userId) : IRequest<List<SubmissionMetadata>> {
	public string UserId { get; set; } = userId;
}