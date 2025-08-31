using System.Text.Json.Serialization;

namespace Codeforge.Domain.Entities;

public sealed class Standing : BaseEntity {
    public int ContestId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public double Score { get; set; }
    [JsonIgnore]
    public ICollection<ProblemResult> Problems { get; set; } = new List<ProblemResult>();
    public Contest Contest { get; set; }
}