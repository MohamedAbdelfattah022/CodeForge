using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Testcases.Commands.DeleteTestcase;

public class DeleteTestcaseCommandHandler(
	ILogger<DeleteTestcaseCommandHandler> logger,
	ITestcasesRepository testcasesRepository,
	ISupabaseService supabaseService,
	IOptions<SupabaseOptions> supabaseOptions)
	: IRequestHandler<DeleteTestcaseCommand> {
	public async Task Handle(DeleteTestcaseCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("DeleteTestcaseCommandHandler.Handle called with request: {@Request}", request);

		if (request.TestcaseId <= 0) throw new ValidationException("TestcaseId must be positive.");

		var testcase = await testcasesRepository.GetByIdAsync(request.TestcaseId);
		if (testcase is null) throw new NotFoundException(nameof(TestCase), request.TestcaseId.ToString());

		var deleteInputTask = supabaseService.DeleteFileAsync(supabaseOptions.Value.Bucket, testcase.Input);
		var deleteOutputTask = supabaseService.DeleteFileAsync(supabaseOptions.Value.Bucket, testcase.ExpectedOutput);
		await Task.WhenAll(deleteInputTask, deleteOutputTask);

		await testcasesRepository.DeleteAsync(testcase);
	}
}