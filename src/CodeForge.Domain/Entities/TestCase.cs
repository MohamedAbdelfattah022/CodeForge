using System.Text.Json.Serialization;

namespace CodeForge.Domain.Entities;

public sealed class TestCase {
	public int Id { get; set; }

	public int ProblemId { get; set; }

	public string Input { get; set; }

	public string ExpectedOutput { get; set; }

	public bool IsVisible { get; set; } = true;

	[JsonIgnore] public Problem Problem { get; set; }
}