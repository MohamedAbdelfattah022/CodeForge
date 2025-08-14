using MediatR;

namespace Codeforge.Application.Users.Commands.UpdateUserDetails;

public class UpdateUserDetailsCommand : IRequest {
	public string? UserName { get; set; }
	public string? Email { get; set; }
}