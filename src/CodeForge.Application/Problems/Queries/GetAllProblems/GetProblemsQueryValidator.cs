using FluentValidation;

namespace Codeforge.Application.Problems.Queries.GetAllProblems;

public class GetProblemsQueryValidator : AbstractValidator<GetProblemsQuery> {
	public GetProblemsQueryValidator() {
		RuleFor(x => x.PageNumber)
			.GreaterThan(0).WithMessage("PageNumber must be a positive integer.");

		RuleFor(x => x.PageSize)
			.GreaterThan(0).WithMessage("PageSize must be a positive integer.");
	}
}