using AutoFixture;
using CodeForge.Application.Mappings;
using CodeForge.Domain.Constants;
using CodeForge.Domain.Entities;

namespace Codeforge.Application.Problems.Tests.Unit;

public class ProblemsMappingTests {
	private readonly Fixture _fixture = new();

	public ProblemsMappingTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());
	}

	[Fact]
	public void ToDto_ShouldMapCorrectly_WhenValidProblemIsProvided() {
		// Arrange
		var problem = _fixture.Create<Problem>();
		problem.Tags = _fixture.CreateMany<Tag>(3).ToList();
		problem.Submissions = _fixture.CreateMany<Submission>(2).ToList();
		problem.TestCases = _fixture.CreateMany<TestCase>(2).ToList();

		// Act
		var result = problem.ToDto();

		// Assert
		result.Should().NotBeNull();
		result.Id.Should().Be(problem.Id);
		result.Title.Should().Be(problem.Title);
		result.Description.Should().Be(problem.Description);
		result.Constraints.Should().Be(problem.Constraints);
		result.Difficulty.Should().Be(problem.Difficulty);
		result.Tags.Should().HaveCount(problem.Tags.Count);
		result.Submissions.Should().HaveCount(problem.Submissions.Count);
		result.TestCases.Should().HaveCount(problem.TestCases.Count);
	}

	[Fact]
	public void ToDto_ShouldMapTagsCorrectly_WhenProblemHasTags() {
		// Arrange
		var problem = _fixture.Create<Problem>();
		var tags = new List<Tag>
			{
				new() { Name = "Array" },
				new() { Name = "Dynamic Programming" },
				new() { Name = "Graph" }
			};
		problem.Tags = tags;

		// Act
		var result = problem.ToDto();

		// Assert
		result.Tags.Should().Contain("Array");
		result.Tags.Should().Contain("Dynamic Programming");
		result.Tags.Should().Contain("Graph");
		result.Tags.Should().HaveCount(3);
	}

	[Fact]
	public void ToDto_ShouldMapSubmissionsCorrectly_WhenProblemHasSubmissions() {
		// Arrange
		var problem = _fixture.Create<Problem>();
		var submissions = new List<Submission>
			{
				new() { Verdict = Verdict.Accepted, Language = "C#", SubmittedAt = DateTime.UtcNow },
				new() { Verdict = Verdict.WrongAnswer, Language = "Python", SubmittedAt = DateTime.UtcNow.AddHours(-1) }
			};
		problem.Submissions = submissions;

		// Act
		var result = problem.ToDto();

		// Assert
		result.Submissions.Should().HaveCount(2);
		result.Submissions.Should().Contain(s => s.Verdict == Verdict.Accepted && s.Language == "C#");
		result.Submissions.Should().Contain(s => s.Verdict == Verdict.WrongAnswer && s.Language == "Python");
	}

	[Fact]
	public void ToDto_ShouldHandleEmptyCollections_WhenProblemHasNoTagsOrSubmissions() {
		// Arrange
		var problem = _fixture.Create<Problem>();
		problem.Tags = new List<Tag>();
		problem.Submissions = new List<Submission>();
		problem.TestCases = new List<TestCase>();

		// Act
		var result = problem.ToDto();

		// Assert
		result.Tags.Should().BeEmpty();
		result.Submissions.Should().BeEmpty();
		result.TestCases.Should().BeEmpty();
	}

	[Fact]
	public void ToDto_ShouldMapNullCollectionsToEmptyLists_WhenProblemHasNullCollections() {
		// Arrange
		var problem = _fixture.Create<Problem>();
		problem.Tags = null!;
		problem.Submissions = null!;
		problem.TestCases = null!;

		// Act
		var result = problem.ToDto();

		// Assert
		result.Tags.Should().NotBeNull();
		result.Submissions.Should().NotBeNull();
		result.TestCases.Should().NotBeNull();
	}
}