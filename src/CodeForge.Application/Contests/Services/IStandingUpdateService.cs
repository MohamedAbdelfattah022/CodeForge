using Codeforge.Domain.Entities;

namespace Codeforge.Application.Contests.Services;

public interface IStandingUpdateService {
	void ScheduleStandingsUpdate(DateTime startTime, DateTime endTime, int contestId, string jobId);
	void StartStandingsUpdateRecurringJob(int contestId, string jobId);
	Task StopStandingsUpdateAsync(int contestId, string jobId);
	Task UpdateUserResults(Submission submission);
}