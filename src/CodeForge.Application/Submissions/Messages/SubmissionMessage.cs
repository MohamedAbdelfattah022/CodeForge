namespace Codeforge.Application.Submissions.Messages;

public record SubmissionMessage(int Id, string Code, string Language, List<string> InputUrls, List<string> OutputUrls);