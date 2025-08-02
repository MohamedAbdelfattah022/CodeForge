using FluentValidation;

namespace CodeForge.Application.Problems.Commands.CreateProblem;

public class CreateProblemCommandValidator : AbstractValidator<CreateProblemCommand> {
	public CreateProblemCommandValidator() {
		RuleFor(x => x.Difficulty)
			.IsInEnum()
			.WithMessage("Difficulty must be a valid enum value.");
		
		RuleFor(x => x.Title)
			.NotEmpty();
		
		RuleFor(x => x.Description)
			.NotEmpty();
		
		RuleFor(x => x.Constraints)
			.NotEmpty();
	}
}