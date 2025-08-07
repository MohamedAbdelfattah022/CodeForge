using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Codeforge.Application.Tags.Commands.UpdateTag;

public class UpdateTagCommand(int id) : IRequest {
	public int Id { get; set; } = id;
	[Required] public required string Name { get; set; }
}