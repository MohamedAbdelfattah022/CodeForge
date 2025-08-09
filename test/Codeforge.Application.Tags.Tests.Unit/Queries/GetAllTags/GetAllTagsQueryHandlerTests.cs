using Codeforge.Application.Mappings;
using Codeforge.Application.Tags.Queries.GetAllTags;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Tags.Tests.Unit.Queries.GetAllTags;

public class GetAllTagsQueryHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<GetAllTagsQueryHandler> _logger = Substitute.For<ILogger<GetAllTagsQueryHandler>>();
	private readonly ITagsRepository _tagsRepository = Substitute.For<ITagsRepository>();
	private readonly GetAllTagsQueryHandler _handler;

	public GetAllTagsQueryHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new GetAllTagsQueryHandler(_logger, _tagsRepository);
	}

	[Fact]
	public async Task Handle_ShouldReturnTagDtos_WhenTagsExist() {
		// Arrange
		var tags = _fixture.CreateMany<Tag>(3).ToList();
		var query = new GetAllTagsQuery { PageNumber = 1, PageSize = 10 };

		_tagsRepository.GetAllAsync(query.PageNumber, query.PageSize).Returns((tags, tags.Count));

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.Should().HaveCount(tags.Count);
		result.Select(d => d.Id).Should().BeEquivalentTo(tags.Select(t => t.Id));
		result.Select(d => d.Name).Should().BeEquivalentTo(tags.Select(t => t.Name));
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenNoTagsExist() {
		// Arrange
		var query = new GetAllTagsQuery();

		_tagsRepository.GetAllAsync(Arg.Any<int>(), Arg.Any<int>()).Returns((null, 0));

		// Act
		var act = () => _handler.Handle(query, CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<NotFoundException>();
	}
}