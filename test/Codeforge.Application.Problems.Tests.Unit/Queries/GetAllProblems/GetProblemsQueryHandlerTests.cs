using Codeforge.Application.Problems.Queries.GetAllProblems;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Problems.Tests.Unit.Queries.GetAllProblems;

public class GetProblemsQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<GetProblemsQueryHandler> _logger = Substitute.For<ILogger<GetProblemsQueryHandler>>();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();

	public GetProblemsQueryHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
	}

	[Fact]
	public async Task Handle_ShouldReturnPaginationResult_WhenValidQueryIsProvided() {
		// Arrange
		var problems = _fixture.CreateMany<Problem>(3).ToList();
		var totalCount = problems.Count;
		var pageNumber = 1;
		var pageSize = 5;

		var query = new GetProblemsQuery
			{
				PageNumber = pageNumber,
				PageSize = pageSize
			};

		var handler = new GetProblemsQueryHandler(_logger, _problemsRepository);
		_problemsRepository.GetAllAsync(pageNumber, pageSize).Returns((problems, totalCount));

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Data.Should().HaveCount(problems.Count);
		result.TotalItems.Should().Be(totalCount);
		result.PageNumber.Should().Be(pageNumber);
		result.PageSize.Should().Be(pageSize);
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenNoProblemsExist() {
		// Arrange
		var query = new GetProblemsQuery();
		var handler = new GetProblemsQueryHandler(_logger, _problemsRepository);

		_problemsRepository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>()).Returns((null, 0));

		// Act
		var action = () => handler.Handle(query, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<NotFoundException>();
	}
}