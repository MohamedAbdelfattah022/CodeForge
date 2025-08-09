using Codeforge.Application.Tags.Commands.AddTagToProblem;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Tests.Unit.Commands.AddTagToProblem;

public class AddTagToProblemCommandHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<AddTagToProblemCommandHandler> _logger = Substitute.For<ILogger<AddTagToProblemCommandHandler>>();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();
	private readonly ITagsRepository _tagsRepository = Substitute.For<ITagsRepository>();
	private readonly AddTagToProblemCommandHandler _handler;

	public AddTagToProblemCommandHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new AddTagToProblemCommandHandler(_logger, _problemsRepository, _tagsRepository);
	}

	[Fact]
	public async Task Handle_ShouldAddTagToProblem_WhenValidCommandIsProvided() {
		// Arrange
		var problem = _fixture.Create<Problem>();
		var tag = _fixture.Create<Tag>();
		var command = new AddTagToProblemCommand { ProblemId = problem.Id, TagId = tag.Id };

		_problemsRepository.GetByIdWithTagsAsync(problem.Id).Returns(problem);
		_tagsRepository.GetByIdAsync(tag.Id).Returns(tag);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _problemsRepository.Received(1).AddTagToProblemAsync(problem, tag);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task Handle_ShouldThrowValidationException_WhenProblemIdIsLessThanOrEqualZero(int problemId) {
		// Arrange
		var command = new AddTagToProblemCommand { ProblemId = problemId, TagId = 1 };

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("ProblemId must be a positive integer.");
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task Handle_ShouldThrowValidationException_WhenTagIdIsLessThanOrEqualZero(int tagId) {
		// Arrange
		var command = new AddTagToProblemCommand { ProblemId = 1, TagId = tagId };

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("TagId must be a positive integer.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenProblemDoesNotExist() {
		// Arrange
		var command = new AddTagToProblemCommand { ProblemId = _fixture.Create<int>(), TagId = _fixture.Create<int>() };

		_problemsRepository.GetByIdWithTagsAsync(command.ProblemId).Returns((Problem)null!);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Problem)}*{command.ProblemId}*");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenTagDoesNotExist() {
		// Arrange
		var problem = _fixture.Create<Problem>();
		var command = new AddTagToProblemCommand { ProblemId = problem.Id, TagId = _fixture.Create<int>() };

		_problemsRepository.GetByIdWithTagsAsync(problem.Id).Returns(problem);
		_tagsRepository.GetByIdAsync(command.TagId).Returns((Tag)null!);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Tag)}*{command.TagId}*");
	}
}