using Codeforge.Application.Submissions.Commands.CreateSubmission;
using Codeforge.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codeforge.Api.Controllers;

[Route("api")]
[ApiController]
public class SubmissionsController(IMediator mediator) : ControllerBase {
	[HttpPost("problems/{problemId:int}/submissions")]
	public async Task<IActionResult> Submit(int problemId, CreateSubmissionCommand submissionCommand) {
		submissionCommand.ProblemId = problemId;
		await mediator.Send(submissionCommand);
		return Ok("Submission created successfully.");
	}
}