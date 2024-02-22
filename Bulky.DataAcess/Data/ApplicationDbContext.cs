using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAcess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Category Sample", DisplayOrder = 1 }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Title sample",
                    Description = "Description sample",
                    ISBN = "123456789",
                    Author = "Author sample",
                    ListPrice = 20,
                    Price = 20,
                    Price50 = 18,
                    Price100 = 15,
                    CategoryId = 1,
                    ImageUrl = ""
                }
                );
        }
    }
}
