using CodeForge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeForge.Application.Problems.Commands.CreateProblem;

public class CreateProblemCommandHandler(
	ILogger<CreateProblemCommandHandler> logger,
	IProblemsRepository problemsRepository) : IRequestHandler<CreateProblemCommand, int> {
	public async Task<int> Handle(CreateProblemCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("CreateProblemCommandHandler.Handle called with request: {@Request}", request);

		var problem = request.ToProblem();
		
		var problemId = await problemsRepository.CreateAsync(problem);
		return problemId;
	}
}