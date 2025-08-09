using Codeforge.Application.Tags.Commands.UpdateTag;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Tests.Unit.Commands.UpdateTag;

public class UpdateTagCommandHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<UpdateTagCommandHandler> _logger = Substitute.For<ILogger<UpdateTagCommandHandler>>();
	private readonly ITagsRepository _tagsRepository = Substitute.For<ITagsRepository>();
	private readonly UpdateTagCommandHandler _handler;

	public UpdateTagCommandHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new UpdateTagCommandHandler(_logger, _tagsRepository);
	}

	[Fact]
	public async Task Handle_ShouldUpdateTag_WhenValidCommandIsProvided() {
		// Arrange
		var existingTag = _fixture.Create<Tag>();
		var command = new UpdateTagCommand(existingTag.Id) { Name = _fixture.Create<string>() };

		_tagsRepository.GetByIdAsync(existingTag.Id).Returns(existingTag);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		existingTag.Name.Should().Be(command.Name);
		await _tagsRepository.Received(1).UpdateAsync(existingTag);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-5)]
	public async Task Handle_ShouldThrowValidationException_WhenIdIsLessThanOrEqualZero(int tagId) {
		// Arrange
		var command = new UpdateTagCommand(tagId) { Name = _fixture.Create<string>() };

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("Tag ID must be greater than zero.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenTagDoesNotExist() {
		// Arrange
		var command = new UpdateTagCommand(_fixture.Create<int>()) { Name = _fixture.Create<string>() };

		_tagsRepository.GetByIdAsync(Arg.Any<int>()).Returns((Tag)null!);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Tag)}*{command.Id}*");
	}
}