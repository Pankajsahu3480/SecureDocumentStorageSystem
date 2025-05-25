using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SecureDocumentStorageSystem.Data;
using SecureDocumentStorageSystem.Models;
using SecureDocumentStorageSystem.Repositories.Interfaces;

namespace SecureDocumentStorageSystem.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
		private readonly string _connection;
		private readonly AppDbContext _context;


		public DocumentRepository(IConfiguration config, AppDbContext context)
		{
			_connection = config.GetConnectionString("DefaultConnection")!;
			_context = context;
		}

		public async Task UploadDocumentAsync(Guid userId, string fileName, byte[] content)
		{
			// Get latest revision
			var latestVersion = await _context.Documents
				.Where(d => d.UserId == userId && d.FileName == fileName)
				.OrderByDescending(d => d.Version)
				.Select(d => d.Version)
				.FirstOrDefaultAsync();

			var nextVersion = latestVersion + 1;

			var parameters = new[]
			{
			new SqlParameter("@Id", Guid.NewGuid()),
			new SqlParameter("@UserId", userId),
			new SqlParameter("@FileName", fileName),
			new SqlParameter("@Content", content),
			new SqlParameter("@UploadDate", DateTime.UtcNow),
			new SqlParameter("@Version", nextVersion)
		};

			await _context.Database.ExecuteSqlRawAsync("EXEC AddDocument @Id, @UserId, @FileName, @Content, @UploadDate, @Version", parameters);

		}

		public async Task<Document?> GetLatestDocumentAsync(Guid userId, string fileName)
		{
			using var conn = new SqlConnection(_connection);
			await conn.OpenAsync();

			var cmd = new SqlCommand("GetLatestDocument", conn)
			{
				CommandType = System.Data.CommandType.StoredProcedure
			};
			cmd.Parameters.AddWithValue("@UserId", userId);
			cmd.Parameters.AddWithValue("@FileName", fileName);

			using var reader = await cmd.ExecuteReaderAsync();
			return await ReadDocumentAsync(reader);
		}

		public async Task<Document?> GetDocumentByRevisionAsync(Guid userId, string fileName, int revision)
		{
			using var conn = new SqlConnection(_connection);
			await conn.OpenAsync();

			var cmd = new SqlCommand("GetDocumentByRevision", conn)
			{
				CommandType = System.Data.CommandType.StoredProcedure
			};
			cmd.Parameters.AddWithValue("@UserId", userId);
			cmd.Parameters.AddWithValue("@FileName", fileName);
			cmd.Parameters.AddWithValue("@Revision", revision);

			using var reader = await cmd.ExecuteReaderAsync();
			return await ReadDocumentAsync(reader);
		}

		public async Task<IEnumerable<string>> ListUserFilesAsync(Guid userId)
		{
			var files = new List<string>();

			using var conn = new SqlConnection(_connection);
			await conn.OpenAsync();

			var cmd = new SqlCommand("ListUserDocuments", conn)
			{
				CommandType = System.Data.CommandType.StoredProcedure
			};
			cmd.Parameters.AddWithValue("@UserId", userId);

			using var reader = await cmd.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				files.Add(reader.GetString(0));
			}

			return files;
		}
		public async Task SaveDocumentAsync(Guid userId, string fileName, byte[] content)
		{
			// Get latest revision (or -1 if none)
			var latestRevision = await _context.Documents
				.Where(d => d.UserId == userId && d.FileName == fileName)
				.OrderByDescending(d => d.Version)
				.Select(d => d.Version)
				.FirstOrDefaultAsync();

			var nextRevision = latestRevision + 1;

			var parameters = new[]
			{
		new SqlParameter("@Id", Guid.NewGuid()),
		new SqlParameter("@UserId", userId),
		new SqlParameter("@FileName", fileName),
		new SqlParameter("@Content", content),
		new SqlParameter("@UploadDate", DateTime.UtcNow),
		new SqlParameter("@Revision", nextRevision)
			};

			await _context.Database.ExecuteSqlRawAsync(
				"EXEC AddDocument @Id, @UserId, @FileName, @Content, @UploadDate, @Revision",
				parameters);
		}

		private async Task<Document?> ReadDocumentAsync(SqlDataReader reader)
		{
			if (await reader.ReadAsync())
			{
				return new Document
				{
					Id = reader.GetGuid(0),
					UserId = reader.GetGuid(1),
					FileName = reader.GetString(2),
					Content = (byte[])reader["Content"],
					Version = reader.GetInt32(4),
					UploadedAt = reader.GetDateTime(5)
				};
			}

			return null;
		}
		
	}
}
