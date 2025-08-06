using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Testcases.Commands.DeleteTestcase;

public class DeleteTestcaseCommandHandler(
	ILogger<DeleteTestcaseCommandHandler> logger,
	ITestcasesRepository testcasesRepository)
	: IRequestHandler<DeleteTestcaseCommand> {
	public async Task Handle(DeleteTestcaseCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("DeleteTestcaseCommandHandler.Handle called with request: {@Request}", request);

		if (request.TestcaseId <= 0) throw new ValidationException("TestcaseId must be positive.");

		var testcase = await testcasesRepository.GetByIdAsync(request.TestcaseId);
		if (testcase is null) throw new NotFoundException(nameof(TestCase), request.TestcaseId.ToString());

		await testcasesRepository.DeleteAsync(testcase);
	}
}