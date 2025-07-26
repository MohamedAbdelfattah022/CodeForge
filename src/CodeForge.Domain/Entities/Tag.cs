using System.ComponentModel.DataAnnotations;

namespace CodeForge.Domain.Entities;

public class Tag {
	public int Id { get; set; }
	public string Name { get; set; }

	public virtual ICollection<ProblemTag> ProblemTags { get; set; } = new List<ProblemTag>();
}