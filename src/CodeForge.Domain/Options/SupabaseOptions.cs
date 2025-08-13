namespace Codeforge.Domain.Options;

public sealed class SupabaseOptions {
	public const string SectionName = "Supabase";
	public string Url { get; set; } = string.Empty;
	public string ApiKey { get; set; } = string.Empty;
	public string Bucket { get; set; } = "testcases";
}