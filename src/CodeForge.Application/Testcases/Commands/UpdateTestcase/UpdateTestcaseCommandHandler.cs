using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Testcases.Commands.UpdateTestcase;

public class UpdateTestcaseCommandHandler(
	ILogger<UpdateTestcaseCommandHandler> logger,
	ITestcasesRepository testcasesRepository) : IRequestHandler<UpdateTestcaseCommand> {
	public async Task Handle(UpdateTestcaseCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("Updating testcase with ID {TestcaseId}", request.TestcaseId);

		if (request.TestcaseId <= 0) throw new ValidationException("Testcase ID must be greater than 0.");

		var testcase = await testcasesRepository.GetByIdAsync(request.TestcaseId);
		if (testcase is null) throw new NotFoundException(nameof(TestCase), request.TestcaseId.ToString());

		testcase.Input = request.Input ?? testcase.Input;
		testcase.ExpectedOutput = request.ExpectedOutput ?? testcase.ExpectedOutput;
		testcase.IsVisible = request.IsVisible ?? testcase.IsVisible;

		await testcasesRepository.UpdateAsync(testcase);
	}
}