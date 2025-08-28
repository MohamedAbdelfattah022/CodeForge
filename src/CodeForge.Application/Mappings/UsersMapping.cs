using Codeforge.Application.Dtos;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Mappings;

public static class UsersMapping {
	public static UserForContestDto ToUserForContestDto(this User user) {
		return new UserForContestDto
			{
				Id = user.Id,
				UserName = user.UserName ?? string.Empty,
				Email = user.Email ?? string.Empty
			};
	}
}