using SecureDocumentStorageSystem.Models;
using SecureDocumentStorageSystem.Repositories.Interfaces;
using SecureDocumentStorageSystem.Services.Interfaces;

namespace SecureDocumentStorageSystem.Services
{
	public class DocumentService :IDocumentService
	{
		private readonly IDocumentRepository _repo;

		public DocumentService(IDocumentRepository repo)
		{
			_repo = repo;
		}

		public async Task UploadAsync(Guid userId, string fileName, IFormFile file)
		{
			using var ms = new MemoryStream();
			await file.CopyToAsync(ms);

			var doc = new Document
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				FileName = fileName,
				Content = ms.ToArray(),
				UploadedAt = DateTime.UtcNow
			};

			await _repo.UploadDocumentAsync(userId,fileName,ms.ToArray());
		}

		public Task<Document?> GetLatestAsync(Guid userId, string fileName)
			=> _repo.GetLatestDocumentAsync(userId, fileName);

		public Task<Document?> GetByRevisionAsync(Guid userId, string fileName, int revision)
			=> _repo.GetDocumentByRevisionAsync(userId, fileName, revision);

		public Task<IEnumerable<string>> ListUserFilesAsync(Guid userId)
			=> _repo.ListUserFilesAsync(userId);
	}

}

