using AutoFixture;
using CodeForge.Application.Problems.Commands.CreateProblem;
using CodeForge.Domain.Constants;

namespace Codeforge.Application.Problems.Tests.Unit;

public class CreateProblemCommandValidatorTests {
	private readonly Fixture _fixture = new();

	[Fact]
	public void Validate_ShouldPass_WhenAllRequiredFieldsAreProvided() {
		// Arrange
		var validator = new CreateProblemCommandValidator();
		var command = _fixture.Create<CreateProblemCommand>();

		// Act
		var result = validator.Validate(command);

		// Assert
		result.IsValid.Should().BeTrue();
		result.Errors.Should().BeEmpty();
	}

	[Theory]
	[InlineData("", "Description", "Constraints", Difficulty.Easy, "Title")]
	[InlineData("Title", "", "Constraints", Difficulty.Easy, "Description")]
	[InlineData("Title", "Description", "", Difficulty.Easy, "Constraints")]
	public void Validate_ShouldFail_WhenRequiredFieldIsEmpty(string title, string description, string constraints, Difficulty difficulty,
		string expectedField) {
		// Arrange
		var validator = new CreateProblemCommandValidator();
		var command = new CreateProblemCommand
			{
				Title = title,
				Description = description,
				Constraints = constraints,
				Difficulty = difficulty
			};

		// Act
		var result = validator.Validate(command);

		// Assert
		result.IsValid.Should().BeFalse();
		result.Errors.Should().ContainSingle(e => e.PropertyName == expectedField);
	}

	[Theory]
	[InlineData("   ", "Description", "Constraints", Difficulty.Easy, "Title")]
	[InlineData("Title", "   ", "Constraints", Difficulty.Easy, "Description")]
	[InlineData("Title", "Description", "   ", Difficulty.Easy, "Constraints")]
	public void Validate_ShouldFail_WhenRequiredFieldIsWhitespace(string title, string description, string constraints, Difficulty difficulty,
		string expectedField) {
		// Arrange
		var validator = new CreateProblemCommandValidator();
		var command = new CreateProblemCommand
			{
				Title = title,
				Description = description,
				Constraints = constraints,
				Difficulty = difficulty
			};

		// Act
		var result = validator.Validate(command);

		// Assert
		result.IsValid.Should().BeFalse();
		result.Errors.Should().ContainSingle(e => e.PropertyName == expectedField);
	}

	[Theory]
	[InlineData((Difficulty)999)]
	[InlineData((Difficulty)(-1))]
	public void Validate_ShouldFail_WhenDifficultyIsInvalid(Difficulty invalidDifficulty) {
		// Arrange
		var validator = new CreateProblemCommandValidator();
		var command = new CreateProblemCommand
			{
				Title = "Valid Title",
				Description = "Valid Description",
				Constraints = "Valid Constraints",
				Difficulty = invalidDifficulty
			};

		// Act
		var result = validator.Validate(command);

		// Assert
		result.IsValid.Should().BeFalse();
		result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateProblemCommand.Difficulty));
		result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Difficulty must be a valid enum value.");
	}

	[Theory]
	[InlineData(Difficulty.Easy)]
	[InlineData(Difficulty.Medium)]
	[InlineData(Difficulty.Hard)]
	public void Validate_ShouldPass_WhenDifficultyIsValid(Difficulty validDifficulty) {
		// Arrange
		var validator = new CreateProblemCommandValidator();
		var command = new CreateProblemCommand
			{
				Title = "Valid Title",
				Description = "Valid Description",
				Constraints = "Valid Constraints",
				Difficulty = validDifficulty
			};

		// Act
		var result = validator.Validate(command);

		// Assert
		result.IsValid.Should().BeTrue();
	}
}