using CodeForge.Application.Testcases.Commands.AddTestcaseToProblem;
using CodeForge.Domain.Entities;

namespace Codeforge.Application.Testcases.Tests.Unit.Commands.AddTestcaseToProblem;

public class AddTestcaseToProblemCommandExtensionsTests {
	private readonly Fixture _fixture = new();

	[Fact]
	public void ToTestCase_ShouldMapCorrectly_WhenValidCommandIsProvided() {
		// Arrange
		var command = _fixture.Create<AddTestcaseToProblemCommand>();

		// Act
		var result = command.ToTestCase();

		// Assert
		result.Should().NotBeNull();
		result.ProblemId.Should().Be(command.ProblemId);
		result.Input.Should().Be(command.Input);
		result.ExpectedOutput.Should().Be(command.ExpectedOutput);
		result.IsVisible.Should().Be(command.IsVisible);
	}

	[Fact]
	public void ToTestCase_ShouldCreateNewTestCase_WhenCommandPropertiesAreSet() {
		// Arrange
		var command = new AddTestcaseToProblemCommand
			{
				ProblemId = 123,
				Input = "test input",
				ExpectedOutput = "test output",
				IsVisible = true
			};

		// Act
		var result = command.ToTestCase();

		// Assert
		result.Should().BeOfType<TestCase>();
		result.ProblemId.Should().Be(123);
		result.Input.Should().Be("test input");
		result.ExpectedOutput.Should().Be("test output");
		result.IsVisible.Should().BeTrue();
	}

	[Fact]
	public void ToTestCase_ShouldMapAllProperties_WhenCommandHasAllValues() {
		// Arrange
		var command = new AddTestcaseToProblemCommand
			{
				ProblemId = 456,
				Input = "complex input with spaces",
				ExpectedOutput = "expected result",
				IsVisible = false
			};

		// Act
		var result = command.ToTestCase();

		// Assert
		result.ProblemId.Should().Be(456);
		result.Input.Should().Be("complex input with spaces");
		result.ExpectedOutput.Should().Be("expected result");
		result.IsVisible.Should().BeFalse();
	}
}