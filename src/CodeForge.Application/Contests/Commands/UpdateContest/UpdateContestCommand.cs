using Codeforge.Domain.Constants;
using MediatR;

namespace Codeforge.Application.Contests.Commands.UpdateContest;

public class UpdateContestCommand : IRequest {
	public int Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public DateTime? StartTime { get; set; }
	public DateTime? EndTime { get; set; }
}