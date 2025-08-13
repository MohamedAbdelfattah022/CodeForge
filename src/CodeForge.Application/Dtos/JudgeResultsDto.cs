using System.Text.Json.Serialization;
using Codeforge.Domain.Constants;

namespace Codeforge.Application.Dtos;

public class JudgeResultsDto {
	[JsonPropertyName("overall_verdict")] public string OverallVerdict { get; set; } = nameof(Verdict.Pending);

	[JsonPropertyName("execution_time_ms")]
	public int ExecutionTimeMs { get; set; } = 0;

	[JsonPropertyName("used_memory_kb")] public int UsedMemoryKb { get; set; } = 0;
}