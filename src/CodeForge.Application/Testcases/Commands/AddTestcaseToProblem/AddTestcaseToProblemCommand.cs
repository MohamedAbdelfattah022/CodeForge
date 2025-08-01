using CodeForge.Domain.Entities;
using MediatR;

namespace CodeForge.Application.Testcases.Commands.AddTestcaseToProblem;

public class AddTestcaseToProblemCommand : IRequest<int> {
	public int ProblemId { get; set; }
	public required string Input { get; set; }
	public required string ExpectedOutput { get; set; }
	public bool IsVisible { get; set; }
}

public static class AddTestcaseToProblemCommandExtensions {
	public static TestCase ToTestCase(this AddTestcaseToProblemCommand request) {
		return new TestCase
			{
				ProblemId = request.ProblemId,
				Input = request.Input,
				ExpectedOutput = request.ExpectedOutput,
				IsVisible = request.IsVisible
			};
	}
}