using Codeforge.Application.Dtos;
using Codeforge.Application.Testcases.Queries.GetProblemTestcase;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Testcases.Tests.Unit.Queries.GetProblemTestcase;

public class GetProblemTestcaseQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly GetProblemTestcaseQueryHandler _handler;
	private readonly ILogger<GetProblemTestcaseQueryHandler> _logger = Substitute.For<ILogger<GetProblemTestcaseQueryHandler>>();
	private readonly ITestcasesRepository _testcasesRepository = Substitute.For<ITestcasesRepository>();
	private readonly ISupabaseService _supabaseService = Substitute.For<ISupabaseService>();
	private readonly IOptions<SupabaseOptions> _supabaseOptions = Substitute.For<IOptions<SupabaseOptions>>();

	public GetProblemTestcaseQueryHandlerTests() {
		_handler = new GetProblemTestcaseQueryHandler(_logger, _testcasesRepository, _supabaseService, _supabaseOptions);
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
		
		var supabaseOptionsValue = _fixture.Build<SupabaseOptions>()
			.With(x => x.Bucket, "test-bucket")
			.Create();
		_supabaseOptions.Value.Returns(supabaseOptionsValue);
	}

	[Fact]
	public async Task Handle_ShouldReturnTestcaseDto_WhenValidQueryIsProvided() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var testcaseId = _fixture.Create<int>();
		var query = new GetProblemTestcaseQuery(problemId, testcaseId);
		var testcase = _fixture.Create<TestCase>();

		_testcasesRepository.GetProblemTestcaseByIdAsync(problemId, testcaseId).Returns(testcase);

		_supabaseService.ReadFileAsync(_supabaseOptions.Value.Bucket, testcase.Input).Returns(Task.FromResult(testcase.Input));
		_supabaseService.ReadFileAsync(_supabaseOptions.Value.Bucket, testcase.ExpectedOutput).Returns(Task.FromResult(testcase.ExpectedOutput));
		
		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<TestcaseDto>();
		result.Id.Should().Be(testcase.Id);
		result.Input.Should().Be(testcase.Input);
		result.ExpectedOutput.Should().Be(testcase.ExpectedOutput);

		await _testcasesRepository.Received(1).GetProblemTestcaseByIdAsync(problemId, testcaseId);
	}

	[Theory]
	[InlineData(0, 1)]
	[InlineData(1, 0)]
	[InlineData(-1, 1)]
	[InlineData(1, -1)]
	public async Task Handle_ShouldThrowValidationException_WhenProblemOrTestcaseIdAreInvalid(int problemId, int testcaseId) {
		// Arrange
		var query = new GetProblemTestcaseQuery(problemId, testcaseId);

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("ProblemId and TestcaseId must be greater than zero.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenTestcaseDoesNotExist() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var testcaseId = _fixture.Create<int>();
		var query = new GetProblemTestcaseQuery(problemId, testcaseId);

		_testcasesRepository.GetProblemTestcaseByIdAsync(problemId, testcaseId).Returns((TestCase?)null);

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(TestCase)}*{testcaseId}*");
	}

	[Fact]
	public async Task Handle_ShouldNotCallRepository_WhenIdsAreInvalid() {
		// Arrange
		var query = new GetProblemTestcaseQuery(0, 0);

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>();
		await _testcasesRepository.DidNotReceive().GetProblemTestcaseByIdAsync(Arg.Any<int>(), Arg.Any<int>());
	}
}