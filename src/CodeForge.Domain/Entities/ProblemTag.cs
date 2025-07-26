using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeForge.Domain.Entities;

public class ProblemTag {
	public int ProblemId { get; set; }

	public int TagId { get; set; }

	public virtual Problem Problem { get; set; }

	public virtual Tag Tag { get; set; }
}