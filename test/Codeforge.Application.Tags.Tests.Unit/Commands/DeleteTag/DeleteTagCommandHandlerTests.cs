using Codeforge.Application.Tags.Commands.DeleteTag;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Tests.Unit.Commands.DeleteTag;

public class DeleteTagCommandHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<DeleteTagCommandHandler> _logger = Substitute.For<ILogger<DeleteTagCommandHandler>>();
	private readonly ITagsRepository _tagsRepository = Substitute.For<ITagsRepository>();
	private readonly DeleteTagCommandHandler _handler;

	public DeleteTagCommandHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new DeleteTagCommandHandler(_logger, _tagsRepository);
	}

	[Fact]
	public async Task Handle_ShouldDeleteTag_WhenValidCommandIsProvided() {
		// Arrange
		var tag = _fixture.Create<Tag>();
		var command = new DeleteTagCommand(tag.Id);

		_tagsRepository.GetByIdAsync(tag.Id).Returns(tag);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _tagsRepository.Received(1).DeleteAsync(tag);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-10)]
	public async Task Handle_ShouldThrowValidationException_WhenIdIsLessThanOrEqualZero(int tagId) {
		// Arrange
		var command = new DeleteTagCommand(tagId);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("Tag ID must be greater than zero.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenTagDoesNotExist() {
		// Arrange
		var id = _fixture.Create<int>();
		var command = new DeleteTagCommand(id);

		_tagsRepository.GetByIdAsync(id).Returns((Tag)null!);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Tag)}*{id}*");
	}
}