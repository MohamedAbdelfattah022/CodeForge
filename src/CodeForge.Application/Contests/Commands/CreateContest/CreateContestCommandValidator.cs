using FluentValidation;

namespace Codeforge.Application.Contests.Commands.CreateContest;

public class CreateContestCommandValidator : AbstractValidator<CreateContestCommand> {
	public CreateContestCommandValidator() {
		RuleFor(x => x.Name)
			.NotEmpty();

		RuleFor(x => x.Description)
			.NotEmpty();

		RuleFor(x => x.StartTime)
			.LessThan(x => x.EndTime).WithMessage("StartTime must be before EndTime.");
	}
}