using Codeforge.Application.Submissions.Messages;
using Codeforge.Application.Users;
using Codeforge.Domain.Entities;
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
	ITestcasesRepository testcasesRepository,
	ITempCodeFileService tempCodeFileService,
	IUserContext userContext) : IRequestHandler<CreateSubmissionCommand, int> {
	public async Task<int> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("Creating submission for problem {ProblemId} with language {Language}", request.ProblemId, request.Language);

		if (request.ProblemId <= 0) throw new ValidationException("ProblemId must be greater than 0.");
		
		var user = userContext.GetCurrentUser();
		if(user is null) throw new UnauthorizedAccessException("User is not authenticated.");
		
		var submission = request.ToSubmission(user.Id);
		var id = await submissionsRepository.CreateAsync(submission);

		var tests = (await testcasesRepository.GetAllProblemTestcasesAsync(request.ProblemId))?.ToList() ??
		            throw new NotFoundException(nameof(TestCase));
		
		var tempFilePath = await tempCodeFileService.SaveCodeToTempFileAsync(request.Code, request.Language);

		var message = new SubmissionMessage(
			Id: id,
			Code: tempFilePath,
			Language: request.Language,
			InputUrls: tests.Select(t => t.Input).ToList(),
			OutputUrls: tests.Select(t => t.ExpectedOutput).ToList()
		);
		await messageProducer.PublishAsync(message);

		return id;
	}
}