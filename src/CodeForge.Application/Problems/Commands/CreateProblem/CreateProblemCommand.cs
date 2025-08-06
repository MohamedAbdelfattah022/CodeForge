using Codeforge.Domain.Constants;
using Codeforge.Domain.Entities;
using MediatR;

namespace Codeforge.Application.Problems.Commands.CreateProblem;

public class CreateProblemCommand : IRequest<int> {
	public string Title { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Constraints { get; set; } = string.Empty;
	public Difficulty Difficulty { get; set; }
}

public static class CreateProblemCommandMapping {
	public static Problem ToProblem(this CreateProblemCommand command) {
		return new Problem
			{
				Title = command.Title,
				Description = command.Description,
				Constraints = command.Constraints,
				Difficulty = command.Difficulty
			};
	}
}