using Codeforge.Application.Contests.Services;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Repositories;
using Hangfire;
using Microsoft.AspNetCore.Identity;

namespace Codeforge.Infrastructure.Services;

public class StandingUpdateService(
	IRecurringJobManager recurringJob,
	IBackgroundJobClient jobClient,
	IRedisCacheService redisCache,
	IContestsRepository contestsRepository,
	IStandingsRepository standingsRepository,
	UserManager<User> userManager)
	: IStandingUpdateService {
	public void ScheduleStandingsUpdate(DateTime startTime, DateTime endTime, int contestId, string jobId) {
		jobClient.Schedule<IStandingUpdateService>(
			job => job.StartStandingsUpdateRecurringJob(contestId, jobId),
			startTime);

		jobClient.Schedule<IStandingUpdateService>(
			job => job.StopStandingsUpdateAsync(contestId, jobId),
			endTime);
	}

	public void StartStandingsUpdateRecurringJob(int contestId, string jobId) {
		recurringJob.AddOrUpdate(jobId, () => UpdateStandings(contestId, null), "*/1 * * * *");
	}

	public async Task StopStandingsUpdateAsync(int contestId, string jobId) {
		recurringJob.RemoveIfExists(jobId);
		await UpdateStandings(contestId, TimeSpan.FromMinutes(1));
	}

	public async Task UpdateStandings(int contestId, TimeSpan? absoluteExpiration = null) {
		var standings = await contestsRepository.GetStandingsAsync(contestId);
		if (standings is null)
			throw new NotFoundException(nameof(Standing), contestId.ToString());

		await redisCache.SetAsync($"standings:{contestId}", standings, absoluteExpiration);
	}

	public async Task UpdateUserResults(Submission submission) {
		var user = await userManager.FindByIdAsync(submission.UserId);
		if (user is null)
			throw new NotFoundException(nameof(User), submission.UserId);

		if (submission.ContestId is null) return;

		var contest = await contestsRepository.GetByIdAsync(submission.ContestId.Value);
		if (contest is null || contest.EndTime <= DateTime.Now) return;

		var standing = await standingsRepository.GetUserStandings(submission.ContestId!.Value, user.UserName!);
		if (standing is null) {
			standing = new Standing
				{
					ContestId = submission.ContestId.Value,
					UserName = user.UserName!,
					Score = submission.Penalty ?? 0,
					Rank = 0,
					Problems = []
				};
			await standingsRepository.CreateAsync(standing);
		}

		standing.Score = submission.Penalty ?? standing.Score;

		var solvedProblem = contest.Problems.First(p => p.Id == submission.ProblemId);
		var problemResult = solvedProblem.ToProblemResult(standing.Id);

		var existingProblem = standing.Problems.Any(p => p.ProblemId == solvedProblem.Id);
		if (!existingProblem) standing.Problems.Add(problemResult);

		await standingsRepository.UpdateAsync(standing);
	}
}