using Codeforge.Application.Dtos;
using Codeforge.Application.Tags.Commands.AddTagToProblem;
using Codeforge.Application.Tags.Commands.CreateTag;
using Codeforge.Application.Tags.Commands.DeleteTag;
using Codeforge.Application.Tags.Commands.RemoveTagFromProblem;
using Codeforge.Application.Tags.Commands.UpdateTag;
using Codeforge.Application.Tags.Queries.GetAllTags;
using Codeforge.Application.Tags.Queries.GetProblemTags;
using Codeforge.Application.Tags.Queries.GetTagById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Codeforge.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TagsController(IMediator mediator) : ControllerBase {
	[HttpGet]
	public async Task<ActionResult<List<TagDto>>> GetAllTags() {
		var query = new GetAllTagsQuery();
		var result = await mediator.Send(query);
		return Ok(result);
	}

	[HttpPost]
	public async Task<ActionResult<int>> CreateTag(CreateTagCommand command) {
		var id = await mediator.Send(command);
		return CreatedAtAction(nameof(GetTagById), new { id }, id);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<TagDto>> GetTagById(int id) {
		var query = new GetTagByIdQuery(id);
		var result = await mediator.Send(query);
		return Ok(result);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> UpdateTag(int id, UpdateTagCommand command) {
		command.Id = id;
		await mediator.Send(command);
		return NoContent();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> DeleteTag(int id) {
		var command = new DeleteTagCommand(id);
		await mediator.Send(command);
		return NoContent();
	}

	[HttpGet("/api/problems/{problemId:int}/tags")]
	public async Task<ActionResult<List<TagDto>>> GetProblemTags(int problemId) {
		var query = new GetProblemTagQuery(problemId);
		var result = await mediator.Send(query);
		return Ok(result);
	}

	[HttpPost("/api/problems/{problemId:int}/tags")]
	public async Task<IActionResult> AddTagToProblem(int problemId, [FromBody] AddTagToProblemCommand command) {
		command.ProblemId = problemId;
		await mediator.Send(command);
		return NoContent();
	}

	[HttpDelete("/api/problems/{problemId:int}/tags/{tagId:int}")]
	public async Task<IActionResult> RemoveTagFromProblem(int problemId, int tagId) {
		var command = new RemoveTagFromProblemCommand { ProblemId = problemId, TagId = tagId };
		await mediator.Send(command);
		return NoContent();
	}
}