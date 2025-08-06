using FluentValidation;

namespace Codeforge.Application.Testcases.Commands.AddTestcaseToProblem;

public class AddTestcaseToProblemCommandValidator : AbstractValidator<AddTestcaseToProblemCommand> {
	public AddTestcaseToProblemCommandValidator() {
		RuleFor(x => x.Input)
			.NotEmpty()
			.WithMessage("Input must be provided.");

		RuleFor(x => x.ExpectedOutput)
			.NotEmpty()
			.WithMessage("ExpectedOutput must be provided.");
	}
}