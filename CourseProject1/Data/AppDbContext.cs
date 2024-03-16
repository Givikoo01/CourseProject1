using CourseProject1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseProject1.Data
{
    public class AppDbContext:IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions options) : base(options) {
            Collections = Set<Collection>();
            Items = Set<Item>();
        }

        public DbSet<Collection> Collections { get; set; }
        public DbSet<Item> Items { get; set; }
    }
}
