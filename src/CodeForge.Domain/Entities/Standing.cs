namespace Codeforge.Domain.Entities;

public sealed class Standing : BaseEntity {
    public int ContestId { get; set; }
    public int Rank { get; set; }
    public string UserName { get; set; } = string.Empty;
    public double Score { get; set; }
    public ICollection<ProblemResult> Problems { get; set; } = new List<ProblemResult>();
    public Contest Contest { get; set; }
}