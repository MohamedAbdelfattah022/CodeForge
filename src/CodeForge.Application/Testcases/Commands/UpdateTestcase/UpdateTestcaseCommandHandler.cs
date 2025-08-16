using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Testcases.Commands.UpdateTestcase;

public class UpdateTestcaseCommandHandler(
	ILogger<UpdateTestcaseCommandHandler> logger,
	ITestcasesRepository testcasesRepository,
	ISupabaseService supabaseService,
	ITempCodeFileService fileService,
	IOptions<SupabaseOptions> supabaseOptions) : IRequestHandler<UpdateTestcaseCommand> {
	public async Task Handle(UpdateTestcaseCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("Updating testcase with ID {TestcaseId}", request.TestcaseId);

		if (request.TestcaseId <= 0) throw new ValidationException("Testcase ID must be greater than 0.");

		var testcase = await testcasesRepository.GetByIdAsync(request.TestcaseId);
		if (testcase is null) throw new NotFoundException(nameof(TestCase), request.TestcaseId.ToString());


		if (request.Input is not null)
			await UpdateSupabaseFileAsync(supabaseOptions.Value.Bucket, testcase.Input, request.Input);

		if (request.ExpectedOutput is not null)
			await UpdateSupabaseFileAsync(supabaseOptions.Value.Bucket, testcase.ExpectedOutput, request.ExpectedOutput);

		testcase.IsVisible = request.IsVisible ?? testcase.IsVisible;

		await testcasesRepository.UpdateAsync(testcase);
	}

	private async Task UpdateSupabaseFileAsync(string bucket, string fileName, string content) {
		var tempFile = await fileService.CreateFileWithNameAsync(fileName, content);
		await supabaseService.UploadOrUpdateFileAsync(bucket, tempFile);
		fileService.DeleteTempFile(tempFile);
	}
}