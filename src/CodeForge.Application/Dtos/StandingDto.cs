namespace Codeforge.Application.Dtos;

public class StandingDto {
    public int ContestId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public double Score { get; set; }
    public ICollection<ProblemResultDto> Problems { get; set; } = new List<ProblemResultDto>();
}