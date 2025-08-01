using CodeForge.Application.Dtos;
using CodeForge.Application.Mappings;
using CodeForge.Domain.Entities;
using CodeForge.Domain.Exceptions;
using CodeForge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeForge.Application.Testcases.Queries.GetProblemTestcase;

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