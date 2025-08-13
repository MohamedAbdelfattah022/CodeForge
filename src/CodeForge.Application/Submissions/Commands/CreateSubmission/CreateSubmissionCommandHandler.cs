using Codeforge.Application.Submissions.Messages;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Commands.CreateSubmission;

public class CreateSubmissionCommandHandler(
	ILogger<CreateSubmissionCommandHandler> logger,
	IMessageProducer messageProducer,
	ISubmissionsRepository submissionsRepository,
	ITestcasesRepository testcasesRepository) : IRequestHandler<CreateSubmissionCommand, int> {
	public async Task<int> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("Creating submission for problem {ProblemId} with language {Language}", request.ProblemId, request.Language);

		if (request.ProblemId <= 0) throw new ValidationException("ProblemId must be greater than 0.");

		var submission = request.ToSubmission("e84f4f28-a17d-4734-97c7-78c765616ce3");

		var id = await submissionsRepository.CreateAsync(submission);

		var tests = (await testcasesRepository.GetProblemTestcasesAsync(request.ProblemId))?.ToList() ??
		            throw new NotFoundException("No testcases found for the specified problem.");

		var message = new SubmissionMessage(
			Id: id,
			Code: request.Code,
			Language: request.Language,
			InputUrls: tests.Select(t => t.Input).ToList(),
			OutputUrls: tests.Select(t => t.ExpectedOutput).ToList()
		);
		await messageProducer.PublishAsync(message);

		return id;
	}
}