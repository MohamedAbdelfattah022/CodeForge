using CodeForge.Domain.Constants;

namespace CodeForge.Domain.Entities;

public sealed class Problem {
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public Difficulty Difficulty { get; set; }
	public string Constraints { get; set; } = string.Empty;
	
	public ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
	public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
	public ICollection<ProblemTag> ProblemTags { get; set; } = new List<ProblemTag>();
}