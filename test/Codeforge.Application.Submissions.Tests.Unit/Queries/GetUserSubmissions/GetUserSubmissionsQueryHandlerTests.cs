using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Application.Submissions.Queries.GetUserSubmissions;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Tests.Unit.Queries.GetUserSubmissions;

public class GetUserSubmissionsQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<GetUserSubmissionsQueryHandler> _logger = Substitute.For<ILogger<GetUserSubmissionsQueryHandler>>();
	private readonly ISubmissionsRepository _submissionsRepository = Substitute.For<ISubmissionsRepository>();
	private readonly GetUserSubmissionsQueryHandler _handler;

	public GetUserSubmissionsQueryHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new GetUserSubmissionsQueryHandler(_logger, _submissionsRepository);
	}

	[Fact]
	public async Task Handle_ShouldReturnSubmissionMetadataList_WhenValidRequestIsProvided() {
		// Arrange
		var query = _fixture.Create<GetUserSubmissionsQuery>();
		var submissions = _fixture.CreateMany<Submission>(3).ToList();

		_submissionsRepository.GetUserSubmissionsAsync(query.UserId).Returns(submissions);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<List<SubmissionMetadata>>();
		result.Should().HaveCount(3);
		result.Should().BeEquivalentTo(submissions.Select(s => s.ToMetadata()));
	}

	[Fact]
	public async Task Handle_ShouldReturnEmptyList_WhenNoSubmissionsExist() {
		// Arrange
		var query = _fixture.Create<GetUserSubmissionsQuery>();

		_submissionsRepository.GetUserSubmissionsAsync(query.UserId).Returns([]);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<List<SubmissionMetadata>>();
		result.Should().BeEmpty();
	}

	[Fact]
	public async Task Handle_ShouldGetUserSubmissions_WhenValidRequestIsProvided() {
		// Arrange
		var query = _fixture.Create<GetUserSubmissionsQuery>();
		var submissions = _fixture.CreateMany<Submission>(2).ToList();

		_submissionsRepository.GetUserSubmissionsAsync(query.UserId).Returns(submissions);

		// Act
		await _handler.Handle(query, CancellationToken.None);

		// Assert
		await _submissionsRepository.Received(1).GetUserSubmissionsAsync(query.UserId);
	}
}