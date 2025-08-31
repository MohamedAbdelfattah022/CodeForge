using Codeforge.Application.Dtos;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Mappings;

public static class ProblemResultsMapping {
    public static ProblemResultDto ToDto(this ProblemResult problemResult) {
        return new ProblemResultDto {
            ProblemId = problemResult.ProblemId
        };
    }
}