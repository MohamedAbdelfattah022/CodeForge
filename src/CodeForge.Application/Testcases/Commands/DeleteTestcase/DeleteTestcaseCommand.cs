using MediatR;

namespace Codeforge.Application.Testcases.Commands.DeleteTestcase;

public class DeleteTestcaseCommand(int testcaseId) : IRequest {
	public int TestcaseId { get; } = testcaseId;
}