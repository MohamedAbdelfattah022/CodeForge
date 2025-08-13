using System.Text.Json.Serialization;

namespace Codeforge.Domain.Entities;

// TODO: rename input and output fields with url suffixes to avoid confusion
public sealed class TestCase : BaseEntity {
	public int ProblemId { get; set; }

	public required string Input { get; set; }

	public required string ExpectedOutput { get; set; }

	public bool IsVisible { get; set; } = true;

	[JsonIgnore] public Problem Problem { get; set; }
}