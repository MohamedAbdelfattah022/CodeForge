using System.Text.Json.Serialization;

namespace Codeforge.Application.Dtos;

public class JudgeResultsDto {
	[JsonPropertyName("overall_verdict")] public string Verdict { get; set; } = nameof(Domain.Constants.Verdict.Pending);

	[JsonPropertyName("execution_time_ms")]
	public int ExecutionTimeMs { get; set; } = 0;

	[JsonPropertyName("used_memory_kb")] public int UsedMemoryKb { get; set; } = 0;
}