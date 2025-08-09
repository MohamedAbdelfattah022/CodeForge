using Codeforge.Application.Tags.Queries.GetProblemTags;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Tests.Unit.Queries.GetProblemTags;

public class GetProblemTagQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<GetProblemTagQueryHandler> _logger = Substitute.For<ILogger<GetProblemTagQueryHandler>>();
	private readonly IProblemsRepository _problemsRepository = Substitute.For<IProblemsRepository>();
	private readonly GetProblemTagQueryHandler _handler;

	public GetProblemTagQueryHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new GetProblemTagQueryHandler(_logger, _problemsRepository);
	}

	[Fact]
	public async Task Handle_ShouldReturnTagDtos_WhenProblemExists() {
		// Arrange
		var problem = _fixture.Create<Problem>();
		problem.Tags = _fixture.CreateMany<Tag>(3).ToList();
		var query = new GetProblemTagQuery(problem.Id);

		_problemsRepository.GetByIdAsync(problem.Id).Returns(problem);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().HaveCount(problem.Tags.Count);
		result.Select(t => t.Id).Should().BeEquivalentTo(problem.Tags.Select(t => t.Id));
		result.Select(t => t.Name).Should().BeEquivalentTo(problem.Tags.Select(t => t.Name));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-3)]
	public async Task Handle_ShouldThrowValidationException_WhenIdIsLessThanOrEqualZero(int id) {
		// Arrange
		var query = new GetProblemTagQuery(id);

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("Problem Id must be greater than zero.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenProblemDoesNotExist() {
		// Arrange
		var id = _fixture.Create<int>();
		var query = new GetProblemTagQuery(id);

		_problemsRepository.GetByIdAsync(id).Returns((Problem)null!);

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Problem)}*{id}*");
	}
}