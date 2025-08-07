using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Queries.GetTagById;

public class GetTagByIdQueryHandler(
	ILogger<GetTagByIdQueryHandler> logger,
	ITagsRepository tagsRepository) : IRequestHandler<GetTagByIdQuery, TagDto> {
	public async Task<TagDto> Handle(GetTagByIdQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("Retrieving Tag by ID: {TagId}", request.TagId);
		if (request.TagId <= 0) throw new ValidationException("Tag ID must be greater than zero.");

		var tag = await tagsRepository.GetByIdAsync(request.TagId);
		if (tag is null) throw new NotFoundException(nameof(Tag), request.TagId.ToString());

		return tag.ToDto();
	}
}