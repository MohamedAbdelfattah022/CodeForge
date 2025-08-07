using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Commands.CreateTag;

public class CreateTagCommandHandler(
	ILogger<CreateTagCommandHandler> logger,
	ITagsRepository tagsRepository)
	: IRequestHandler<CreateTagCommand, int> {
	public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("Creating tag with name: {Name}", request.Name);

		var tag = new Tag { Name = request.Name };
		var id = await tagsRepository.CreateAsync(tag);

		return id;
	}
}