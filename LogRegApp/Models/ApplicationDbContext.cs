using Microsoft.EntityFrameworkCore;

namespace LogRegApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<AppUser>? AppUsers { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
