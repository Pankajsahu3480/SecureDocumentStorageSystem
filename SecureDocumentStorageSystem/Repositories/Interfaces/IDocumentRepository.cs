using SecureDocumentStorageSystem.Models;

namespace SecureDocumentStorageSystem.Repositories.Interfaces
{
    public interface IDocumentRepository
    {
		Task UploadDocumentAsync(Guid userId, string fileName, byte[] content);
		Task<Document?> GetLatestDocumentAsync(Guid userId, string fileName);
		Task<Document?> GetDocumentByRevisionAsync(Guid userId, string fileName, int revision);
		Task<IEnumerable<string>> ListUserFilesAsync(Guid userId);
		Task SaveDocumentAsync(Guid userId, string fileName, byte[] content);
	}
}
