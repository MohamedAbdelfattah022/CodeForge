using Codeforge.Application.Testcases.Commands.UpdateTestcase;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Testcases.Tests.Unit.Commands.UpdateTestcase;

public class UpdateTestcaseCommandHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly UpdateTestcaseCommandHandler _handler;
	private readonly ILogger<UpdateTestcaseCommandHandler> _logger = Substitute.For<ILogger<UpdateTestcaseCommandHandler>>();
	private readonly ITestcasesRepository _testcasesRepository = Substitute.For<ITestcasesRepository>();
	private readonly ITempCodeFileService _fileService = Substitute.For<ITempCodeFileService>();
	private readonly ISupabaseService _supabaseService = Substitute.For<ISupabaseService>();
	private readonly IOptions<SupabaseOptions> _supabaseOptions = Substitute.For<IOptions<SupabaseOptions>>();

	public UpdateTestcaseCommandHandlerTests() {
		_handler = new UpdateTestcaseCommandHandler(_logger, _testcasesRepository, _supabaseService, _fileService, _supabaseOptions);
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		var supabaseOptionsValue = _fixture.Build<SupabaseOptions>()
			.With(x => x.Bucket, "test-bucket")
			.Create();
		_supabaseOptions.Value.Returns(supabaseOptionsValue);
	}

	[Fact]
	public async Task Handle_ShouldUpdateTestcase_WhenValidCommandIsProvided() {
		// Arrange
		var command = _fixture.Create<UpdateTestcaseCommand>();
		var existingTestcase = _fixture.Create<TestCase>();

		_testcasesRepository.GetByIdAsync(command.TestcaseId).Returns(existingTestcase);
		_fileService.CreateFileWithNameAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult("temp-file-path"));
		_supabaseService.UploadOrUpdateFileAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);
		_fileService.DeleteTempFile(Arg.Any<string>());

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _testcasesRepository.Received(1).GetByIdAsync(command.TestcaseId);
		await _testcasesRepository.Received(1).UpdateAsync(Arg.Is<TestCase>(tc =>
			tc.Id == existingTestcase.Id &&
			tc.Input == existingTestcase.Input &&
			tc.ExpectedOutput == existingTestcase.ExpectedOutput &&
			tc.IsVisible == (command.IsVisible ?? existingTestcase.IsVisible)));
	}

	[Fact]
	public async Task Handle_ShouldUpdateOnlyProvidedFields_WhenPartialUpdateIsRequested() {
		// Arrange
		var command = new UpdateTestcaseCommand
			{
				TestcaseId = 123,
				Input = "new input",
				ExpectedOutput = null,
				IsVisible = null
			};

		var existingTestcase = new TestCase
			{
				Id = 123,
				Input = "old input",
				ExpectedOutput = "old output",
				IsVisible = false
			};

		_testcasesRepository.GetByIdAsync(command.TestcaseId).Returns(existingTestcase);
		_fileService.CreateFileWithNameAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult("temp-file-path"));
		_supabaseService.UploadOrUpdateFileAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);
		_fileService.DeleteTempFile(Arg.Any<string>());

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _supabaseService.Received(1).UploadOrUpdateFileAsync(
			Arg.Any<string>(),
			Arg.Any<string>());
		await _fileService.Received(1).CreateFileWithNameAsync(
			Arg.Any<string>(),
			Arg.Any<string>());
		await _testcasesRepository.Received(1).UpdateAsync(Arg.Is<TestCase>(tc =>
			tc.IsVisible == false));
	}

	[Fact]
	public async Task Handle_ShouldPreserveOriginalValues_WhenNullValuesAreProvided() {
		// Arrange
		var command = new UpdateTestcaseCommand
			{
				TestcaseId = 456,
				Input = null,
				ExpectedOutput = null,
				IsVisible = null
			};

		var existingTestcase = new TestCase
			{
				Id = 456,
				Input = "original input",
				ExpectedOutput = "original output",
				IsVisible = true
			};

		_testcasesRepository.GetByIdAsync(command.TestcaseId).Returns(existingTestcase);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _testcasesRepository.Received(1).UpdateAsync(Arg.Is<TestCase>(tc =>
			tc.Input == "original input" &&
			tc.ExpectedOutput == "original output" &&
			tc.IsVisible == true));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task Handle_ShouldThrowValidationException_WhenTestcaseIdIsLessThanOrEqualZero(int id) {
		// Arrange
		var command = _fixture.Create<UpdateTestcaseCommand>();
		command.TestcaseId = id;

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("Testcase ID must be greater than 0.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenTestcaseDoesNotExist() {
		// Arrange
		var command = _fixture.Create<UpdateTestcaseCommand>();

		_testcasesRepository.GetByIdAsync(command.TestcaseId).Returns((TestCase?)null);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(TestCase)}*{command.TestcaseId}*");
	}

	[Fact]
	public async Task Handle_ShouldNotCallUpdateAsync_WhenTestcaseNotFound() {
		// Arrange
		var command = _fixture.Create<UpdateTestcaseCommand>();

		_testcasesRepository.GetByIdAsync(command.TestcaseId).Returns((TestCase?)null);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>();
		await _testcasesRepository.DidNotReceive().UpdateAsync(Arg.Any<TestCase>());
	}
}