namespace Codeforge.Domain.Interfaces;

public interface ISupabaseService {
	Task<string> ReadFileAsync(string bucketName, string remotePath);
	Task<string> GetSignedUrlAsync(string bucketName, string filePath);

	Task UploadOrUpdateFileAsync(string bucketName, string localFilePath, bool upsert = true);

	Task DeleteFileAsync(string bucketName, string remotePath);
}