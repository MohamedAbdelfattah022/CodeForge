using CodeForge.Application.Dtos;
using CodeForge.Domain.Entities;

namespace CodeForge.Application.Mappings;

public static class ProblemsMapping {
	public static ProblemDto ToDto(this Problem problem) {
		return new ProblemDto
			{
				Id = problem.Id,
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
}