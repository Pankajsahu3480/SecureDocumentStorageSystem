using SecureDocumentStorageSystem.Models;

namespace SecureDocumentStorageSystem.Services.Interfaces
{
    public interface IDocumentService
    {
		Task UploadAsync(Guid userId, string fileName, IFormFile file);
		Task<Document?> GetLatestAsync(Guid userId, string fileName);
		Task<Document?> GetByRevisionAsync(Guid userId, string fileName, int revision);
		Task<IEnumerable<string>> ListUserFilesAsync(Guid userId);
		Task SoftDeleteDocumentAsync(Guid userId, Guid fileId);

	}
}
