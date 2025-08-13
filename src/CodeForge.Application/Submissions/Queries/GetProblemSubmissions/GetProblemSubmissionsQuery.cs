using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Submissions.Queries.GetProblemSubmissions;

public class GetProblemSubmissionsQuery(int problemId) : IRequest<List<SubmissionMetadata>> {
	public int ProblemId { get; set; } = problemId;
}