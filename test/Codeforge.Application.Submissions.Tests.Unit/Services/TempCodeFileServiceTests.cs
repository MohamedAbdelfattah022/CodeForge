using Codeforge.Application.Submissions.Services;
using Codeforge.Domain.Constants;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Tests.Unit.Services;

public class TempCodeFileServiceTests {
	private readonly ILogger<TempCodeFileService> _logger = Substitute.For<ILogger<TempCodeFileService>>();
	private readonly TempCodeFileService _service;

	public TempCodeFileServiceTests() {
		_service = new TempCodeFileService(_logger);
	}

	[Theory]
	[InlineData(Language.Python, ".py")]
	[InlineData(Language.CSharp, ".cs")]
	[InlineData(Language.Cpp, ".cpp")]
	[InlineData("JavaScript", ".txt")]
	public async Task SaveCodeToTempFileAsync_ShouldCreateFileWithCorrectExtension(string language, string expectedExtension) {
		// Arrange
		var code = "test code";

		// Act
		var result = await _service.SaveCodeToTempFileAsync(code, language);

		// Assert
		result.Should().NotBeNullOrEmpty();
		result.Should().EndWith(expectedExtension);
		File.Exists(result).Should().BeTrue();
		var fileContent = await File.ReadAllTextAsync(result);
		fileContent.Should().Be(code);

		// Cleanup
		_service.DeleteTempFile(result);
	}

	[Fact]
	public async Task ReadCodeFromTempFileAsync_ShouldReturnFileContent_WhenFileExists() {
		// Arrange
		var code = "print('Hello World')";
		var language = Language.Python;
		var filePath = await _service.SaveCodeToTempFileAsync(code, language);

		// Act
		var result = await _service.ReadCodeFromTempFileAsync(filePath);

		// Assert
		result.Should().Be(code);

		// Cleanup
		_service.DeleteTempFile(filePath);
	}

	[Fact]
	public async Task ReadCodeFromTempFileAsync_ShouldReturnEmptyContent_WhenFileDoesNotExist() {
		// Arrange
		var nonExistentPath = Path.Combine(Path.GetTempPath(), "non-existent-file.txt");

		// Act
		var content = await _service.ReadCodeFromTempFileAsync(nonExistentPath);

		// Assert
		content.Should().BeEmpty();
	}

	[Fact]
	public async Task DeleteTempFileAsync_ShouldDeleteFile_WhenFileExists() {
		// Arrange
		var code = "print('Hello World')";
		var language = Language.Python;
		var filePath = await _service.SaveCodeToTempFileAsync(code, language);
		File.Exists(filePath).Should().BeTrue();

		// Act
		_service.DeleteTempFile(filePath);

		// Assert
		File.Exists(filePath).Should().BeFalse();
	}

	[Fact]
	public void DeleteTempFileAsync_ShouldNotThrowException_WhenFileDoesNotExist() {
		// Arrange
		var nonExistentPath = Path.Combine(Path.GetTempPath(), "non-existent-file.txt");

		// Act
		var action = () => _service.DeleteTempFile(nonExistentPath);

		// Assert
		action.Should().NotThrow();
	}

	[Theory]
	[InlineData(Language.Python, ".py")]
	[InlineData(Language.CSharp, ".cs")]
	[InlineData(Language.Cpp, ".cpp")]
	[InlineData("UnknownLanguage", ".txt")]
	public async Task GetFileExtension_ShouldReturnCorrectExtension_WhenLanguageIsProvided(string language, string expectedExtension) {
		// Arrange
		var code = "test code";

		// Act
		var filePath = await _service.SaveCodeToTempFileAsync(code, language);

		// Assert
		filePath.Should().EndWith(expectedExtension);

		// Cleanup
		_service.DeleteTempFile(filePath);
	}
}