using BulkyWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { id = 1, name = "Action", display_order = 1 },
                new Category { id = 2, name = "SciFi", display_order = 2 },
                new Category { id = 3, name = "History", display_order = 3 }
                );
        }
    }
}
