using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Submissions.Queries.GetSubmissionById;

public class GetSubmissionByIdQuery(int subissionId, int problemId) : IRequest<SubmissionDto> {
	public int SubmissionId { get; set; } = subissionId;
	public int ProblemId { get; set; } = problemId;
}