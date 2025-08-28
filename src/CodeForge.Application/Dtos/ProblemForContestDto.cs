using Codeforge.Domain.Constants;

namespace Codeforge.Application.Dtos;

public class ProblemForContestDto {
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public Difficulty Difficulty { get; set; }
}