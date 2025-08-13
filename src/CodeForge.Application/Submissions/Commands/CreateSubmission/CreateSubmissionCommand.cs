using System.Text.Json.Serialization;
using Codeforge.Domain.Entities;
using MediatR;

namespace Codeforge.Application.Submissions.Commands.CreateSubmission;

// TODO: Add validation to the properties
public class CreateSubmissionCommand : IRequest<int> {
	[JsonIgnore] public int ProblemId { get; set; }
	public required string Code { get; set; }
	public required string Language { get; set; }
}

public static class CreateSubmissionCommandMapping {
	public static Submission ToSubmission(this CreateSubmissionCommand command, string userId) {
		return new Submission
			{
				UserId = userId,
				ProblemId = command.ProblemId,
				Code = command.Code,
				Language = command.Language,
				SubmittedAt = DateTime.UtcNow
			};
	}
}