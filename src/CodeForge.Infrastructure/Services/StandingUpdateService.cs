using Codeforge.Application.Standings.Services;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Repositories;
using Hangfire;

namespace Codeforge.Infrastructure.Services;

public class StandingUpdateService(
	IRecurringJobManager recurringJob,
	IBackgroundJobClient jobClient,
	IRedisCacheService redisCache,
	IContestsRepository contestsRepository)
	: IStandingUpdateService {
	public void ScheduleStandingsUpdate(DateTime startTime, DateTime endTime, int standingsId, string jobId) {
		jobClient.Schedule<IStandingUpdateService>(
			job => job.StartStandingsUpdateRecurringJob(standingsId, jobId),
			startTime);

		jobClient.Schedule<IStandingUpdateService>(
			job => job.StopStandingsUpdateAsync(standingsId, jobId),
			endTime);
	}

	public void StartStandingsUpdateRecurringJob(int standingsId, string jobId) {
		recurringJob.AddOrUpdate(jobId, () => UpdateStandings(standingsId, null), "*/1 * * * *");
	}

	public async Task StopStandingsUpdateAsync(int standingsId, string jobId) {
		recurringJob.RemoveIfExists(jobId);
		await UpdateStandings(standingsId, TimeSpan.FromMinutes(30));
	}

	public async Task UpdateStandings(int standingsId, TimeSpan? absoluteExpiration = null) {
		var standings = await contestsRepository.GetStandingsAsync(standingsId);
		if (standings is null)
			throw new NotFoundException(nameof(Standing), standingsId.ToString());

		await redisCache.SetAsync($"standings:{standingsId}", standings, absoluteExpiration);
	}
}