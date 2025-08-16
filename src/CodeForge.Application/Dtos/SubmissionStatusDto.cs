namespace Codeforge.Application.Dtos;

public class SubmissionStatusDto {
	public int SubmissionId { get; set; }
	public string OverallVerdict { get; set; } = string.Empty;
	public int ExecutionTimeMs { get; set; }
	public int UsedMemoryKb { get; set; }
}