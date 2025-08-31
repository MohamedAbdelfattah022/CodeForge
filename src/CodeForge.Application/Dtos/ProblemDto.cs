using Codeforge.Domain.Constants;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Dtos;

public class ProblemDto {
	public int Id { get; set; }
	public int? ContestId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public Difficulty Difficulty { get; set; }
	public string Constraints { get; set; } = string.Empty;
	public List<string>? Tags { get; set; } = [];
	public List<ProblemDtoSubmission>? Submissions { get; set; } = [];
	public List<TestCase>? TestCases { get; set; } = [];
}

public class ProblemDtoSubmission {
	public Verdict Verdict { get; set; }
	public string Language { get; set; } = string.Empty;
	public DateTime? SubmittedAt { get; set; }
}