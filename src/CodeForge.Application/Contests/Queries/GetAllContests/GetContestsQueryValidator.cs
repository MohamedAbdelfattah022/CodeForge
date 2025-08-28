using FluentValidation;

namespace Codeforge.Application.Contests.Queries.GetAllContests;

public class GetContestsQueryValidator : AbstractValidator<GetContestsQuery> {
	public GetContestsQueryValidator() {
		RuleFor(x => x.PageNumber)
			.GreaterThan(0).WithMessage("PageNumber must be a positive integer.");

		RuleFor(x => x.PageSize)
			.GreaterThan(0).WithMessage("PageSize must be a positive integer.");
	}
}