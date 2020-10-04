using WebAPIAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace WebAPIAuth.Contexts {
    public class DatabaseContext : DbContext {
        public DbSet<User> User { get; set; }
        public DbSet<UserSession> UserSession { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserSession>().ToTable("UserSession");
        }
    }
}