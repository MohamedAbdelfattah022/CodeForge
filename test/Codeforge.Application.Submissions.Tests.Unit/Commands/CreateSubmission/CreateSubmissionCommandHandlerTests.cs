using Codeforge.Application.Submissions.Commands.CreateSubmission;
using Codeforge.Application.Submissions.Messages;
using Codeforge.Application.Users;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Tests.Unit.Commands.CreateSubmission;

public class CreateSubmissionCommandHandlerTests {
	private readonly Fixture _fixture = new();
	private readonly ILogger<CreateSubmissionCommandHandler> _logger = Substitute.For<ILogger<CreateSubmissionCommandHandler>>();
	private readonly IMessageProducer _messageProducer = Substitute.For<IMessageProducer>();
	private readonly ISubmissionsRepository _submissionsRepository = Substitute.For<ISubmissionsRepository>();
	private readonly ITempCodeFileService _tempCodeFileService = Substitute.For<ITempCodeFileService>();
	private readonly ITestcasesRepository _testcasesRepository = Substitute.For<ITestcasesRepository>();
	private readonly IUserContext _userContext = Substitute.For<IUserContext>();
	private readonly CreateSubmissionCommandHandler _handler;

	public CreateSubmissionCommandHandlerTests() {
		_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
			.ForEach(b => _fixture.Behaviors.Remove(b));
		_fixture.Behaviors.Add(new OmitOnRecursionBehavior());

		_handler = new CreateSubmissionCommandHandler(
			_logger, _messageProducer, _submissionsRepository,
			_testcasesRepository, _tempCodeFileService, _userContext);
	}

	[Fact]
	public async Task Handle_ShouldReturnSubmissionId_WhenValidCommandIsProvided() {
		// Arrange
		var command = _fixture.Create<CreateSubmissionCommand>();
		var currentUser = _fixture.Create<CurrentUser>();
		var expectedSubmissionId = _fixture.Create<int>();
		var testcases = _fixture.CreateMany<TestCase>(3).ToList();
		var tempFilePath = _fixture.Create<string>();

		_userContext.GetCurrentUser().Returns(currentUser);
		_submissionsRepository.CreateAsync(Arg.Any<Submission>()).Returns(expectedSubmissionId);
		_testcasesRepository.GetAllProblemTestcasesAsync(command.ProblemId).Returns(testcases);
		_tempCodeFileService.SaveCodeToTempFileAsync(command.Code, command.Language).Returns(tempFilePath);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.Should().Be(expectedSubmissionId);
		await _submissionsRepository.Received(1).CreateAsync(Arg.Is<Submission>(s =>
			s.UserId == currentUser.Id &&
			s.ProblemId == command.ProblemId &&
			s.Code == command.Code &&
			s.Language == command.Language));
	}

	[Fact]
	public async Task Handle_ShouldPublishSubmissionMessage_WhenSubmissionIsCreated() {
		// Arrange
		var command = _fixture.Create<CreateSubmissionCommand>();
		var currentUser = _fixture.Create<CurrentUser>();
		var submissionId = _fixture.Create<int>();
		var testcases = _fixture.CreateMany<TestCase>(2).ToList();
		var tempFilePath = _fixture.Create<string>();


		_userContext.GetCurrentUser().Returns(currentUser);
		_submissionsRepository.CreateAsync(Arg.Any<Submission>()).Returns(submissionId);
		_testcasesRepository.GetAllProblemTestcasesAsync(command.ProblemId).Returns(testcases);
		_tempCodeFileService.SaveCodeToTempFileAsync(command.Code, command.Language).Returns(tempFilePath);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _messageProducer.Received(1).PublishAsync(Arg.Is<SubmissionMessage>(m =>
			m.Id == submissionId &&
			m.Code == tempFilePath &&
			m.Language == command.Language &&
			m.InputUrls.Count == testcases.Count &&
			m.OutputUrls.Count == testcases.Count));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task Handle_ShouldThrowValidationException_WhenProblemIdIsNotPositive(int problemId) {
		// Arrange
		var command = _fixture.Create<CreateSubmissionCommand>();
		command.ProblemId = problemId;

		// Act
		var action = () => _handler.Handle(command, CancellationToken.None);
		
		// Assert
		await action.Should().ThrowAsync<ValidationException>()
			.WithMessage("ProblemId must be greater than 0.");
	}

	[Fact]
	public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenCurrentUserIsNull() {
		// Arrange
		var command = _fixture.Create<CreateSubmissionCommand>();

		_userContext.GetCurrentUser().Returns((CurrentUser?)null);

		// Act
		var action = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<UnauthorizedAccessException>()
			.WithMessage("User is not authenticated.");
	}

	[Fact]
	public async Task Handle_ShouldThrowNotFoundException_WhenNoTestcasesFound() {
		// Arrange
		var command = _fixture.Create<CreateSubmissionCommand>();
		var currentUser = _fixture.Create<CurrentUser>();

		_userContext.GetCurrentUser().Returns(currentUser);
		_testcasesRepository.GetAllProblemTestcasesAsync(command.ProblemId).Returns((List<TestCase>?)null);

		// Act
		var action = () => _handler.Handle(command, CancellationToken.None);

		// Assert
		await action.Should().ThrowAsync<NotFoundException>()
			.WithMessage($"Resource {nameof(TestCase)} not found.");
	}

	[Fact]
	public async Task Handle_ShouldSaveCodeToTempFile_WhenSubmissionIsCreated() {
		// Arrange
		var command = _fixture.Create<CreateSubmissionCommand>();
		var currentUser = _fixture.Create<CurrentUser>();
		var submissionId = _fixture.Create<int>();
		var testcases = _fixture.CreateMany<TestCase>(1).ToList();
		var tempFilePath = _fixture.Create<string>();

		_userContext.GetCurrentUser().Returns(currentUser);
		_submissionsRepository.CreateAsync(Arg.Any<Submission>()).Returns(submissionId);
		_testcasesRepository.GetAllProblemTestcasesAsync(command.ProblemId).Returns(testcases);
		_tempCodeFileService.SaveCodeToTempFileAsync(command.Code, command.Language).Returns(tempFilePath);

		// Act
		await _handler.Handle(command, CancellationToken.None);

		// Assert
		await _tempCodeFileService.Received(1).SaveCodeToTempFileAsync(command.Code, command.Language);
	}
}