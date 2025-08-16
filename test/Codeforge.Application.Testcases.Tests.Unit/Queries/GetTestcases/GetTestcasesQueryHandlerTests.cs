using Codeforge.Application.Testcases.Queries.GetTestcases;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Testcases.Tests.Unit.Queries.GetTestcases;

public class GetTestcasesQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly GetTestcasesQueryHandler _handler;
	private readonly ILogger<GetTestcasesQueryHandler> _logger = Substitute.For<ILogger<GetTestcasesQueryHandler>>();
	private readonly ITestcasesRepository _testcasesRepository = Substitute.For<ITestcasesRepository>();
	private readonly ISupabaseService _supabaseService = Substitute.For<ISupabaseService>();
	private readonly IOptions<SupabaseOptions> _supabaseOptions = Substitute.For<IOptions<SupabaseOptions>>();

	public GetTestcasesQueryHandlerTests() {
		_handler = new GetTestcasesQueryHandler(_logger, _testcasesRepository, _supabaseService, _supabaseOptions);
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		var supabaseOptionsValue = _fixture.Build<SupabaseOptions>()
			.With(x => x.Bucket, "test-bucket")
			.Create();
		_supabaseOptions.Value.Returns(supabaseOptionsValue);
	}

	[Fact]
	public async Task Handle_ShouldReturnTestcaseDtos_WhenValidQueryIsProvided() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var query = new GetTestcasesQuery(problemId);
		var testcases = _fixture.CreateMany<TestCase>(3).ToList();

		_testcasesRepository.GetProblemTestcasesAsync(problemId).Returns(testcases);

		foreach (var testcase in testcases) {
			_supabaseService.ReadFileAsync("test-bucket", testcase.Input).Returns(Task.FromResult(testcase.Input));
			_supabaseService.ReadFileAsync("test-bucket", testcase.ExpectedOutput).Returns(Task.FromResult(testcase.ExpectedOutput));
		}

		// Act
		var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

		// Assert
		result.Should().NotBeNull();
		result.Should().HaveCount(3);

		await _testcasesRepository.Received(1).GetProblemTestcasesAsync(problemId);
	}

	[Fact]
	public async Task Handle_ShouldReturnEmptyCollection_WhenNoTestcasesExist() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var query = new GetTestcasesQuery(problemId);

		_testcasesRepository.GetProblemTestcasesAsync(problemId).Returns(new List<TestCase>());

		// Act
		var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

		// Assert
		result.Should().NotBeNull();
		result.Should().BeEmpty();
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task Handle_ShouldThrowValidationException_WhenProblemIdIsLessThanOrEqualZero(int id) {
		// Arrange
		var query = new GetTestcasesQuery(id);

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("ProblemId must be a positive integer.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenTestcasesAreNull() {
		// Arrange
		var problemId = _fixture.Create<int>();
		var query = new GetTestcasesQuery(problemId);

		_testcasesRepository.GetProblemTestcasesAsync(problemId).Returns((IEnumerable<TestCase>?)null);

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(TestCase)}*");
	}

	[Fact]
	public async Task Handle_ShouldNotCallRepository_WhenProblemIdIsInvalid() {
		// Arrange
		var query = new GetTestcasesQuery(0);

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>();
		await _testcasesRepository.DidNotReceive().GetProblemTestcasesAsync(Arg.Any<int>());
	}
}