using Codeforge.Application.Dtos;
using Codeforge.Application.Testcases.Commands.AddTestcaseToProblem;
using Codeforge.Application.Testcases.Commands.DeleteTestcase;
using Codeforge.Application.Testcases.Commands.UpdateTestcase;
using Codeforge.Application.Testcases.Queries.GetProblemTestcase;
using Codeforge.Application.Testcases.Queries.GetTestcases;
using Codeforge.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codeforge.Api.Controllers;

[ApiController]
[Route("api")]
public class TestcasesController(ILogger<TestcasesController> logger, IMediator mediator) : ControllerBase {
	[HttpGet]
	[Route("problems/{problemId:int}/[controller]")]
	public async Task<ActionResult<IEnumerable<TestcaseDto>>> GetTestcases([FromRoute] int problemId) {
		var query = new GetTestcasesQuery(problemId);
		var testcases = await mediator.Send(query);
		return Ok(testcases);
	}

	[HttpGet]
	[Route("problems/{problemId:int}/[controller]/{testcaseId:int}")]
	public async Task<ActionResult<TestcaseDto>> GetTestcase([FromRoute] int problemId, [FromRoute] int testcaseId) {
		var query = new GetProblemTestcaseQuery(problemId, testcaseId);
		var testcase = await mediator.Send(query);
		return Ok(testcase);
	}

	[HttpPost]
	[Authorize(Roles = UserRoles.Admin)]
	[Route("problems/{problemId:int}/[controller]")]
	public async Task<ActionResult<int>> CreateTestcase([FromRoute] int problemId, [FromBody] AddTestcaseToProblemCommand command) {
		command.ProblemId = problemId;
		var testcaseId = await mediator.Send(command);
		return CreatedAtAction(nameof(GetTestcase), new { problemId, testcaseId }, testcaseId);
	}

	[HttpPatch]
	[Authorize(Roles = UserRoles.Admin)]
	[Route("[controller]/{testcaseId:int}")]
	public async Task<IActionResult> UpdateTestcase([FromRoute] int testcaseId, [FromBody] UpdateTestcaseCommand command) {
		command.TestcaseId = testcaseId;

		await mediator.Send(command);
		return NoContent();
	}

	[HttpDelete]
	[Authorize(Roles = UserRoles.Admin)]
	[Route("[controller]/{testcaseId:int}")]
	public async Task<IActionResult> DeleteTestcase([FromRoute] int testcaseId) {
		var command = new DeleteTestcaseCommand(testcaseId);
		await mediator.Send(command);
		return NoContent();
	}
}