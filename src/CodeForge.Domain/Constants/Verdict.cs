using System.Text.Json.Serialization;

namespace Codeforge.Domain.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Verdict {
	Pending,
	Accepted,
	WrongAnswer,
	TimeLimitExceeded,
	MemoryLimitExceeded,
	RuntimeError,
	CompilationError
}