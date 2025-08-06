using Codeforge.Application.Testcases.Commands.AddTestcaseToProblem;
using FluentValidation.TestHelper;

namespace Codeforge.Application.Testcases.Tests.Unit.Commands.AddTestcaseToProblem;

public class AddTestcaseToProblemCommandValidatorTests {
	private readonly Fixture _fixture = new();
	private readonly AddTestcaseToProblemCommandValidator _validator = new();

	[Fact]
	public void Validate_ShouldPass_WhenValidCommandIsProvided() {
		// Arrange
		var command = _fixture.Create<AddTestcaseToProblemCommand>();

		// Act
		var result = _validator.TestValidate(command);

		// Assert
		result.ShouldNotHaveAnyValidationErrors();
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	[InlineData("   ")]
	public void Validate_ShouldFail_WhenInputIsInvalid(string? input) {
		// Arrange
		var command = _fixture.Create<AddTestcaseToProblemCommand>();
		command.Input = input!;

		// Act
		var result = _validator.TestValidate(command);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.Input)
			.WithErrorMessage("Input must be provided.");
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	[InlineData("   ")]
	public void Validate_ShouldFail_WhenExpectedOutputIsInvalid(string? expectedOutput) {
		// Arrange
		var command = _fixture.Create<AddTestcaseToProblemCommand>();
		command.ExpectedOutput = expectedOutput!;

		// Act
		var result = _validator.TestValidate(command);

		// Assert
		result.ShouldHaveValidationErrorFor(x => x.ExpectedOutput)
			.WithErrorMessage("ExpectedOutput must be provided.");
	}


	[Fact]
	public void Validate_ShouldPass_WhenBothInputAndExpectedOutputAreValid() {
		// Arrange
		var command = _fixture.Create<AddTestcaseToProblemCommand>();
		command.Input = "test input";
		command.ExpectedOutput = "test output";

		// Act
		var result = _validator.TestValidate(command);

		// Assert
		result.ShouldNotHaveValidationErrorFor(x => x.Input);
		result.ShouldNotHaveValidationErrorFor(x => x.ExpectedOutput);
	}
}