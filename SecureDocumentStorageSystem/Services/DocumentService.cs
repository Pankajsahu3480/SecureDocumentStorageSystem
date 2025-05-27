using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SecureDocumentStorageSystem.Data;
using SecureDocumentStorageSystem.Models;
using SecureDocumentStorageSystem.Repositories.Interfaces;
using SecureDocumentStorageSystem.Services.Interfaces;

namespace SecureDocumentStorageSystem.Services
{
	public class DocumentService :IDocumentService
	{
		private readonly IDocumentRepository _repo;
		private readonly AppDbContext _context;
		public DocumentService(IDocumentRepository repo, AppDbContext context)
		{
			_repo = repo;
			_context = context;
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


		public async Task SoftDeleteDocumentAsync(Guid id, Guid userId)
		{
			try
			{
				var sql = "EXEC dbo.usp_SofDelete @Id = {0}, @UserId = {1}";
				await _context.Database.ExecuteSqlRawAsync(sql, id, userId);
			}
			catch (Exception ex)
			{
			
				throw new ApplicationException("Failed to soft delete the document.", ex);
			}
		}


	}

}

