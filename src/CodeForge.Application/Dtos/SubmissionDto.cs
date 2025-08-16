namespace Codeforge.Application.Dtos;

public class SubmissionDto {
	public int Id { get; set; }
	public required string Code { get; set; }
	public required string Language { get; set; }
	public DateTime SubmittedAt { get; set; }
	public int ExecutionTime { get; set; }
	public int MemoryUsed { get; set; }
	public required string Verdict { get; set; }
}

public class SubmissionMetadata {
	public int Id { get; set; }
	public required string Language { get; set; }
	public DateTime SubmittedAt { get; set; }
	public required string Verdict { get; set; }
}