using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CodeForge.Domain.Constants;

namespace CodeForge.Domain.Entities;

public class SubmissionTestCaseResult {
	public int Id { get; set; }
	public int SubmissionId { get; set; }
	public int TestCaseId { get; set; }
	public Verdict Verdict { get; set; }
	public int? ExecutionTime { get; set; }
	public int? MemoryUsed { get; set; }

	public virtual Submission Submission { get; set; }
	public virtual TestCase TestCase { get; set; }
}