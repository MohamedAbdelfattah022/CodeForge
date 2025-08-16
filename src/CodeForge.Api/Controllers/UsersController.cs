using Codeforge.Application.Dtos;
using Codeforge.Application.Users.Commands.AssignUserRole;
using Codeforge.Application.Users.Commands.DeleteUser;
using Codeforge.Application.Users.Commands.UnassignUserRole;
using Codeforge.Application.Users.Commands.UpdateUserDetails;
using Codeforge.Application.Users.Queries.GetUserProfile;
using Codeforge.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codeforge.Api.Controllers;

[ApiController]
[Route("api/[controller]/user")]
public class UsersController(IMediator mediator) : ControllerBase {
	[Authorize]
	[HttpGet]
	public async Task<ActionResult<UserProfileDto>> GetUserProfile() {
		var userProfile = await mediator.Send(new GetUserProfileQuery());
		return Ok(userProfile);
	}

	[Authorize]
	[HttpPatch]
	public async Task<IActionResult> UpdateUserProfile(UpdateUserDetailsCommand request) {
		await mediator.Send(request);
		return NoContent();
	}

	[Authorize]
	[HttpDelete]
	public async Task<IActionResult> DeleteUser() {
		await mediator.Send(new DeleteUserCommand());
		return NoContent();
	}

	[HttpPost("/api/users/userRole")]
	[Authorize(Roles = UserRoles.Admin)]
	public async Task<IActionResult> AssignUserRole(AssignUserRoleCommand command) {
		await mediator.Send(command);
		return NoContent();
	}

	[HttpDelete("/api/users/userRole")]
	[Authorize(Roles = UserRoles.Admin)]
	public async Task<IActionResult> UnassignUserRole(UnassignUserRoleCommand command) {
		await mediator.Send(command);
		return NoContent();
	}
}