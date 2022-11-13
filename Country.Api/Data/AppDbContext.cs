using Country.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Country.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

    }
}
