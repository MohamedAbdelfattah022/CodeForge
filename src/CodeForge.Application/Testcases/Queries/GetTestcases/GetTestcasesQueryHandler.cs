using CodeForge.Application.Dtos;
using CodeForge.Application.Mappings;
using CodeForge.Domain.Entities;
using CodeForge.Domain.Exceptions;
using CodeForge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CodeForge.Application.Testcases.Queries.GetTestcases;

public class GetTestcasesQueryHandler(
	ILogger<GetTestcasesQueryHandler> logger,
	ITestcasesRepository testcasesRepository) : IRequestHandler<GetTestcasesQuery, IEnumerable<TestcaseDto>> {
	public async Task<IEnumerable<TestcaseDto>> Handle(GetTestcasesQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("GetTestcasesQueryHandler.Handle called with request: {@Request}", request);

		if (request.ProblemId <= 0) throw new ValidationException("ProblemId must be a positive integer.");

		var testcases =
			await testcasesRepository.GetProblemTestcasesAsync(request.ProblemId);

		if (testcases is null) throw new NotFoundException(nameof(TestCase));

		var results = testcases.Select(tc => tc.ToDto()).ToList();

		return results;
	}
}