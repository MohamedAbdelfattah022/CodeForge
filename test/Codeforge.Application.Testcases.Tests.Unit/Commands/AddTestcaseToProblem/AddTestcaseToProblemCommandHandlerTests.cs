using Codeforge.Application.Testcases.Commands.AddTestcaseToProblem;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Testcases.Tests.Unit.Commands.AddTestcaseToProblem;

public class AddTestcaseToProblemCommandHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly AddTestcaseToProblemCommandHandler _handler;
	private readonly ILogger<AddTestcaseToProblemCommandHandler> _logger = Substitute.For<ILogger<AddTestcaseToProblemCommandHandler>>();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();
	private readonly ITestcasesRepository _testcasesRepository = Substitute.For<ITestcasesRepository>();
	private readonly ITempCodeFileService _fileService = Substitute.For<ITempCodeFileService>();
	private readonly ISupabaseService _supabaseService = Substitute.For<ISupabaseService>();
	private readonly IOptions<SupabaseOptions> _supabaseOptions = Substitute.For<IOptions<SupabaseOptions>>();

	public AddTestcaseToProblemCommandHandlerTests() {
		_handler = new AddTestcaseToProblemCommandHandler(_logger, _testcasesRepository, _problemsRepository, _fileService, _supabaseService,
			_supabaseOptions);

		var supabaseOptionsValue = _fixture.Build<SupabaseOptions>()
			.With(x => x.Bucket, "test-bucket")
			.Create();
		_supabaseOptions.Value.Returns(supabaseOptionsValue);
	}

	[Fact]
	public async Task Handle_ShouldReturnTestcaseId_WhenValidCommandIsProvided() {
		// Arrange
		var command = _fixture.Create<AddTestcaseToProblemCommand>();
		var expectedTestcaseId = _fixture.Create<int>();

		_problemsRepository.ExistsAsync(command.ProblemId).Returns(true);
		_testcasesRepository.CreateAsync(Arg.Any<TestCase>()).Returns(expectedTestcaseId);

		_fileService.SaveCodeToTempFileAsync(command.Input, "txt").Returns(Task.FromResult("temp/input.txt"));
		_fileService.SaveCodeToTempFileAsync(command.ExpectedOutput, "txt").Returns(Task.FromResult("temp/expectedOutput.txt"));

		_supabaseService.UploadOrUpdateFileAsync("test-bucket", "temp/input.txt").Returns(Task.CompletedTask);
		_supabaseService.UploadOrUpdateFileAsync("test-bucket", "temp/expectedOutput.txt").Returns(Task.CompletedTask);
		
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