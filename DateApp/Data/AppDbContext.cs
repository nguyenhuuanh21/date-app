using DateApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Data
{
    public class AppDbContext(DbContextOptions options) :DbContext(options)
    {
        public DbSet<AppUser>AppUsers { get; set; }
        public DbSet<Member>Members { get; set; }
        public DbSet<Photo>Photos { get; set; }
    }
}
