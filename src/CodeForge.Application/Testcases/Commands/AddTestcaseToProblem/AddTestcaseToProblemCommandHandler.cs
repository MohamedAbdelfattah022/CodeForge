using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Codeforge.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeforge.Application.Testcases.Commands.AddTestcaseToProblem;

public class AddTestcaseToProblemCommandHandler(
	ILogger<AddTestcaseToProblemCommandHandler> logger,
	ITestcasesRepository testcasesRepository,
	IProblemsRepository problemsRepository,
	ITempCodeFileService fileService,
	ISupabaseService supabaseService,
	IOptions<SupabaseOptions> supabaseOptions) : IRequestHandler<AddTestcaseToProblemCommand, int> {
	public async Task<int> Handle(AddTestcaseToProblemCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("AddTestcaseToProblemCommandHandler.Handle called with request: {@Request}", request);

		if (request.ProblemId <= 0) throw new ValidationException("ProblemId must be a positive integer.");

		var isProblemExist = await problemsRepository.ExistsAsync(request.ProblemId);
		if (!isProblemExist) throw new NotFoundException(nameof(Problem), request.ProblemId.ToString());

		var tmpInputPath = await fileService.SaveCodeToTempFileAsync(request.Input, "txt");
		var tmpExpectedOutputPath = await fileService.SaveCodeToTempFileAsync(request.ExpectedOutput, "txt");

		await supabaseService.UploadOrUpdateFileAsync(supabaseOptions.Value.Bucket, tmpInputPath);
		await supabaseService.UploadOrUpdateFileAsync(supabaseOptions.Value.Bucket, tmpExpectedOutputPath);

		request.Input = tmpInputPath.Split("\\").Last();
		request.ExpectedOutput = tmpExpectedOutputPath.Split("\\").Last();

		var testcase = request.ToTestCase();
		var testcaseId = await testcasesRepository.CreateAsync(testcase);

		fileService.DeleteTempFile(tmpInputPath);
		fileService.DeleteTempFile(tmpExpectedOutputPath);

		return testcaseId;
	}
}