namespace Codeforge.Application.Users;

public record CurrentUser(string Id, string Username, string Email, IEnumerable<string> Roles) {
	public bool IsInRole(string role) {
		return Roles.Contains(role);
	}
}