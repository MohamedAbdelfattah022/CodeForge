using Codeforge.Domain.Constants;

namespace Codeforge.Application.Dtos;

public class ContestDto {
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }
	public List<ProblemForContestDto>? Problems { get; set; } = [];
	public List<UserForContestDto>? Participants { get; set; } = [];
}