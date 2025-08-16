using Codeforge.Application.Submissions.Commands.CreateSubmission;
using Codeforge.Application.Users;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Tests.Unit.Commands.CreateSubmission;

public class CreateSubmissionCommandMappingTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<CreateSubmissionCommandHandler> _logger = Substitute.For<ILogger<CreateSubmissionCommandHandler>>();
	private readonly IMessageProducer _messageProducer = Substitute.For<IMessageProducer>();
	private readonly ISubmissionsRepository _submissionsRepository = Substitute.For<ISubmissionsRepository>();
	private readonly ITempCodeFileService _tempCodeFileService = Substitute.For<ITempCodeFileService>();
	private readonly ITestcasesRepository _testcasesRepository = Substitute.For<ITestcasesRepository>();
	private readonly IUserContext _userContext = Substitute.For<IUserContext>();

	[Fact]
	public void ToSubmission_ShouldMapCorrectly_WhenValidCommandIsProvided() {
		// Arrange
		var command = _fixture.Create<CreateSubmissionCommand>();
		var userId = _fixture.Create<string>();

		// Act
		var result = command.ToSubmission(userId);

		// Assert
		result.Should().NotBeNull();
		result.UserId.Should().Be(userId);
		result.ProblemId.Should().Be(command.ProblemId);
		result.Code.Should().Be(command.Code);
		result.Language.Should().Be(command.Language);
		result.SubmittedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
	}

	[Fact]
	public void ToSubmission_ShouldSetSubmittedAtToUtcNow_WhenMappingIsPerformed() {
		// Arrange
		var command = _fixture.Create<CreateSubmissionCommand>();
		var userId = _fixture.Create<string>();

		// Act
		var result = command.ToSubmission(userId);

		// Assert
		result.SubmittedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
		result.SubmittedAt.Kind.Should().Be(DateTimeKind.Utc);
	}
}