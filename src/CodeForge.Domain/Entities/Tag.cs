using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CodeForge.Domain.Entities;

public sealed class Tag {
	public int Id { get; set; }
	public string Name { get; set; }

	public ICollection<Problem> Problems { get; set; } = new List<Problem>();
}