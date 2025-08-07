using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Commands.DeleteTag;

public class DeleteTagCommandHandler(
	ILogger<DeleteTagCommandHandler> logger,
	ITagsRepository tagsRepository) : IRequestHandler<DeleteTagCommand> {
	public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("Delete tag with ID {Id}", request.Id);
		if (request.Id <= 0) throw new ValidationException("Tag ID must be greater than zero.");

		var tag = await tagsRepository.GetByIdAsync(request.Id);
		if (tag is null) throw new NotFoundException(nameof(Tag), request.Id.ToString());

		await tagsRepository.DeleteAsync(tag);
	}
}