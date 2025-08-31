namespace Codeforge.Application.Dtos;

public class ProblemResultDto {
    public int ProblemId { get; set; }
    public string ProblemLabel { get; set; } = string.Empty;
    public double Score { get; set; }
}