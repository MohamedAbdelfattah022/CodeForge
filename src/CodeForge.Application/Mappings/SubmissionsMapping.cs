using Codeforge.Application.Dtos;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Mappings;

public static class SubmissionsMapping {
	public static SubmissionDto ToDto(this Submission submission) {
		return new SubmissionDto
			{
				Id = submission.Id,
				Code = submission.Code,
				Language = submission.Language,
				SubmittedAt = submission.SubmittedAt,
				ExecutionTime = submission.ExecutionTime ?? 0,
				MemoryUsed = submission.MemoryUsed ?? 0,
				Verdict = submission.Verdict.ToString()
			};
	}

	public static SubmissionMetadata ToMetadata(this Submission submission) {
		return new SubmissionMetadata
			{
				Id = submission.Id,
				Language = submission.Language,
				SubmittedAt = submission.SubmittedAt,
				Verdict = submission.Verdict.ToString()
			};
	}
}