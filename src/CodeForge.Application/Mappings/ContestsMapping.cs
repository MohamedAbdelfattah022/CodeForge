using Codeforge.Application.Dtos;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Mappings;

public static class ContestsMapping {
	public static ContestDto ToDto(this Contest contest) {
		return new ContestDto
			{
				Id = contest.Id,
				Name = contest.Name,
				Description = contest.Description,
				StartTime = contest.StartTime,
				EndTime = contest.EndTime,
				Status = contest.Status,
				Problems = contest.Problems?.Select(p => p.ToProblemForContestDto()).ToList() ?? [],
				Participants = contest.Participants?.Select(u => u.ToUserForContestDto()).ToList() ?? []
			};
	}

	public static Contest ToEntity(this ContestDto dto) {
		return new Contest
			{
				Id = dto.Id,
				Name = dto.Name,
				Description = dto.Description,
				StartTime = dto.StartTime,
				EndTime = dto.EndTime,
				Status = dto.Status
			};
	}
}