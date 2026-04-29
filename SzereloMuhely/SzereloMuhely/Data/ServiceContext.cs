using Microsoft.EntityFrameworkCore;
using SzereloMuhely.Models;

namespace SzereloMuhely.Data
{
    public class ServiceContext : DbContext
    {
        public DbSet<Models.User> Users { get; set; } = null!;
        public DbSet<Models.WorkItem> WorkItems { get; set; } = null!;
        public DbSet<Models.WorkSheet> WorkSheets { get; set; } = null!;
        public DbSet<Models.WorkProcess> WorkProcesses { get; set; } = null!;
        public DbSet<Models.Material> Materials { get; set; } = null!;
        public DbSet<Models.Part> Parts { get; set; } = null!;
        public DbSet<Models.Vehicle> Vehicles { get; set; } = null!;

        public ServiceContext(DbContextOptions<ServiceContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkItem>()
                .Property(w => w.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<WorkProcess>()
                .HasMany(wp => wp.Materials)
                .WithOne(m => m.WorkProcess)
                .HasForeignKey(m => m.WorkProcessID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkProcess>()
                .HasMany(wp => wp.Parts)
                .WithOne(p => p.WorkProcess)
                .HasForeignKey(p => p.WorkProcessID)
                .OnDelete(DeleteBehavior.Restrict);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
