using System.ComponentModel.DataAnnotations;

namespace CodeForge.Domain.Entities;

public sealed class Tag {
	public int Id { get; set; }
	public string Name { get; set; }

	public ICollection<ProblemTag> ProblemTags { get; set; } = new List<ProblemTag>();
}