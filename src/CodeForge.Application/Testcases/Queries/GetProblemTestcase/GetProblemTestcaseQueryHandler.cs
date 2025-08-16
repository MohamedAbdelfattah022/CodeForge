using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Testcases.Queries.GetProblemTestcase;

public class GetProblemTestcaseQueryHandler(
	ILogger<GetProblemTestcaseQueryHandler> logger,
	ITestcasesRepository testcasesRepository,
	ISupabaseService supabaseService,
	IOptions<SupabaseOptions> supabaseOptions) : IRequestHandler<GetProblemTestcaseQuery, TestcaseDto> {
	public async Task<TestcaseDto> Handle(GetProblemTestcaseQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("Handling GetProblemTestcaseQuery for Testcase ID: {TestcaseId}", request.TestcaseId);

		if (request.ProblemId <= 0 || request.TestcaseId <= 0)
			throw new ValidationException("ProblemId and TestcaseId must be greater than zero.");

		var testcase = await testcasesRepository.GetProblemTestcaseByIdAsync(request.ProblemId, request.TestcaseId);

		if (testcase is null) throw new NotFoundException(nameof(TestCase), request.TestcaseId.ToString());

		var inputTask = supabaseService.ReadFileAsync(supabaseOptions.Value.Bucket, testcase.Input);
		var expectedOutputTask = supabaseService.ReadFileAsync(supabaseOptions.Value.Bucket, testcase.ExpectedOutput);

		await Task.WhenAll(inputTask, expectedOutputTask);

		testcase.Input = inputTask.Result;
		testcase.ExpectedOutput = expectedOutputTask.Result;

		return testcase.ToDto();
	}
}