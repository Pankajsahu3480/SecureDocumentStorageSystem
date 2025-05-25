using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SecureDocumentStorageSystem.Data;
using SecureDocumentStorageSystem.Models;
using SecureDocumentStorageSystem.Repositories.Interfaces;

namespace SecureDocumentStorageSystem.Repositories
{
    public class UserRepository : IUserRepository
    {
		private readonly AppDbContext _context;

		public UserRepository(AppDbContext context)
		{
			_context = context;
		}

		public async Task<ApplicationUser?> GetByUsernameAsync(string username)
		{
			return await _context.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(u => u.Username == username);
		}

		public async Task CreateUserAsync(ApplicationUser user)
		{
			var parameters = new[]
			{
			new SqlParameter("@Id", user.Id),
			new SqlParameter("@Username", user.Username),
			new SqlParameter("@PasswordHash", user.PasswordHash)
		};

			await _context.Database.ExecuteSqlRawAsync("EXEC AddUser @Id, @Username, @PasswordHash", parameters);
		}
		
	}
}
