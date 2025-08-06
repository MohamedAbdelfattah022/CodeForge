using Codeforge.Application.Problems.Queries.GetProblemById;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Problems.Tests.Unit.Queries.GetProblemById;

public class GetProblemByIdQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<GetProblemByIdQueryHandler> _logger = Substitute.For<ILogger<GetProblemByIdQueryHandler>>();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();

	public GetProblemByIdQueryHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
	}

	[Fact]
	public async Task Handle_ShouldReturnProblemDto_WhenValidIdIsProvided() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var problem = _fixture.Create<Problem>();
		problem.Id = problemId;

		var query = new GetProblemByIdQuery(problemId);
		var handler = new GetProblemByIdQueryHandler(_logger, _problemsRepository);

		_problemsRepository.GetByIdAsync(problemId).Returns(problem);

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Id.Should().Be(problemId);
		result.Title.Should().Be(problem.Title);
		result.Description.Should().Be(problem.Description);
		result.Constraints.Should().Be(problem.Constraints);
		result.Difficulty.Should().Be(problem.Difficulty);
	}

	[Fact]
	public async Task Handle_ShouldThrowValidationException_WhenIdIsZero() {
		// Arrange
		var query = new GetProblemByIdQuery(0);
		var handler = new GetProblemByIdQueryHandler(_logger, _problemsRepository);

		// Act
		var action = () => handler.Handle(query, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<ValidationException>()
			.WithMessage("ID must be positive.");
	}

	[Fact]
	public async Task Handle_ShouldThrowValidationException_WhenIdIsNegative() {
		// Arrange
		var query = new GetProblemByIdQuery(-1);
		var handler = new GetProblemByIdQueryHandler(_logger, _problemsRepository);

		// Act
		var action = () => handler.Handle(query, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<ValidationException>()
			.WithMessage("ID must be positive.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenProblemDoesNotExist() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var query = new GetProblemByIdQuery(problemId);
		var handler = new GetProblemByIdQueryHandler(_logger, _problemsRepository);

		_problemsRepository.GetByIdAsync(problemId).Returns((Problem?)null);

		// Act
		var action = () => handler.Handle(query, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<NotFoundException>();
	}
}