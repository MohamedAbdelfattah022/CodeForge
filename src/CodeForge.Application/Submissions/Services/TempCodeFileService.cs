using System.Text;
using Codeforge.Domain.Constants;
using Codeforge.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Submissions.Services;

public class TempCodeFileService(ILogger<TempCodeFileService> logger) : ITempCodeFileService {
	private static readonly Dictionary<string, string> LanguageExtensions = new(StringComparer.OrdinalIgnoreCase)
		{
			[Language.Python] = ".py",
			[Language.CSharp] = ".cs",
			[Language.Cpp] = ".cpp"
		};

	public async Task<string> SaveCodeToTempFileAsync(string code, string language) {
		var extension = GetFileExtension(language);
		var fileName = $"{Guid.NewGuid()}{extension}";
		var tempPath = Path.Combine(Path.GetTempPath(), fileName);

		try {
			await File.WriteAllTextAsync(tempPath, code, Encoding.UTF8);
			logger.LogDebug("Saved code to temporary file: {FilePath}", tempPath);
			return tempPath;
		}
		catch (Exception ex) {
			logger.LogError(ex, "Failed to save code to temporary file: {FilePath}", tempPath);
			throw;
		}
	}

	public Task<string> CreateFileWithNameAsync(string fileName, string content) {
		var tempPath = Path.Combine(Path.GetTempPath(), fileName);

		try {
			File.WriteAllText(tempPath, content, Encoding.UTF8);
			logger.LogDebug("Created file with name: {FilePath}", tempPath);
			return Task.FromResult(tempPath);
		}
		catch (Exception ex) {
			logger.LogError(ex, "Failed to create file with name: {FilePath}", tempPath);
			throw;
		}
	}

	public async Task<string> ReadCodeFromTempFileAsync(string filePath) {
		var content = string.Empty;
		try {
			if (!File.Exists(filePath))
				throw new FileNotFoundException($"Temporary file not found: {filePath}");

			content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
			logger.LogDebug("Read file content from temporary file: {FilePath}", filePath);
		}
		catch (Exception ex) {
			logger.LogError(ex, "Failed to read content from temporary file: {FilePath}", filePath);
		}

		return content;
	}

	public void DeleteTempFile(string filePath) {
		if (!File.Exists(filePath)) return;
		File.Delete(filePath);
		logger.LogDebug("Deleted temporary file: {FilePath}", filePath);
	}

	private static string GetFileExtension(string language) {
		return LanguageExtensions.TryGetValue(language, out var extension)
			? extension
			: ".txt";
	}
}