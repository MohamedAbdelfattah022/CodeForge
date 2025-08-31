using Codeforge.Application.Dtos;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Mappings;

public static class ProblemsMapping {
	public static ProblemDto ToDto(this Problem problem) {
		return new ProblemDto
			{
				Id = problem.Id,
				ContestId = problem.ContestId,
				Title = problem.Title,
				Description = problem.Description,
				Difficulty = problem.Difficulty,
				Constraints = problem.Constraints,
				Tags = (problem.Tags ?? Enumerable.Empty<Tag>()).Select(t => t.Name).ToList(),
				Submissions = (problem.Submissions ?? Enumerable.Empty<Submission>()).Select(s => new ProblemDtoSubmission
					{
						Verdict = s.Verdict,
						Language = s.Language,
						SubmittedAt = s.SubmittedAt
					}).ToList(),
				TestCases = (problem.TestCases ?? Enumerable.Empty<TestCase>()).ToList()
			};
	}

	public static ProblemForContestDto ToProblemForContestDto(this Problem problem) {
		return new ProblemForContestDto
			{
				Id = problem.Id,
				Title = problem.Title,
				Difficulty = problem.Difficulty
			};
	}
	
	public static ProblemResult ToProblemResult(this Problem problem, int standingId) {
		return new ProblemResult
			{
				ProblemId = problem.Id,
				StandingId = standingId
			};
	}
}