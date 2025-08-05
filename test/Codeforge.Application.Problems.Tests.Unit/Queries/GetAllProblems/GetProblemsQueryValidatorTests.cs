using CodeForge.Application.Problems.Queries.GetAllProblems;

namespace Codeforge.Application.Problems.Tests.Unit.Queries.GetAllProblems;

public class GetProblemsQueryValidatorTests {
	[Theory]
	[InlineData(1, 10)]
	[InlineData(5, 20)]
	[InlineData(10, 50)]
	public void Validate_ShouldPass_WhenValidPaginationValuesAreProvided(int pageNumber, int pageSize) {
		// Arrange
		var validator = new GetProblemsQueryValidator();
		var query = new GetProblemsQuery
			{
				PageNumber = pageNumber,
				PageSize = pageSize
			};

		// Act
		var result = validator.Validate(query);

		// Assert
		result.IsValid.Should().BeTrue();
		result.Errors.Should().BeEmpty();
	}

	[Theory]
	[InlineData(0, 10, "PageNumber")]
	[InlineData(-1, 10, "PageNumber")]
	[InlineData(-5, 10, "PageNumber")]
	public void Validate_ShouldFail_WhenPageNumberIsInvalid(int pageNumber, int pageSize, string expectedField) {
		// Arrange
		var validator = new GetProblemsQueryValidator();
		var query = new GetProblemsQuery
			{
				PageNumber = pageNumber,
				PageSize = pageSize
			};

		// Act
		var result = validator.Validate(query);

		// Assert
		result.IsValid.Should().BeFalse();
		result.Errors.Should().ContainSingle(e => e.PropertyName == expectedField);
		result.Errors.Should().ContainSingle(e => e.ErrorMessage == "PageNumber must be a positive integer.");
	}

	[Theory]
	[InlineData(1, 0, "PageSize")]
	[InlineData(1, -1, "PageSize")]
	[InlineData(1, -10, "PageSize")]
	public void Validate_ShouldFail_WhenPageSizeIsInvalid(int pageNumber, int pageSize, string expectedField) {
		// Arrange
		var validator = new GetProblemsQueryValidator();
		var query = new GetProblemsQuery
			{
				PageNumber = pageNumber,
				PageSize = pageSize
			};

		// Act
		var result = validator.Validate(query);

		// Assert
		result.IsValid.Should().BeFalse();
		result.Errors.Should().ContainSingle(e => e.PropertyName == expectedField);
		result.Errors.Should().ContainSingle(e => e.ErrorMessage == "PageSize must be a positive integer.");
	}

	[Theory]
	[InlineData(0, 0)]
	[InlineData(-1, -1)]
	[InlineData(-5, -10)]
	public void Validate_ShouldFail_WhenBothPaginationValuesAreInvalid(int pageNumber, int pageSize) {
		// Arrange
		var validator = new GetProblemsQueryValidator();
		var query = new GetProblemsQuery
			{
				PageNumber = pageNumber,
				PageSize = pageSize
			};

		// Act
		var result = validator.Validate(query);

		// Assert
		result.IsValid.Should().BeFalse();
		result.Errors.Should().HaveCount(2);
		result.Errors.Should().Contain(e => e.PropertyName == "PageNumber");
		result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
	}
}