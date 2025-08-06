using MediatR;

namespace Codeforge.Application.Testcases.Commands.UpdateTestcase;

public class UpdateTestcaseCommand : IRequest {
	public int TestcaseId { get; set; }
	public string? Input { get; set; }
	public string? ExpectedOutput { get; set; }
	public bool? IsVisible { get; set; }
}