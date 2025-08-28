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
				Status = contest.Status
			};
	}
}