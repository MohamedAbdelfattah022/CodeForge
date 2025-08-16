using System.Text;
using Codeforge.Domain.Interfaces;
using Supabase;
using FileOptions = Supabase.Storage.FileOptions;

namespace Codeforge.Infrastructure.Services;

public class SupabaseService(Client supabase) : ISupabaseService {
	public async Task<string> ReadFileAsync(string bucketName, string remotePath) {
		var res = await supabase.Storage.From(bucketName).Download(remotePath, null);
		using var stream = new MemoryStream(res);
		using var reader = new StreamReader(stream, Encoding.UTF8, true);
		var content = await reader.ReadToEndAsync();

		return content;
	}

	public async Task<string> GetSignedUrlAsync(string bucketName, string filePath) {
		var fileName = filePath.Split("\\").Last();
		return await supabase.Storage.From(bucketName).CreateSignedUrl(fileName, 60);
	}

	public async Task UploadOrUpdateFileAsync(string bucketName, string localFilePath, bool upsert = true) {
		var fileName = localFilePath.Split("\\").Last();
		await supabase.Storage.From(bucketName).Upload(localFilePath,
			fileName,
			new FileOptions
				{
					Upsert = upsert
				});
	}

	public async Task DeleteFileAsync(string bucketName, string remotePath) {
		var fileName = remotePath.Split("\\").Last();
		await supabase.Storage.From(bucketName).Remove([fileName]);
	}
}