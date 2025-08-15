using Codeforge.Domain.Constants;
using FluentValidation;

namespace Codeforge.Application.Submissions.Commands.CreateSubmission;

public class CreateSubmissionCommandValidator : AbstractValidator<CreateSubmissionCommand> {
	private readonly string[] _validLanguages = [Language.Cpp, Language.Python, Language.CSharp];

	public CreateSubmissionCommandValidator() {
		RuleFor(cmd => cmd.Code)
			.NotEmpty()
			.WithMessage("Code is required");

		RuleFor(cmd => cmd.Language)
			.NotEmpty()
			.WithMessage("Language is required")
			.Must(lang => _validLanguages.Contains(lang))
			.WithMessage($"Language must be one of the following: {string.Join(", ", _validLanguages)}");
	}
}