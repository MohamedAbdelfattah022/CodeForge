using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeForge.Domain.Entities;

public sealed class ProblemTag {
	public int ProblemId { get; set; }

	public int TagId { get; set; }

	public Problem Problem { get; set; }

	public Tag Tag { get; set; }
}