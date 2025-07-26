using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeForge.Domain.Entities;

public class TestCase {
	public int Id { get; set; }

	public int ProblemId { get; set; }

	public string Input { get; set; }

	public string ExpectedOutput { get; set; }

	public bool IsVisible { get; set; } = true;

	public virtual Problem Problem { get; set; }
	public virtual ICollection<SubmissionTestCaseResult> SubmissionTestCaseResults { get; set; } = new List<SubmissionTestCaseResult>();
}