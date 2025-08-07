using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Tags.Queries.GetAllTags;

public class GetAllTagsQuery : IRequest<List<TagDto>> {
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 50;
}