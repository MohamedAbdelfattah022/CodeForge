using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Tags.Queries.GetProblemTags;

public class GetProblemTagQuery(int problemId) : IRequest<List<TagDto>> {
	public int Id { get; set; } = problemId;
}