namespace Codeforge.Application.Standings.Services;

public interface IStandingUpdateService {
	void StartStandingsUpdateRecurringJob(int standingsId, string jobId);
	void ScheduleStandingsUpdate(DateTime startTime, DateTime endTime, int standingsId, string jobId);
	Task StopStandingsUpdateAsync(int standingsId, string jobId);
}