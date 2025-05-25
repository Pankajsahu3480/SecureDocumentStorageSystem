using SecureDocumentStorageSystem.Helpers;
using SecureDocumentStorageSystem.Models;
using SecureDocumentStorageSystem.Repositories.Interfaces;
using SecureDocumentStorageSystem.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace SecureDocumentStorageSystem.Services
{
	public class AuthService : IAuthService
	{
		private readonly IUserRepository _userRepo;
		private readonly IConfiguration _config;

		public AuthService(IUserRepository userRepo, IConfiguration config)
		{
			_userRepo = userRepo;
			_config = config;
		}

		public async Task<string?> LoginAsync(string username, string password)
		{
			var user = await _userRepo.GetByUsernameAsync(username);
			if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
				return null;

			var jwtKey = _config["Jwt:Key"]!;
			return JwtTokenHelper.GenerateToken(user.Id, user.Username, jwtKey);
		}

		public async Task<bool> RegisterAsync(string username, string password)
		{
			var existing = await _userRepo.GetByUsernameAsync(username);
			if (existing != null) return false;

			var user = new ApplicationUser
			{
				Id = Guid.NewGuid(),
				Username = username,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
			};

			await _userRepo.CreateUserAsync(user);
			return true;
		}
	}

}