using Codeforge.Application.Tags.Queries.GetTagById;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Tests.Unit.Queries.GetTagById;

public class GetTagByIdQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<GetTagByIdQueryHandler> _logger = Substitute.For<ILogger<GetTagByIdQueryHandler>>();
	private readonly ITagsRepository _tagsRepository = Substitute.For<ITagsRepository>();
	private readonly GetTagByIdQueryHandler _handler;

	public GetTagByIdQueryHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new GetTagByIdQueryHandler(_logger, _tagsRepository);
	}

	[Fact]
	public async Task Handle_ShouldReturnTagDto_WhenValidIdIsProvided() {
		// Arrange
		var tag = _fixture.Create<Tag>();
		var query = new GetTagByIdQuery(tag.Id);

		_tagsRepository.GetByIdAsync(tag.Id).Returns(tag);

		// Act
		var dto = await _handler.Handle(query, CancellationToken.None);

		// Assert
		dto.Should().NotBeNull();
		dto.Id.Should().Be(tag.Id);
		dto.Name.Should().Be(tag.Name);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-2)]
	public async Task Handle_ShouldThrowValidationException_WhenIdIsLessThanOrEqualZero(int id) {
		// Arrange
		var query = new GetTagByIdQuery(id);

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>()
			.WithMessage("Tag ID must be greater than zero.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenTagDoesNotExist() {
		// Arrange
		var id = _fixture.Create<int>();
		var query = new GetTagByIdQuery(id);

		_tagsRepository.GetByIdAsync(id).Returns((Tag)null!);

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"*{nameof(Tag)}*{id}*");
	}
}