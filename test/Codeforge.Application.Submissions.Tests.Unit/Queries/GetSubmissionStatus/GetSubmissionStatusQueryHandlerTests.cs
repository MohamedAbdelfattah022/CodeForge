using Codeforge.Application.Dtos;
using Codeforge.Application.Submissions.Queries.GetSubmissionStatus;
using Codeforge.Domain.Constants;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Tests.Unit.Queries.GetSubmissionStatus;

public class GetSubmissionStatusQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<GetSubmissionStatusQueryHandler> _logger = Substitute.For<ILogger<GetSubmissionStatusQueryHandler>>();
	private readonly ISubmissionsRepository _submissionsRepository = Substitute.For<ISubmissionsRepository>();
	private readonly GetSubmissionStatusQueryHandler _handler;

	public GetSubmissionStatusQueryHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new GetSubmissionStatusQueryHandler(_logger, _submissionsRepository);
	}

	[Fact]
	public async Task Handle_ShouldReturnSubmissionStatusDto_WhenValidRequestIsProvided() {
		// Arrange
		var query = _fixture.Create<GetSubmissionStatusQuery>();
		var submission = _fixture.Create<Submission>();
		submission.ExecutionTime = 150;
		submission.MemoryUsed = 1024;
		submission.Verdict = Verdict.Accepted;

		_submissionsRepository.GetByIdAsync(query.SubmissionId).Returns(submission);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<SubmissionStatusDto>();
		result.SubmissionId.Should().Be(submission.Id);
		result.OverallVerdict.Should().Be(submission.Verdict.ToString());
		result.ExecutionTimeMs.Should().Be(submission.ExecutionTime.Value);
		result.UsedMemoryKb.Should().Be(submission.MemoryUsed.Value);
	}

	[Fact]
	public async Task Handle_ShouldReturnZeroValues_WhenSubmissionHasNoExecutionData() {
		// Arrange
		var query = _fixture.Create<GetSubmissionStatusQuery>();
		var submission = _fixture.Create<Submission>();
		submission.ExecutionTime = null;
		submission.MemoryUsed = null;

		_submissionsRepository.GetByIdAsync(query.SubmissionId).Returns(submission);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.ExecutionTimeMs.Should().Be(0);
		result.UsedMemoryKb.Should().Be(0);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task Handle_ShouldThrowValidationException_WhenSubmissionIdIsNotPositive(int submissionId) {
		// Arrange
		var query = _fixture.Create<GetSubmissionStatusQuery>();
		query.SubmissionId = submissionId;

		// Act
		var action = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<ValidationException>()
			.WithMessage("SubmissionId must be positive.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenSubmissionDoesNotExist() {
		// Arrange
		var query = _fixture.Create<GetSubmissionStatusQuery>();

		_submissionsRepository.GetByIdAsync(query.SubmissionId).Returns((Submission?)null);

		// Act & Assert
		var action = () => _handler.Handle(query, CancellationToken.None);
		await action.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Submission)}*{query.SubmissionId}*");
	}

	[Fact]
	public async Task Handle_ShouldGetSubmissionById_WhenValidRequestIsProvided() {
		// Arrange
		var query = _fixture.Create<GetSubmissionStatusQuery>();
		var submission = _fixture.Create<Submission>();

		_submissionsRepository.GetByIdAsync(query.SubmissionId).Returns(submission);

		// Act
		await _handler.Handle(query, CancellationToken.None);

		// Assert
		await _submissionsRepository.Received(1).GetByIdAsync(query.SubmissionId);
	}

	[Theory]
	[InlineData(Verdict.Accepted)]
	[InlineData(Verdict.WrongAnswer)]
	[InlineData(Verdict.TimeLimitExceeded)]
	[InlineData(Verdict.MemoryLimitExceeded)]
	[InlineData(Verdict.RuntimeError)]
	[InlineData(Verdict.CompilationError)]
	public async Task Handle_ShouldReturnCorrectVerdict_WhenSubmissionHasDifferentVerdicts(Verdict verdict) {
		// Arrange
		var query = _fixture.Create<GetSubmissionStatusQuery>();
		var submission = _fixture.Create<Submission>();
		submission.Verdict = verdict;

		_submissionsRepository.GetByIdAsync(query.SubmissionId).Returns(submission);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.OverallVerdict.Should().Be(verdict.ToString());
	}
}