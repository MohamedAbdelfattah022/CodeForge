using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Constants;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Submissions.Tests.Unit.Mappings;

public class SubmissionsMappingTests {
	private readonly Fixture _fixture = new();

	public SubmissionsMappingTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
	}

	[Fact]
	public void ToDto_ShouldMapCorrectly_WhenValidSubmissionIsProvided() {
		// Arrange
		var submission = _fixture.Create<Submission>();
		submission.ExecutionTime = 150;
		submission.MemoryUsed = 1024;
		submission.Verdict = Verdict.Accepted;

		// Act
		var result = submission.ToDto();

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<SubmissionDto>();
		result.Id.Should().Be(submission.Id);
		result.Code.Should().Be(submission.Code);
		result.Language.Should().Be(submission.Language);
		result.SubmittedAt.Should().Be(submission.SubmittedAt);
		result.ExecutionTime.Should().Be(submission.ExecutionTime.Value);
		result.MemoryUsed.Should().Be(submission.MemoryUsed.Value);
		result.Verdict.Should().Be(submission.Verdict.ToString());
	}

	[Fact]
	public void ToDto_ShouldReturnZeroValues_WhenSubmissionHasNoExecutionData() {
		// Arrange
		var submission = _fixture.Create<Submission>();
		submission.ExecutionTime = null;
		submission.MemoryUsed = null;

		// Act
		var result = submission.ToDto();

		// Assert
		result.Should().NotBeNull();
		result.ExecutionTime.Should().Be(0);
		result.MemoryUsed.Should().Be(0);
	}

	[Theory]
	[InlineData(Verdict.Accepted)]
	[InlineData(Verdict.WrongAnswer)]
	[InlineData(Verdict.TimeLimitExceeded)]
	[InlineData(Verdict.MemoryLimitExceeded)]
	[InlineData(Verdict.RuntimeError)]
	[InlineData(Verdict.CompilationError)]
	public void ToDto_ShouldMapVerdictCorrectly_WhenSubmissionHasDifferentVerdicts(Verdict verdict) {
		// Arrange
		var submission = _fixture.Create<Submission>();
		submission.Verdict = verdict;

		// Act
		var result = submission.ToDto();

		// Assert
		result.Verdict.Should().Be(verdict.ToString());
	}

	[Theory]
	[InlineData(Language.Python)]
	[InlineData(Language.CSharp)]
	[InlineData(Language.Cpp)]
	public void ToDto_ShouldMapLanguageCorrectly_WhenSubmissionHasDifferentLanguages(string language) {
		// Arrange
		var submission = _fixture.Create<Submission>();
		submission.Language = language;

		// Act
		var result = submission.ToDto();

		// Assert
		result.Language.Should().Be(language);
	}

	[Fact]
	public void ToMetadata_ShouldMapCorrectly_WhenValidSubmissionIsProvided() {
		// Arrange
		var submission = _fixture.Create<Submission>();
		submission.Verdict = Verdict.Accepted;

		// Act
		var result = submission.ToMetadata();

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<SubmissionMetadata>();
		result.Id.Should().Be(submission.Id);
		result.Language.Should().Be(submission.Language);
		result.SubmittedAt.Should().Be(submission.SubmittedAt);
		result.Verdict.Should().Be(submission.Verdict.ToString());
	}

	[Theory]
	[InlineData(Verdict.Accepted)]
	[InlineData(Verdict.WrongAnswer)]
	[InlineData(Verdict.TimeLimitExceeded)]
	[InlineData(Verdict.MemoryLimitExceeded)]
	[InlineData(Verdict.RuntimeError)]
	[InlineData(Verdict.CompilationError)]
	public void ToMetadata_ShouldMapVerdictCorrectly_WhenSubmissionHasDifferentVerdicts(Verdict verdict) {
		// Arrange
		var submission = _fixture.Create<Submission>();
		submission.Verdict = verdict;

		// Act
		var result = submission.ToMetadata();

		// Assert
		result.Verdict.Should().Be(verdict.ToString());
	}

	[Theory]
	[InlineData(Language.Python)]
	[InlineData(Language.CSharp)]
	[InlineData(Language.Cpp)]
	public void ToMetadata_ShouldMapLanguageCorrectly_WhenSubmissionHasDifferentLanguages(string language) {
		// Arrange
		var submission = _fixture.Create<Submission>();
		submission.Language = language;

		// Act
		var result = submission.ToMetadata();

		// Assert
		result.Language.Should().Be(language);
	}

	[Fact]
	public void ToMetadata_ShouldNotIncludeExecutionData_WhenMappingIsPerformed() {
		// Arrange
		var submission = _fixture.Create<Submission>();
		submission.ExecutionTime = 150;
		submission.MemoryUsed = 1024;

		// Act
		var result = submission.ToMetadata();

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<SubmissionMetadata>();
	}

	[Fact]
	public void ToDto_ShouldPreserveAllSubmissionProperties_WhenMappingIsPerformed() {
		// Arrange
		var submission = _fixture.Create<Submission>();
		submission.ExecutionTime = 200;
		submission.MemoryUsed = 2048;
		submission.Verdict = Verdict.WrongAnswer;

		// Act
		var result = submission.ToDto();

		// Assert
		result.Should().NotBeNull();
		result.Id.Should().Be(submission.Id);
		result.Code.Should().Be(submission.Code);
		result.Language.Should().Be(submission.Language);
		result.SubmittedAt.Should().Be(submission.SubmittedAt);
		result.ExecutionTime.Should().Be(submission.ExecutionTime.Value);
		result.MemoryUsed.Should().Be(submission.MemoryUsed.Value);
		result.Verdict.Should().Be(submission.Verdict.ToString());
	}
}