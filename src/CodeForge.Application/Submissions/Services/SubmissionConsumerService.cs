using System.Diagnostics;
using System.Text.Json;
using Codeforge.Application.Contests.Services;
using Codeforge.Application.Dtos;
using Codeforge.Application.Submissions.Messages;
using Codeforge.Domain.Constants;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Submissions.Services;

public class SubmissionConsumerService : BackgroundService {
	private readonly ILogger<SubmissionConsumerService> _logger;
	private readonly IMessageConsumer _messageConsumer;
	private readonly string _pythonScriptPath;
	private readonly RabbitMqOptions _rabbitMqOptions;
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ITempCodeFileService _tempCodeFileService;

	public SubmissionConsumerService(
		ILogger<SubmissionConsumerService> logger,
		IOptions<RabbitMqOptions> rabbitMqOptions,
		IMessageConsumer messageConsumer,
		ITempCodeFileService tempCodeFileService,
		IServiceScopeFactory scopeFactory) {
		_logger = logger;
		_rabbitMqOptions = rabbitMqOptions.Value;
		_messageConsumer = messageConsumer;
		_tempCodeFileService = tempCodeFileService;
		_scopeFactory = scopeFactory;

		var baseDir = AppDomain.CurrentDomain.BaseDirectory;
		var parentFullName = Directory.GetParent(baseDir)?.Parent?.Parent?.Parent?.Parent?.FullName
		                     ?? throw new InvalidOperationException("Unable to determine parent directory");
		_pythonScriptPath = Path.Combine(parentFullName, "Codeforge.Application", "Submissions", "scripts", "run.py");
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		await _messageConsumer.ConsumeAsync<SubmissionMessage>(
			_rabbitMqOptions.QueueName,
			RunJudgeAsync,
			stoppingToken);
	}

	private async Task RunJudgeAsync(SubmissionMessage message) {
		_logger.LogInformation("Processing submission {Id}", message.Id);
		var judgeResults = new JudgeResultsDto();

		try {
			foreach (var (inputUrl, outputUrl) in message.InputUrls.Zip(message.OutputUrls)) {
				var judgeResult = await ExecutePythonScript(message, inputUrl, outputUrl);
				UpdateJudgeResults(judgeResults, judgeResult);

				if (judgeResult.Verdict == nameof(Verdict.Accepted)) continue;
				judgeResults.Verdict = judgeResult.Verdict;
				break;
			}
		}
		finally {
			_tempCodeFileService.DeleteTempFile(message.Code);
		}

		await SaveSubmissionResultAsync(message.Id, judgeResults);
	}

	private async Task<JudgeResultsDto> ExecutePythonScript(SubmissionMessage message, string inputUrl, string outputUrl) {
		var startInfo = new ProcessStartInfo
			{
				FileName = "python",
				Arguments = $"\"{_pythonScriptPath}\" \"{message.Code}\" \"{message.Language}\" \"{inputUrl}\" \"{outputUrl}\"",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

		using var process = new Process();
		process.StartInfo = startInfo;
		process.Start();

		var output = await process.StandardOutput.ReadToEndAsync();
		var error = await process.StandardError.ReadToEndAsync();

		await process.WaitForExitAsync();

		if (process.ExitCode != 0)
			throw new Exception($"Python script failed with exit code {process.ExitCode}: {error}");

		return JsonSerializer.Deserialize<JudgeResultsDto>(output)
		       ?? throw new Exception("Failed to parse judge output");
	}

	private void UpdateJudgeResults(JudgeResultsDto finalResult, JudgeResultsDto result) {
		try {
			finalResult.ExecutionTimeMs = Math.Max(finalResult.ExecutionTimeMs, result.ExecutionTimeMs);
			finalResult.UsedMemoryKb = Math.Max(finalResult.UsedMemoryKb, result.UsedMemoryKb);

			if (result.Verdict == nameof(Verdict.Accepted))
				finalResult.Verdict = nameof(Verdict.Accepted);
			else if (finalResult.Verdict != nameof(Verdict.Accepted))
				finalResult.Verdict = result.Verdict;
		}
		catch (Exception ex) {
			_logger.LogCritical(ex, "Error running judge with message {@Message}", result);
		}
	}

	private async Task SaveSubmissionResultAsync(int submissionId, JudgeResultsDto judgeResultsDto) {
		using var scope = _scopeFactory.CreateScope();
		var submissionsRepository = scope.ServiceProvider.GetRequiredService<ISubmissionsRepository>();
		var standingsUpdateService = scope.ServiceProvider.GetRequiredService<IStandingUpdateService>();

		try {
			var submission = await submissionsRepository.GetByIdAsync(submissionId);

			if (Enum.TryParse<Verdict>(judgeResultsDto.Verdict, true, out var parsedVerdict))
				submission!.Verdict = parsedVerdict;

			submission!.ExecutionTime = judgeResultsDto.ExecutionTimeMs;
			submission.MemoryUsed = judgeResultsDto.UsedMemoryKb;

			if (submission.ContestId != null) submission.Penalty = await CalculatePenaltyAsync(submission, submissionsRepository);

			await submissionsRepository.UpdateAsync(submission);
			if (submission is { ContestId: not null, Verdict: Verdict.Accepted }) await standingsUpdateService.UpdateUserResults(submission);
		}
		catch (Exception ex) {
			_logger.LogError(ex, "Failed to update submission {SubmissionId} with judge result", submissionId);
		}
	}

	private async Task<int?> CalculatePenaltyAsync(Submission submission, ISubmissionsRepository submissionsRepository) {
		using var scope = _scopeFactory.CreateScope();
		var contestsRepository = scope.ServiceProvider.GetRequiredService<IContestsRepository>();

		var contestId = submission.ContestId;
		var problemId = submission.ProblemId;
		var userId = submission.UserId;
		var submittedAt = submission.SubmittedAt;

		if (contestId is null) return null;

		var contest = await contestsRepository.GetByIdAsync(contestId.Value);
		if (contest is null || submittedAt < contest.StartTime || submittedAt > contest.EndTime)
			return null;

		var previousSubmissions = await submissionsRepository.GetContestSubmissionsForUserAndProblemAsync(
			contestId.Value, problemId, userId);

		var hasAcceptedSubmission = previousSubmissions.Any(s => s.Verdict == Verdict.Accepted && s.Id != submission.Id);

		if (hasAcceptedSubmission) return null;

		var timeElapsed = (int)(submittedAt - contest.StartTime).TotalMinutes;
		var incorrectSubmissionsCount =
			previousSubmissions.Count(s => s.Verdict != Verdict.Accepted && s.SubmittedAt < submittedAt && s.Id != submission.Id);
		return timeElapsed + (incorrectSubmissionsCount * 20);
	}
}