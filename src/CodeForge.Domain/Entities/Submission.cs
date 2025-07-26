using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CodeForge.Domain.Constants;

namespace CodeForge.Domain.Entities;

public class Submission {
	public int Id { get; set; }
	public int ProblemId { get; set; }
	public string UserId { get; set; }
	public string Code { get; set; } = string.Empty;
	public string Language { get; set; } = string.Empty;
	public Verdict Verdict { get; set; } = Verdict.Pending;
	public int? ExecutionTime { get; set; }
	public int? MemoryUsed { get; set; }
	public DateTime? SubmittedAt { get; set; }

	public virtual Problem Problem { get; set; }
	public virtual User User { get; set; }
	public virtual ICollection<SubmissionTestCaseResult> SubmissionTestCaseResults { get; set; } = new List<SubmissionTestCaseResult>();
}