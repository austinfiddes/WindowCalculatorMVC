using Microsoft.EntityFrameworkCore;
using MyFirstMVCApp.Models;

namespace MyFirstMVCApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define DbSets for your models, e.g.:
        public DbSet<Window> Windows { get; set; }
    }
}