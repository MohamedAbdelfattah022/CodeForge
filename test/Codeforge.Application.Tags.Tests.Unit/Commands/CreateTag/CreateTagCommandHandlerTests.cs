using Codeforge.Application.Tags.Commands.CreateTag;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Codeforge.Application.Tags.Tests.Unit.Commands.CreateTag;

public class CreateTagCommandHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<CreateTagCommandHandler> _logger = Substitute.For<ILogger<CreateTagCommandHandler>>();
	private readonly ITagsRepository _tagsRepository = Substitute.For<ITagsRepository>();

	[Fact]
	public async Task Handle_ShouldReturnTagId_WhenValidCommandIsProvided() {
		// Arrange
		var command = _fixture.Build<CreateTagCommand>()
			.With(c => c.Name, _fixture.Create<string>())
			.Create();

		var expectedId = _fixture.Create<int>();
		var handler = new CreateTagCommandHandler(_logger, _tagsRepository);

		_tagsRepository.CreateAsync(Arg.Any<Tag>()).Returns(expectedId);

		// Act
		var id = await handler.Handle(command, CancellationToken.None);

		// Assert
		id.Should().Be(expectedId);
		await _tagsRepository.Received(1).CreateAsync(Arg.Is<Tag>(t => t.Name == command.Name));
	}

	[Fact]
	public void CreateTagCommand_ShouldFailValidation_WhenNameIsNull() {
		// Arrange
		var command = new CreateTagCommand { Name = null! };
		var context = new ValidationContext(command);
		var validationResults = new List<ValidationResult>();

		// Act
		var isValid = Validator.TryValidateObject(command, context, validationResults, true);

		// Assert
		isValid.Should().BeFalse();
		validationResults.Should().ContainSingle()
			.Which.MemberNames.Should().Contain(nameof(CreateTagCommand.Name));
	}
}