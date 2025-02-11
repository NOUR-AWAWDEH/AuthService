using AuthService.Entity;
using AuthService.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) 
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique(); // Prevent duplicate usernames

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion(
                    v => v.ToString(),
                    v => (UserRole)Enum.Parse(typeof(UserRole), v));
        }

        public DbSet<User> Users { get; set; }

    }
    
}
