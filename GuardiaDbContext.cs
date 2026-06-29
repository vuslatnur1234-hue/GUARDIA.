/*using Guardia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Guardia.API.Data
{
    public class GuardiaDbContext : DbContext
    {
        public GuardiaDbContext(DbContextOptions<GuardiaDbContext> options) : base(options) { }

        public DbSet<Admin> Adminler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().ToTable("admin");
        }
    }
}
*/