using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Testcases.Tests.Unit.Mappings;

public class TestcasesMappingTests {
	private readonly Fixture _fixture = new();

	public TestcasesMappingTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
	}

	[Fact]
	public void ToDto_ShouldMapCorrectly_WhenValidTestCaseIsProvided() {
		// Arrange
		var testCase = _fixture.Create<TestCase>();

		// Act
		var result = testCase.ToDto();

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<TestcaseDto>();
		result.Id.Should().Be(testCase.Id);
		result.Input.Should().Be(testCase.Input);
		result.ExpectedOutput.Should().Be(testCase.ExpectedOutput);
	}

	[Fact]
	public void ToDto_ShouldMapAllProperties_WhenTestCaseHasAllValues() {
		// Arrange
		var testCase = new TestCase
			{
				Id = 123,
				Input = "test input",
				ExpectedOutput = "test output"
			};

		// Act
		var result = testCase.ToDto();

		// Assert
		result.Id.Should().Be(123);
		result.Input.Should().Be("test input");
		result.ExpectedOutput.Should().Be("test output");
	}

	[Fact]
	public void ToDto_ShouldHandleEmptyStrings_WhenTestCaseHasEmptyValues() {
		// Arrange
		var testCase = new TestCase
			{
				Id = 456,
				Input = "",
				ExpectedOutput = ""
			};

		// Act
		var result = testCase.ToDto();

		// Assert
		result.Id.Should().Be(456);
		result.Input.Should().Be("");
		result.ExpectedOutput.Should().Be("");
	}

	[Fact]
	public void ToDto_ShouldHandleNullStrings_WhenTestCaseHasNullValues() {
		// Arrange
		var testCase = new TestCase
			{
				Id = 789,
				Input = null!,
				ExpectedOutput = null!
			};

		// Act
		var result = testCase.ToDto();

		// Assert
		result.Id.Should().Be(789);
		result.Input.Should().BeNull();
		result.ExpectedOutput.Should().BeNull();
	}

	[Fact]
	public void ToDto_ShouldCreateNewDtoInstance_WhenCalledMultipleTimes() {
		// Arrange
		var testCase = _fixture.Create<TestCase>();

		// Act
		var result1 = testCase.ToDto();
		var result2 = testCase.ToDto();

		// Assert
		result1.Should().NotBeSameAs(result2);
		result1.Id.Should().Be(result2.Id);
		result1.Input.Should().Be(result2.Input);
		result1.ExpectedOutput.Should().Be(result2.ExpectedOutput);
	}

	[Fact]
	public void ToDto_ShouldMapComplexInput_WhenTestCaseHasComplexValues() {
		// Arrange
		var testCase = new TestCase
			{
				Id = 999,
				Input = "5\n1 2 3 4 5\n10 20 30 40 50",
				ExpectedOutput = "15\n150"
			};

		// Act
		var result = testCase.ToDto();

		// Assert
		result.Id.Should().Be(999);
		result.Input.Should().Be("5\n1 2 3 4 5\n10 20 30 40 50");
		result.ExpectedOutput.Should().Be("15\n150");
	}
}