namespace CodeForge.Domain.Constants;

public enum Verdict {
	Pending,
	Accepted,
	WrongAnswer,
	TimeLimitExceeded,
	MemoryLimitExceeded,
	RuntimeError,
	CompilationError,
}