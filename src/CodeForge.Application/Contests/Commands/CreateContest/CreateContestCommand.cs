using Codeforge.Domain.Constants;
using Codeforge.Domain.Entities;
using MediatR;

namespace Codeforge.Application.Contests.Commands.CreateContest;

public class CreateContestCommand : IRequest<int> {
	public required string Name { get; set; }
	public string Description { get; set; } = string.Empty;
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }
}

public static class CreateContestCommandMapping {
	public static Contest ToContest(this CreateContestCommand command) {
		return new Contest
			{
				Name = command.Name,
				Description = command.Description,
				StartTime = command.StartTime,
				EndTime = command.EndTime
			};
	}
}