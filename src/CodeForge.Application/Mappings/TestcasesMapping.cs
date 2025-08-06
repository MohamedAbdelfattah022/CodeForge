using Codeforge.Application.Dtos;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Mappings;

public static class TestcasesMapping {
	public static TestcaseDto ToDto(this TestCase testCase) {
		return new TestcaseDto
			{
				Id = testCase.Id,
				Input = testCase.Input,
				ExpectedOutput = testCase.ExpectedOutput
			};
	}
}