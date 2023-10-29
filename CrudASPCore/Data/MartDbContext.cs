using CrudASPCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudASPCore.Data
{
    public class MartDbContext : DbContext
    {
        public MartDbContext(DbContextOptions<MartDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(b => b.Category)
                .WithMany(a => a.products)
                .HasForeignKey(b => b.Category_Id);
        }
    }
}
