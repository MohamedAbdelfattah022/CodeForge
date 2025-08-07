using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Codeforge.Application.Tags.Commands.CreateTag;

public class CreateTagCommand : IRequest<int> {
	[Required] public required string Name { get; set; }
}