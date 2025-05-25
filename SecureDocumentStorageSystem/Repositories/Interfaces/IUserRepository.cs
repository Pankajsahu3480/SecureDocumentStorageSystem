using SecureDocumentStorageSystem.Models;

namespace SecureDocumentStorageSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
		Task<ApplicationUser?> GetByUsernameAsync(string username);
		Task CreateUserAsync(ApplicationUser user);
	}
}
