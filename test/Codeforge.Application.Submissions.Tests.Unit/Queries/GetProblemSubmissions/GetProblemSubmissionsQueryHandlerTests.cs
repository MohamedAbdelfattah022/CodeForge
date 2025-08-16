using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Application.Submissions.Queries.GetProblemSubmissions;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Tests.Unit.Queries.GetProblemSubmissions;

public class GetProblemSubmissionsQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<GetProblemSubmissionsQueryHandler> _logger = Substitute.For<ILogger<GetProblemSubmissionsQueryHandler>>();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();
	private readonly ISubmissionsRepository _submissionsRepository = Substitute.For<ISubmissionsRepository>();
	private readonly GetProblemSubmissionsQueryHandler _handler;


	public GetProblemSubmissionsQueryHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new GetProblemSubmissionsQueryHandler(_logger, _problemsRepository, _submissionsRepository);
	}

	[Fact]
	public async Task Handle_ShouldReturnSubmissionMetadataList_WhenValidRequestIsProvided() {
		// Arrange
		var query = _fixture.Create<GetProblemSubmissionsQuery>();
		var submissions = _fixture.CreateMany<Submission>(3).ToList();

		_problemsRepository.ExistsAsync(query.ProblemId).Returns(true);
		_submissionsRepository.GetAllSubmissions(query.ProblemId).Returns(submissions);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<List<SubmissionMetadata>>();
		result.Should().HaveCount(3);
		result.Should().BeEquivalentTo(submissions.Select(s => s.ToMetadata()));
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenProblemDoesNotExist() {
		// Arrange
		var query = _fixture.Create<GetProblemSubmissionsQuery>();

		_problemsRepository.ExistsAsync(query.ProblemId).Returns(false);

		// Act
		var action = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Problem)}*{query.ProblemId}*");
	}

	[Fact]
	public async Task Handle_ShouldGetAllSubmissionsForProblem_WhenValidRequestIsProvided() {
		// Arrange
		var query = _fixture.Create<GetProblemSubmissionsQuery>();
		var submissions = _fixture.CreateMany<Submission>(2).ToList();

		_problemsRepository.ExistsAsync(query.ProblemId).Returns(true);
		_submissionsRepository.GetAllSubmissions(query.ProblemId).Returns(submissions);

		// Act
		await _handler.Handle(query, CancellationToken.None);

		// Assert
		await _submissionsRepository.Received(1).GetAllSubmissions(query.ProblemId);
	}

	[Fact]
	public async Task Handle_ShouldMapSubmissionsToMetadata_WhenValidRequestIsProvided() {
		// Arrange
		var query = _fixture.Create<GetProblemSubmissionsQuery>();
		var submissions = _fixture.CreateMany<Submission>(2).ToList();

		_problemsRepository.ExistsAsync(query.ProblemId).Returns(true);
		_submissionsRepository.GetAllSubmissions(query.ProblemId).Returns(submissions);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().HaveCount(2);
		for (var i = 0; i < submissions.Count; i++) {
			result[i].Id.Should().Be(submissions[i].Id);
			result[i].Language.Should().Be(submissions[i].Language);
			result[i].SubmittedAt.Should().Be(submissions[i].SubmittedAt);
			result[i].Verdict.Should().Be(submissions[i].Verdict.ToString());
		}
	}
}