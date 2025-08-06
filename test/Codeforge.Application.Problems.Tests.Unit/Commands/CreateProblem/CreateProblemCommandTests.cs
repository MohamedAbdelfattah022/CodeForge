using Codeforge.Application.Problems.Commands.CreateProblem;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Problems.Tests.Unit.Commands.CreateProblem;

public class CreateProblemCommandTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<CreateProblemCommandHandler> _logger = Substitute.For<ILogger<CreateProblemCommandHandler>>();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();

	[Fact]
	public async Task Handle_ShouldReturnProblemId_WhenValidCommandIsProvided() {
		// Arrange
		var command = _fixture.Create<CreateProblemCommand>();
		var expectedProblemId = _fixture.Create<int>();
		var handler = new CreateProblemCommandHandler(_logger, _problemsRepository);

		_problemsRepository.CreateAsync(Arg.Any<Problem>()).Returns(expectedProblemId);

		// Act
		var result = await handler.Handle(command, CancellationToken.None);

		// Assert
		result.Should().Be(expectedProblemId);
		await _problemsRepository.Received(1).CreateAsync(Arg.Is<Problem>(p =>
			p.Title == command.Title &&
			p.Description == command.Description &&
			p.Constraints == command.Constraints &&
			p.Difficulty == command.Difficulty));
	}

	[Fact]
	public void ToProblem_ShouldMapCorrectly_WhenValidCommandIsProvided() {
		// Arrange
		var command = _fixture.Create<CreateProblemCommand>();

		// Act
		var result = command.ToProblem();

		// Assert
		result.Should().NotBeNull();
		result.Title.Should().Be(command.Title);
		result.Description.Should().Be(command.Description);
		result.Constraints.Should().Be(command.Constraints);
		result.Difficulty.Should().Be(command.Difficulty);
	}
}