using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Queries.GetAllTags;

public class GetAllTagsQueryHandler(
	ILogger<GetAllTagsQueryHandler> logger,
	ITagsRepository tagsRepository) : IRequestHandler<GetAllTagsQuery, List<TagDto>> {
	public async Task<List<TagDto>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("Retrieving All Tags");
		var (tags, _) = await tagsRepository.GetAllAsync(request.PageNumber, request.PageSize);

		if (tags is null) throw new NotFoundException(nameof(Tag));

		var result = tags.Select(t => t.ToDto()).ToList();

		return result;
	}
}