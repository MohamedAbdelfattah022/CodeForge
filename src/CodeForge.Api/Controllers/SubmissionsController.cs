using Codeforge.Application.Dtos;
using Codeforge.Application.Submissions.Commands.CreateSubmission;
using Codeforge.Application.Submissions.Queries.GetProblemSubmissions;
using Codeforge.Application.Submissions.Queries.GetSubmissionById;
using Codeforge.Application.Submissions.Queries.GetSubmissionStatus;
using Codeforge.Application.Submissions.Queries.GetUserSubmissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codeforge.Api.Controllers;

[Route("api")]
[ApiController]
public class SubmissionsController(IMediator mediator) : ControllerBase {
	[HttpPost("problems/{problemId:int}/submissions")]
	[Authorize]
	public async Task<IActionResult> Submit(int problemId, CreateSubmissionCommand submissionCommand) {
		submissionCommand.ProblemId = problemId;
		var id = await mediator.Send(submissionCommand);
		return CreatedAtAction(nameof(GetSubmission), new { problemId, submissionId = id }, id);
	}

	[Authorize]
	[HttpGet("submissions/{submissionId:int}/status")]
	public async Task<ActionResult<SubmissionStatusDto>> GetStatus(int submissionId) {
		var result = await mediator.Send(new GetSubmissionStatusQuery(submissionId));
		return Ok(result);
	}

	[HttpGet("problems/{problemId:int}/submissions/{submissionId:int}")]
	public async Task<ActionResult<SubmissionDto>> GetSubmission(int problemId, int submissionId) {
		var query = new GetSubmissionByIdQuery(submissionId, problemId);
		var result = await mediator.Send(query);
		return Ok(result);
	}

	[HttpGet("problems/{problemId:int}/submissions")]
	public async Task<ActionResult<List<SubmissionMetadata>>> GetSubmissions(int problemId) {
		var query = new GetProblemSubmissionsQuery(problemId);
		var result = await mediator.Send(query);
		return Ok(result);
	}


	[HttpGet("users/{userId}/submissions")]
	public async Task<ActionResult<List<SubmissionMetadata>>> GetUserSubmissions(string userId) {
		var query = new GetUserSubmissionsQuery(userId);
		var result = await mediator.Send(query);
		return Ok(result);
	}
}