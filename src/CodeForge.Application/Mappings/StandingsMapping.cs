using Codeforge.Application.Dtos;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Mappings;

public static class StandingsMapping {
    public static StandingDto ToDto(this Standing standing) {
        return new StandingDto {
            ContestId = standing.ContestId,
            UserName = standing.UserName,
            Score = standing.Score,
            Problems = standing.Problems?.Select(pr => pr.ToDto()).ToList() ?? []
        };
    }
}