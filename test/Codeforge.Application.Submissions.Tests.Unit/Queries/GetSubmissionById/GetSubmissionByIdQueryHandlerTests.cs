using Codeforge.Application.Dtos;
using Codeforge.Application.Submissions.Queries.GetSubmissionById;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Tests.Unit.Queries.GetSubmissionById;

public class GetSubmissionByIdQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<GetSubmissionByIdQueryHandler> _logger = Substitute.For<ILogger<GetSubmissionByIdQueryHandler>>();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();
	private readonly ISubmissionsRepository _submissionsRepository = Substitute.For<ISubmissionsRepository>();
	private readonly GetSubmissionByIdQueryHandler _handler;

	public GetSubmissionByIdQueryHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new GetSubmissionByIdQueryHandler(_logger, _problemsRepository, _submissionsRepository);
	}

	[Fact]
	public async Task Handle_ShouldReturnSubmissionDto_WhenValidRequestIsProvided() {
		// Arrange
		var query = _fixture.Create<GetSubmissionByIdQuery>();
		var submission = _fixture.Create<Submission>();

		_problemsRepository.ExistsAsync(query.ProblemId).Returns(true);
		_submissionsRepository.GetByIdAsync(query.SubmissionId).Returns(submission);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<SubmissionDto>();
		result.Id.Should().Be(submission.Id);
		result.Code.Should().Be(submission.Code);
		result.Language.Should().Be(submission.Language);
		result.SubmittedAt.Should().Be(submission.SubmittedAt);
	}

	[Theory]
	[InlineData(0, 1)]
	[InlineData(-1, 1)]
	[InlineData(1, 0)]
	[InlineData(1, -1)]
	public async Task Handle_ShouldThrowValidationException_WhenIdsAreNotPositive(int problemId, int submissionId) {
		// Arrange
		var query = _fixture.Create<GetSubmissionByIdQuery>();
		query.ProblemId = problemId;
		query.SubmissionId = submissionId;

		// Act
		var action = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<ValidationException>()
			.WithMessage("Invalid problem or submission ID.");
	}


	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenProblemDoesNotExist() {
		// Arrange
		var query = _fixture.Create<GetSubmissionByIdQuery>();

		_problemsRepository.ExistsAsync(query.ProblemId).Returns(false);

		// Act
		var action = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Problem)}*{query.ProblemId}*");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenSubmissionDoesNotExist() {
		// Arrange
		var query = _fixture.Create<GetSubmissionByIdQuery>();

		_problemsRepository.ExistsAsync(query.ProblemId).Returns(true);
		_submissionsRepository.GetByIdAsync(query.SubmissionId).Returns((Submission?)null);

		// Act
		var action = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Submission)}*{query.SubmissionId}*");
	}
}