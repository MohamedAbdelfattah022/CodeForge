using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Codeforge.Application.Users;

public interface IUserContext {
	CurrentUser? GetCurrentUser();
}

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext {
	public CurrentUser? GetCurrentUser() {
		var user = httpContextAccessor.HttpContext?.User;
		if (user is null || (!user.Identity?.IsAuthenticated ?? true)) return null;

		var id = user.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)!.Value;
		var email = user.FindFirst(claim => claim.Type == ClaimTypes.Email)!.Value;
		var username = user.FindFirst(claim => claim.Type == ClaimTypes.Name)!.Value;
		var roles = user.FindAll(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value);

		return new CurrentUser(id, username, email, roles);
	}
}