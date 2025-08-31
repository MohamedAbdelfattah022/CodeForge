using Codeforge.Domain.Constants;

namespace Codeforge.Domain.Entities;

public sealed class Contest : BaseEntity {
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }
	public ContestStatus Status { get; set; }

	public ICollection<Problem> Problems { get; set; } = new List<Problem>();
	public ICollection<User> Participants { get; set; } = new List<User>();
	public ICollection<Standing> Standings { get; set; } = new List<Standing>();
}