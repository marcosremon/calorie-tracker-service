using CalorieTrackerService.Domain.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace RoutinesGymService.Infraestructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<MercadonaProduct> MercadonaProducts { get; set; }
        public DbSet<Consumption> Consumption { get; set; }
        public DbSet<AiLogs> AiLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}