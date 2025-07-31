using System.Text.Json.Serialization;

namespace CodeForge.Domain.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Verdict {
	Pending,
	Accepted,
	WrongAnswer,
	TimeLimitExceeded,
	MemoryLimitExceeded,
	RuntimeError,
	CompilationError,
}