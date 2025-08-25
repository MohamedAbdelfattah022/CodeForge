namespace Codeforge.Domain.Entities;

public sealed class ProblemResult : BaseEntity {
    public int StandingId { get; set; }
    public string ProblemLabel { get; set; } = string.Empty;
    public int ProblemId { get; set; }
    public double Score { get; set; }
    public Standing Standing { get; set; }
}