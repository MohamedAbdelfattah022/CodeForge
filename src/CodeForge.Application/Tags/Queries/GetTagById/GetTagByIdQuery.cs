using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Tags.Queries.GetTagById;

public class GetTagByIdQuery(int tagId) : IRequest<TagDto> {
	public int TagId { get; set; } = tagId;
}