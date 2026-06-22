using DateApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace DateApp.Data
{
    public class AppDbContext(DbContextOptions options) :DbContext(options)
    {
        public DbSet<AppUser>AppUsers { get; set; }
    }
}
