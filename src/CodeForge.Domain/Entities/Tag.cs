namespace Codeforge.Domain.Entities;

public sealed class Tag : BaseEntity {
	public string Name { get; set; }

	public ICollection<Problem> Problems { get; set; } = new List<Problem>();
}