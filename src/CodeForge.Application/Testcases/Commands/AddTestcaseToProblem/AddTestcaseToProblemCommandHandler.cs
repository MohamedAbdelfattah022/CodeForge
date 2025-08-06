using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Testcases.Commands.AddTestcaseToProblem;

public class AddTestcaseToProblemCommandHandler(
	ILogger<AddTestcaseToProblemCommandHandler> logger,
	ITestcasesRepository testcasesRepository,
	IProblemsRepository problemsRepository) : IRequestHandler<AddTestcaseToProblemCommand, int> {
	public async Task<int> Handle(AddTestcaseToProblemCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("AddTestcaseToProblemCommandHandler.Handle called with request: {@Request}", request);

		if (request.ProblemId <= 0) throw new ValidationException("ProblemId must be a positive integer.");

		var isProblemExist = await problemsRepository.ExistsAsync(request.ProblemId);
		if (!isProblemExist) throw new NotFoundException(nameof(Problem), request.ProblemId.ToString());

		var testcase = request.ToTestCase();
		var testcaseId = await testcasesRepository.CreateAsync(testcase);

		return testcaseId;
	}
}