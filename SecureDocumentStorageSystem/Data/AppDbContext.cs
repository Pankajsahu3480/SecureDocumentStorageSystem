using Microsoft.EntityFrameworkCore;
using SecureDocumentStorageSystem.Models;

namespace SecureDocumentStorageSystem.Data
{
    public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
		public DbSet<Document> Documents => Set<Document>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ApplicationUser>().ToTable("Users");
			modelBuilder.Entity<Document>().ToTable("Documents");
		}
	}
}
