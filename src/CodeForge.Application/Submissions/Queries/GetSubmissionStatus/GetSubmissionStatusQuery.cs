using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Submissions.Queries.GetSubmissionStatus;

public class GetSubmissionStatusQuery(int submissionId) : IRequest<SubmissionStatusDto> {
	public int SubmissionId { get; set; } = submissionId;
}