using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Testcases.Queries.GetProblemTestcase;

public class GetProblemTestcaseQueryHandler(
	ILogger<GetProblemTestcaseQueryHandler> logger,
	ITestcasesRepository testcasesRepository) : IRequestHandler<GetProblemTestcaseQuery, TestcaseDto> {
	public async Task<TestcaseDto> Handle(GetProblemTestcaseQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("Handling GetProblemTestcaseQuery for Testcase ID: {TestcaseId}", request.TestcaseId);

		if (request.ProblemId <= 0 || request.TestcaseId <= 0)
			throw new ValidationException("ProblemId and TestcaseId must be greater than zero.");

		var testcase = await testcasesRepository.GetProblemTestcaseByIdAsync(request.ProblemId, request.TestcaseId);

		if (testcase is null) throw new NotFoundException(nameof(TestCase), request.TestcaseId.ToString());

		return testcase.ToDto();
	}
}