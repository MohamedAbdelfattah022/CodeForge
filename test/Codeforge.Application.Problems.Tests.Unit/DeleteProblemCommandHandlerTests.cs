using AutoFixture;
using CodeForge.Application.Problems.Commands.DeleteProblem;
using CodeForge.Domain.Entities;
using CodeForge.Domain.Exceptions;
using CodeForge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Problems.Tests.Unit;

public class DeleteProblemCommandHandlerTests {
	private readonly ILogger<DeleteProblemCommandHandler> _logger = Substitute.For<ILogger<DeleteProblemCommandHandler>>();
	private readonly Fixture _fixture = new();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();

	public DeleteProblemCommandHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
	}

	[Fact]
	public async Task Handle_ShouldDeleteProblem_WhenValidIdIsProvided() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var problem = _fixture.Create<Problem>();
		problem.Id = problemId;

		var command = new DeleteProblemCommand(problemId);
		var handler = new DeleteProblemCommandHandler(_logger, _problemsRepository);

		_problemsRepository.GetByIdAsync(problemId).Returns(problem);

		// Act
		await handler.Handle(command, CancellationToken.None);

		// Assert
		await _problemsRepository.Received(1).DeleteAsync(problem);
	}

	[Fact]
	public async Task Handle_ShouldThrowValidationException_WhenIdIsZero() {
		// Arrange
		var command = new DeleteProblemCommand(0);
		var handler = new DeleteProblemCommandHandler(_logger, _problemsRepository);

		// Act
		var action = () => handler.Handle(command, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<ValidationException>()
			.WithMessage("ID must be positive.");
	}

	[Fact]
	public async Task Handle_ShouldThrowValidationException_WhenIdIsNegative() {
		// Arrange
		var command = new DeleteProblemCommand(-1);
		var handler = new DeleteProblemCommandHandler(_logger, _problemsRepository);

		// Act
		var action = () => handler.Handle(command, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<ValidationException>()
			.WithMessage("ID must be positive.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenProblemDoesNotExist() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var command = new DeleteProblemCommand(problemId);
		var handler = new DeleteProblemCommandHandler(_logger, _problemsRepository);

		_problemsRepository.GetByIdAsync(problemId).Returns((Problem?)null);

		// Act
		var action = () => handler.Handle(command, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<NotFoundException>();
	}
}