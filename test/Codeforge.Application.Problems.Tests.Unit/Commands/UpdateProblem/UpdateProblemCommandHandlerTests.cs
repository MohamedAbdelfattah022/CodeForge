using CodeForge.Application.Problems.Commands.UpdateProblem;
using CodeForge.Domain.Constants;
using CodeForge.Domain.Entities;
using CodeForge.Domain.Exceptions;
using CodeForge.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Problems.Tests.Unit.Commands.UpdateProblem;

public class UpdateProblemCommandHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<UpdateProblemCommandHandler> _logger = Substitute.For<ILogger<UpdateProblemCommandHandler>>();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();

	public UpdateProblemCommandHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
	}

	[Fact]
	public async Task Handle_ShouldUpdateProblem_WhenValidCommandIsProvided() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var existingProblem = _fixture.Create<Problem>();
		existingProblem.Id = problemId;

		var command = new UpdateProblemCommand
			{
				Id = problemId,
				Title = "Updated Title",
				Description = "Updated Description",
				Constraints = "Updated Constraints",
				Difficulty = Difficulty.Hard
			};

		var handler = new UpdateProblemCommandHandler(_logger, _problemsRepository);
		_problemsRepository.GetByIdAsync(problemId).Returns(existingProblem);

		// Act
		await handler.Handle(command, CancellationToken.None);

		// Assert
		await _problemsRepository.Received(1).UpdateAsync(Arg.Is<Problem>(p =>
			p.Id == problemId &&
			p.Title == command.Title &&
			p.Description == command.Description &&
			p.Constraints == command.Constraints &&
			p.Difficulty == command.Difficulty));
	}

	[Theory]
	[InlineData("Updated Title", null, null, null)]
	[InlineData(null, "Updated Description", null, null)]
	[InlineData(null, null, "Updated Constraints", null)]
	[InlineData(null, null, null, Difficulty.Medium)]
	public async Task Handle_ShouldUpdateOnlyProvidedFields_WhenPartialUpdateIsRequested(string? title, string? description, string? constraints,
		Difficulty? difficulty) {
		// Arrange
		var problemId = _fixture.Create<int>();
		var existingProblem = _fixture.Create<Problem>();
		existingProblem.Id = problemId;
		var originalTitle = existingProblem.Title;
		var originalDescription = existingProblem.Description;
		var originalConstraints = existingProblem.Constraints;
		var originalDifficulty = existingProblem.Difficulty;


		var command = new UpdateProblemCommand
			{
				Id = problemId,
				Title = title,
				Description = description,
				Constraints = constraints,
				Difficulty = difficulty
			};

		var handler = new UpdateProblemCommandHandler(_logger, _problemsRepository);
		_problemsRepository.GetByIdAsync(problemId).Returns(existingProblem);

		// Act
		await handler.Handle(command, CancellationToken.None);

		// Assert
		await _problemsRepository.Received(1).UpdateAsync(Arg.Is<Problem>(p =>
			p.Id == problemId &&
			p.Title == (title ?? originalTitle) &&
			p.Description == (description ?? originalDescription) &&
			p.Constraints == (constraints ?? originalConstraints) &&
			p.Difficulty == (difficulty ?? originalDifficulty)
		));
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenProblemDoesNotExist() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var command = new UpdateProblemCommand { Id = problemId };
		var handler = new UpdateProblemCommandHandler(_logger, _problemsRepository);

		_problemsRepository.GetByIdAsync(problemId).Returns((Problem?)null);

		// Act
		var action = () => handler.Handle(command, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<NotFoundException>();
	}
}