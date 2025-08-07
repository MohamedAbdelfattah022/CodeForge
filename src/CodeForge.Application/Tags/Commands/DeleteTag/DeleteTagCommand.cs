using MediatR;

namespace Codeforge.Application.Tags.Commands.DeleteTag;

public class DeleteTagCommand(int id) : IRequest {
	public int Id { get; set; } = id;
}