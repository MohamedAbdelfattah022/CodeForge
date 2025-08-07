using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Commands.UpdateTag;

public class UpdateTagCommandHandler(
	ILogger<UpdateTagCommandHandler> logger,
	ITagsRepository tagsRepository) : IRequestHandler<UpdateTagCommand> {
	public async Task Handle(UpdateTagCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("Updating tag with ID {Id}", request.Id);

		if (request.Id <= 0) throw new ValidationException("Tag ID must be greater than zero.");

		var tag = await tagsRepository.GetByIdAsync(request.Id);
		if (tag is null) throw new NotFoundException(nameof(Tag), request.Id.ToString());

		tag.Name = request.Name;

		await tagsRepository.UpdateAsync(tag);
	}
}