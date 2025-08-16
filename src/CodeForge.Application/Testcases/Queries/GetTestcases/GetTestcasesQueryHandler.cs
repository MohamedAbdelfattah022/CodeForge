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

namespace Codeforge.Application.Testcases.Queries.GetTestcases;

public class GetTestcasesQueryHandler(
	ILogger<GetTestcasesQueryHandler> logger,
	ITestcasesRepository testcasesRepository,
	ISupabaseService supabaseService,
	IOptions<SupabaseOptions> supabaseOptions) : IRequestHandler<GetTestcasesQuery, IEnumerable<TestcaseDto>> {
	public async Task<IEnumerable<TestcaseDto>> Handle(GetTestcasesQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("GetTestcasesQueryHandler.Handle called with request: {@Request}", request);

		if (request.ProblemId <= 0) throw new ValidationException("ProblemId must be a positive integer.");

		var testcases =
			await testcasesRepository.GetProblemTestcasesAsync(request.ProblemId);

		if (testcases is null) throw new NotFoundException(nameof(TestCase));

		var results = new List<TestcaseDto>();

		foreach (var testcase in testcases) {
			var inputTask = supabaseService.ReadFileAsync(supabaseOptions.Value.Bucket, testcase.Input);
			var expectedOutputTask = supabaseService.ReadFileAsync(supabaseOptions.Value.Bucket, testcase.ExpectedOutput);

			await Task.WhenAll(inputTask, expectedOutputTask);

			testcase.Input = inputTask.Result;
			testcase.ExpectedOutput = expectedOutputTask.Result;

			results.Add(testcase.ToDto());
		}

		return results;
	}
}