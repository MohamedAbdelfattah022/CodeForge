using CodeForge.Domain.Constants;

namespace CodeForge.Domain.Entities;

public sealed class Problem : BaseEntity {
	public string Title { get; set; }
	public string Description { get; set; }
	public Difficulty Difficulty { get; set; }
	public string Constraints { get; set; }

	public ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
	public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
	public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}