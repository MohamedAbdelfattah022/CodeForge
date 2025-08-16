namespace Codeforge.Domain.Interfaces;

public interface ITempCodeFileService {
	Task<string> SaveCodeToTempFileAsync(string code, string language);
	Task<string> ReadCodeFromTempFileAsync(string filePath);
	void DeleteTempFile(string filePath);
}