using CodeForge.Application.Dtos;
using CodeForge.Application.Problems.Commands.CreateProblem;
using CodeForge.Application.Problems.Commands.DeleteProblem;
using CodeForge.Application.Problems.Commands.UpdateProblem;
using CodeForge.Application.Problems.Queries.GetAllProblems;
using CodeForge.Application.Problems.Queries.GetProblemById;
using CodeForge.Application.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CodeForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProblemsController(
	ILogger<ProblemsController> logger,
	IMediator mediator) : ControllerBase {
	[HttpGet]
	public async Task<ActionResult<PaginationResult<ProblemDto>>> GetProblems([FromQuery] GetProblemsQuery query) {
		logger.LogInformation("ProblemsController.GetProblems called with query: {@Query}", query);
		
		var problems = await mediator.Send(query);
		return Ok(problems);
	}

	[HttpGet("{problemId:int}")]
	public async Task<ActionResult<ProblemDto>> GetProblemById(int problemId) {
		logger.LogInformation("ProblemsController.GetProblemById called with problemId: {ProblemId}", problemId);
		var problem = await mediator.Send(new GetProblemByIdQuery(problemId));

		return Ok(problem);
	}

	[HttpPost]
	public async Task<ActionResult<int>> CreateProblem([FromBody] CreateProblemCommand command) {
		logger.LogInformation("ProblemsController.CreateProblem called with command: {@Command}", command);
		
		var problemId = await mediator.Send(command);
		return CreatedAtAction(nameof(GetProblemById), new { problemId }, problemId);
	}

	[HttpPatch("{problemId:int}")]
	public async Task<IActionResult> UpdateProblem(int problemId, [FromBody] UpdateProblemCommand command) {
		logger.LogInformation("ProblemsController.UpdateProblem called with problemId: {ProblemId} and command: {@Command}", problemId, command);

		command.Id = problemId;
		await mediator.Send(command);
		return NoContent();
	}
	
	[HttpDelete("{problemId:int}")]
	public async Task<IActionResult> DeleteProblem(int problemId) {
		logger.LogInformation("ProblemsController.DeleteProblem called with problemId: {ProblemId}", problemId);
		
		await mediator.Send(new DeleteProblemCommand(problemId));
		return NoContent();
	}
}