using Codeforge.Application.Testcases.Commands.AddTestcaseToProblem;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Testcases.Tests.Unit.Commands.AddTestcaseToProblem;

public class AddTestcaseToProblemCommandHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly AddTestcaseToProblemCommandHandler _handler;
	private readonly ILogger<AddTestcaseToProblemCommandHandler> _logger = Substitute.For<ILogger<AddTestcaseToProblemCommandHandler>>();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();
	private readonly ITestcasesRepository _testcasesRepository = Substitute.For<ITestcasesRepository>();

	public AddTestcaseToProblemCommandHandlerTests() {
		_handler = new AddTestcaseToProblemCommandHandler(_logger, _testcasesRepository, _problemsRepository);
	}

	[Fact]
	public async Task Handle_ShouldReturnTestcaseId_WhenValidCommandIsProvided() {
		// Arrange
		var command = _fixture.Create<AddTestcaseToProblemCommand>();
		var expectedTestcaseId = _fixture.Create<int>();

		_problemsRepository.ExistsAsync(command.ProblemId).Returns(true);
		_testcasesRepository.CreateAsync(Arg.Any<TestCase>()).Returns(expectedTestcaseId);

		// Act
		var id = await _handler.Handle(command, CancellationToken.None);

		// Assert
		id.Should().Be(expectedTestcaseId);
		await _problemsRepository.Received(1).ExistsAsync(command.ProblemId);
		await _testcasesRepository.Received(1).CreateAsync(Arg.Is<TestCase>(tc =>
			tc.ProblemId == command.ProblemId &&
			tc.Input == command.Input &&
			tc.ExpectedOutput == command.ExpectedOutput &&
			tc.IsVisible == command.IsVisible));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task Handle_ShouldThrowValidationException_WhenProblemIdIsLessThanOrEqualZero(int id) {
		// Arrange
		var command = _fixture.Create<AddTestcaseToProblemCommand>();
		command.ProblemId = id;

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("ProblemId must be a positive integer.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenProblemDoesNotExist() {
		// Arrange
		var command = _fixture.Create<AddTestcaseToProblemCommand>();

		_problemsRepository.ExistsAsync(command.ProblemId).Returns(false);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Problem)}*{command.ProblemId}*");
	}
}