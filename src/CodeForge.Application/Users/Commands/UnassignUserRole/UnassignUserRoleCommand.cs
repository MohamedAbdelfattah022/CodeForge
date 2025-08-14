using MediatR;

namespace Codeforge.Application.Users.Commands.UnassignUserRole;

public class UnassignUserRoleCommand : IRequest{
	public required string UserEmail { get; set; }
	public required string RoleName { get; set; }
}