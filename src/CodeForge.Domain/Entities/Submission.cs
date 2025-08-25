using System.Text.Json.Serialization;
using Codeforge.Domain.Constants;

namespace Codeforge.Domain.Entities;

public sealed class Submission : BaseEntity {
	public int ProblemId { get; set; }
	public string UserId { get; set; }
	public string Code { get; set; } = string.Empty;
	public string Language { get; set; } = string.Empty;
	public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
	public Verdict Verdict { get; set; } = Verdict.Pending;
	public int? ExecutionTime { get; set; }
	public int? MemoryUsed { get; set; }
	public int? ContestId { get; set; }
	public Contest? Contest { get; set; }
	[JsonIgnore] public Problem Problem { get; set; }
	public User User { get; set; }
}