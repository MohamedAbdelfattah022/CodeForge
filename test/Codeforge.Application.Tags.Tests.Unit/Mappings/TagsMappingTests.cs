using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Tags.Tests.Unit.Mappings;

public class TagsMappingTests {
	private readonly Fixture _fixture = new();

	public TagsMappingTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
	}

	[Fact]
	public void ToDto_ShouldMapCorrectly_WhenValidTagIsProvided() {
		// Arrange
		var tag = _fixture.Create<Tag>();

		// Act
		var dto = tag.ToDto();

		// Assert
		dto.Should().NotBeNull();
		dto.Id.Should().Be(tag.Id);
		dto.Name.Should().Be(tag.Name);
	}
}