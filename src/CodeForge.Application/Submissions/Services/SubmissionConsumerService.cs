using System.Diagnostics;
using System.Text.Json;
using Codeforge.Application.Dtos;
using Codeforge.Application.Submissions.Messages;
using Codeforge.Domain.Constants;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Submissions.Services;

public class SubmissionConsumerService(
	ILogger<SubmissionConsumerService> logger,
	IOptions<RabbitMqOptions> rabbitMqOptions,
	IMessageConsumer messageConsumer,
	IServiceScopeFactory scopeFactory) : BackgroundService {
	protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
		await messageConsumer.ConsumeAsync<SubmissionMessage>(
			queueName: rabbitMqOptions.Value.QueueName,
			messageHandler: RunJudgeAsync,
			cancellationToken: stoppingToken);
	}

	private async Task RunJudgeAsync(SubmissionMessage message) {
		logger.LogInformation("Processing submission {Id}", message.Id);

		var baseDir = AppDomain.CurrentDomain.BaseDirectory;
		var parentFullName = Directory.GetParent(baseDir)
			?.Parent?.Parent?.Parent?.Parent?.FullName!;

		var pythonScriptPath = Path.Combine(
			parentFullName,
			"Codeforge.Application",
			"Submissions",
			"scripts",
			"run.py");

		var finalResult = new JudgeResultsDto();

		foreach (var (inputUrl, outputUrl) in message.InputUrls.Zip(message.OutputUrls)) {
			var startInfo = new ProcessStartInfo
				{
					FileName = "python",
					Arguments = $"\"{pythonScriptPath}\" \"{message.Code}\" \"{message.Language}\" \"{inputUrl}\" \"{outputUrl}\"",
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

			try {
				using var process = new Process();
				process.StartInfo = startInfo;
				process.Start();


				var output = await process.StandardOutput.ReadToEndAsync();
				var error = await process.StandardError.ReadToEndAsync();

				await process.WaitForExitAsync();

				if (process.ExitCode != 0) throw new Exception($"Python script failed with exit code {process.ExitCode}: {error}");

				var result = JsonSerializer.Deserialize<JudgeResultsDto>(output);

				if (result is null) throw new Exception("Failed to parse judge output");

				finalResult.ExecutionTimeMs = Math.Max(finalResult.ExecutionTimeMs, result.ExecutionTimeMs);
				finalResult.UsedMemoryKb = Math.Max(finalResult.UsedMemoryKb, result.UsedMemoryKb);

				if (result.OverallVerdict == nameof(Verdict.Accepted))
					finalResult.OverallVerdict = nameof(Verdict.Accepted);
				else if (finalResult.OverallVerdict != nameof(Verdict.Accepted)) {
					finalResult.OverallVerdict = result.OverallVerdict;
					break;
				}
			}
			catch (Exception ex) {
				logger.LogCritical(ex, "Error running judge with message {@Message}", message);
			}
		}
		
		await SaveSubmissionResultAsync(message.Id, finalResult);
	}

	private async Task SaveSubmissionResultAsync(int submissionId, JudgeResultsDto judgeResultsDto) {
		using var scope = scopeFactory.CreateScope();
		var submissionsRepository = scope.ServiceProvider.GetRequiredService<ISubmissionsRepository>();
		
		try {
			var submission = await submissionsRepository.GetByIdAsync(submissionId);

			if (Enum.TryParse<Verdict>(judgeResultsDto.OverallVerdict, true, out var parsedVerdict))
				submission!.Verdict = parsedVerdict;

			submission!.ExecutionTime = judgeResultsDto.ExecutionTimeMs;
			submission.MemoryUsed = judgeResultsDto.UsedMemoryKb;

			await submissionsRepository.UpdateAsync(submission);
		}
		catch (Exception ex) {
			logger.LogError(ex, "Failed to update submission {SubmissionId} with judge result", submissionId);
		}
	}
}