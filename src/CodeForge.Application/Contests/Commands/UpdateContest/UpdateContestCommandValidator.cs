using FluentValidation;

namespace Codeforge.Application.Contests.Commands.UpdateContest;

public class UpdateContestCommandValidator : AbstractValidator<UpdateContestCommand> {
	public UpdateContestCommandValidator() {
		RuleFor(x => x.StartTime)
			.LessThan(x => x.EndTime).When(x => x.StartTime.HasValue && x.EndTime.HasValue)
			.WithMessage("StartTime must be before EndTime.");
		RuleFor(x => x.Status)
			.IsInEnum().When(x => x.Status.HasValue).WithMessage("Status must be a valid enum value.");
	}
}