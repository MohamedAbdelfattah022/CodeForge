using CodeForge.Domain.Constants;

namespace CodeForge.Domain.Entities;

public class Problem {
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public Difficulty Difficulty { get; set; }
	public string Constraints { get; set; } = string.Empty;
	
	public virtual ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
	public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
	public virtual ICollection<ProblemTag> ProblemTags { get; set; } = new List<ProblemTag>();
}