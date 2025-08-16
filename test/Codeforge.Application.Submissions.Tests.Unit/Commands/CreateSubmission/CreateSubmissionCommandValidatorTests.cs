using Codeforge.Application.Submissions.Commands.CreateSubmission;
using Codeforge.Domain.Constants;

namespace Codeforge.Application.Submissions.Tests.Unit.Commands.CreateSubmission;

public class CreateSubmissionCommandValidatorTests {
	private readonly CreateSubmissionCommandValidator _validator = new();

	[Fact]
	public void Validate_ShouldPass_WhenValidCommandIsProvided() {
		// Arrange
		var command = new CreateSubmissionCommand
			{
				Code = "print('Hello World')",
				Language = Language.Python
			};

		// Act
		var result = _validator.Validate(command);

		// Assert
		result.IsValid.Should().BeTrue();
		result.Errors.Should().BeEmpty();
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	[InlineData("   ")]
	public void Validate_ShouldFail_WhenCodeIsEmpty(string code) {
		// Arrange
		var command = new CreateSubmissionCommand
			{
				Code = code,
				Language = Language.Python
			};

		// Act
		var result = _validator.Validate(command);

		// Assert
		result.IsValid.Should().BeFalse();
		result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateSubmissionCommand.Code));
		result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Code is required");
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	[InlineData("   ")]
	public void Validate_ShouldFail_WhenLanguageIsEmpty(string? language) {
		// Arrange
		var command = new CreateSubmissionCommand
			{
				Code = "print('Hello World')",
				Language = language!
			};

		// Act
		var result = _validator.Validate(command);

		// Assert
		result.IsValid.Should().BeFalse();
		result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateSubmissionCommand.Language));
		result.Errors.Should().ContainSingle(e => e.ErrorMessage == "Language is required");
	}

	[Theory]
	[InlineData("Java")]
	[InlineData("JavaScript")]
	[InlineData("Ruby")]
	[InlineData("Go")]
	[InlineData("Rust")]
	public void Validate_ShouldFail_WhenLanguageIsNotSupported(string language) {
		// Arrange
		var command = new CreateSubmissionCommand
			{
				Code = "print('Hello World')",
				Language = language
			};

		// Act
		var result = _validator.Validate(command);

		// Assert
		result.IsValid.Should().BeFalse();
		result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateSubmissionCommand.Language));
		result.Errors.Should().ContainSingle(e =>
			e.ErrorMessage == $"Language must be one of the following: {Language.Cpp}, {Language.Python}, {Language.CSharp}");
	}

	[Theory]
	[InlineData(Language.Python)]
	[InlineData(Language.CSharp)]
	[InlineData(Language.Cpp)]
	public void Validate_ShouldPass_WhenLanguageIsSupported(string language) {
		// Arrange
		var command = new CreateSubmissionCommand
			{
				Code = "print('Hello World')",
				Language = language
			};

		// Act
		var result = _validator.Validate(command);

		// Assert
		result.IsValid.Should().BeTrue();
		result.Errors.Should().BeEmpty();
	}

	[Fact]
	public void Validate_ShouldFail_WhenBothCodeAndLanguageAreInvalid() {
		// Arrange
		var command = new CreateSubmissionCommand
			{
				Code = "",
				Language = "InvalidLanguage"
			};

		// Act
		var result = _validator.Validate(command);

		// Assert
		result.IsValid.Should().BeFalse();
		result.Errors.Should().HaveCount(2);
		result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateSubmissionCommand.Code));
		result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(CreateSubmissionCommand.Language));
	}
}