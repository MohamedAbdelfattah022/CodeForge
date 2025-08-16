using Codeforge.Application.Testcases.Commands.DeleteTestcase;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Testcases.Tests.Unit.Commands.DeleteTestcase;

public class DeleteTestcaseCommandHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly DeleteTestcaseCommandHandler _handler;
	private readonly ILogger<DeleteTestcaseCommandHandler> _logger = Substitute.For<ILogger<DeleteTestcaseCommandHandler>>();
	private readonly ITestcasesRepository _testcasesRepository = Substitute.For<ITestcasesRepository>();
	private readonly ISupabaseService _supabaseService = Substitute.For<ISupabaseService>();
	private readonly IOptions<SupabaseOptions> _supabaseOptions = Substitute.For<IOptions<SupabaseOptions>>();

	public DeleteTestcaseCommandHandlerTests() {
		_handler = new DeleteTestcaseCommandHandler(_logger, _testcasesRepository, _supabaseService, _supabaseOptions);
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		var supabaseOptionsValue = _fixture.Build<SupabaseOptions>()
			.With(x => x.Bucket, "test-bucket")
			.Create();
		_supabaseOptions.Value.Returns(supabaseOptionsValue);
	}

	[Fact]
	public async Task Handle_ShouldDeleteTestcase_WhenValidCommandIsProvided() {
		// Arrange
		var testcaseId = _fixture.Create<int>();
		var command = new DeleteTestcaseCommand(testcaseId);
		var testcase = _fixture.Create<TestCase>();

		_testcasesRepository.GetByIdAsync(testcaseId).Returns(testcase);
		_supabaseService.DeleteFileAsync(_supabaseOptions.Value.Bucket, testcase.Input).Returns(Task.CompletedTask);
		_supabaseService.DeleteFileAsync(_supabaseOptions.Value.Bucket, testcase.ExpectedOutput).Returns(Task.CompletedTask);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _testcasesRepository.Received(1).GetByIdAsync(testcaseId);
		await _supabaseService.Received(1).DeleteFileAsync(_supabaseOptions.Value.Bucket, testcase.Input);
		await _supabaseService.Received(1).DeleteFileAsync(_supabaseOptions.Value.Bucket, testcase.ExpectedOutput);
		await _testcasesRepository.Received(1).DeleteAsync(testcase);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task Handle_ShouldThrowValidationException_WhenTestcaseIdIsLessThanOrEqualZero(int id) {
		// Arrange
		var command = new DeleteTestcaseCommand(id);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("TestcaseId must be positive.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenTestcaseDoesNotExist() {
		// Arrange
		var testcaseId = _fixture.Create<int>();
		var command = new DeleteTestcaseCommand(testcaseId);

		_testcasesRepository.GetByIdAsync(testcaseId).Returns((TestCase?)null);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(TestCase)}*{testcaseId}*");
	}

	[Fact]
	public async Task Handle_ShouldNotCallDeleteAsync_WhenTestcaseNotFound() {
		// Arrange
		var testcaseId = _fixture.Create<int>();
		var command = new DeleteTestcaseCommand(testcaseId);

		_testcasesRepository.GetByIdAsync(testcaseId).Returns((TestCase?)null);

		// Act
		var act = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>();
		await _testcasesRepository.DidNotReceive().DeleteAsync(Arg.Any<TestCase>());
	}
}