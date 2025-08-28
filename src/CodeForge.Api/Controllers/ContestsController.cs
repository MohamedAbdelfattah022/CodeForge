using Codeforge.Application.Contests.Commands.CreateContest;
using Codeforge.Application.Contests.Commands.DeleteContest;
using Codeforge.Application.Contests.Commands.RegisterToContest;
using Codeforge.Application.Contests.Commands.UnregisterFromContest;
using Codeforge.Application.Contests.Commands.UpdateContest;
using Codeforge.Application.Contests.Queries.GetAllContests;
using Codeforge.Application.Contests.Queries.GetContestById;
using Codeforge.Application.Dtos;
using Codeforge.Application.Shared;
using Codeforge.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codeforge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContestsController(IMediator mediator) : ControllerBase {
	[HttpGet]
	public async Task<ActionResult<PaginationResult<ContestDto>>> GetContests([FromQuery] GetContestsQuery query) {
		var contests = await mediator.Send(query);
		return Ok(contests);
	}

	[HttpPost]
	[Authorize(Roles = UserRoles.Admin)]
	public async Task<ActionResult<int>> CreateContest([FromBody] CreateContestCommand command) {
		var contestId = await mediator.Send(command);
		return CreatedAtAction(nameof(GetContests), new { contestId }, contestId);
	}

	[HttpGet("{contestId:int}")]
	public async Task<ActionResult<ContestDto>> GetContestById(int contestId) {
		var contest = await mediator.Send(new GetContestByIdQuery(contestId));
		return Ok(contest);
	}

	[HttpPatch("{contestId:int}")]
	[Authorize(Roles = UserRoles.Admin)]
	public async Task<IActionResult> UpdateContest(int contestId, [FromBody] UpdateContestCommand command) {
		command.Id = contestId;
		await mediator.Send(command);
		return NoContent();
	}

	[HttpDelete("{contestId:int}")]
	[Authorize(Roles = UserRoles.Admin)]
	public async Task<IActionResult> DeleteContest(int contestId) {
		await mediator.Send(new DeleteContestCommand(contestId));
		return NoContent();
	}

	[HttpPost("{contestId:int}/register")]
	[Authorize]
	public async Task<IActionResult> RegisterToContest(int contestId) {
		await mediator.Send(new RegisterToContestCommand(contestId));
		return NoContent();
	}

	[HttpDelete("{contestId:int}/register")]
	[Authorize]
	public async Task<IActionResult> UnregisterFromContest(int contestId) {
		await mediator.Send(new UnregisterFromContestCommand(contestId));
		return NoContent();
	}
}