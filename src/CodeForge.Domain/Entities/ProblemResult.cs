using System.Text.Json.Serialization;

namespace Codeforge.Domain.Entities;

public sealed class ProblemResult : BaseEntity {
    public int StandingId { get; set; }
    public int ProblemId { get; set; }
    [JsonIgnore] public Standing Standing { get; set; }
}