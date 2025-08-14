using Codeforge.Application.Dtos;
using Codeforge.Application.Problems.Commands.CreateProblem;
using Codeforge.Application.Problems.Commands.DeleteProblem;
using Codeforge.Application.Problems.Commands.UpdateProblem;
using Codeforge.Application.Problems.Queries.GetAllProblems;
using Codeforge.Application.Problems.Queries.GetProblemById;
using Codeforge.Application.Shared;
using Codeforge.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codeforge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProblemsController(
	ILogger<ProblemsController> logger,
	IMediator mediator) : ControllerBase {
	[HttpGet]
	public async Task<ActionResult<PaginationResult<ProblemDto>>> GetProblems([FromQuery] GetProblemsQuery query) {
		var problems = await mediator.Send(query);
		return Ok(problems);
	}

	[HttpGet("{problemId:int}")]
	public async Task<ActionResult<ProblemDto>> GetProblemById(int problemId) {
		var problem = await mediator.Send(new GetProblemByIdQuery(problemId));

		return Ok(problem);
	}

	[HttpPost]
	[Authorize(Roles = UserRoles.Admin)]
	public async Task<ActionResult<int>> CreateProblem([FromBody] CreateProblemCommand command) {
		var problemId = await mediator.Send(command);
		return CreatedAtAction(nameof(GetProblemById), new { problemId }, problemId);
	}

	[HttpPatch("{problemId:int}")]
	[Authorize(Roles = UserRoles.Admin)]
	public async Task<IActionResult> UpdateProblem(int problemId, [FromBody] UpdateProblemCommand command) {
		command.Id = problemId;
		await mediator.Send(command);
		return NoContent();
	}

	[HttpDelete("{problemId:int}")]
	[Authorize(Roles = UserRoles.Admin)]
	public async Task<IActionResult> DeleteProblem(int problemId) {
		await mediator.Send(new DeleteProblemCommand(problemId));
		return NoContent();
	}
}