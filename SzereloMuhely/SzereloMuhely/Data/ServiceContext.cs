using Microsoft.EntityFrameworkCore;

namespace SzereloMuhely.Data
{
    public class ServiceContext:DbContext
    {
        public DbSet<Models.User> Users{ get; set; } = null!;
        public DbSet<Models.Mechanic> Mechanics { get; set; } = null!;
        public DbSet<Models.WorkRecorder> WorkRecorders { get; set; } = null!;
        public DbSet<Models.WorkSheet> WorkSheets { get; set; } = null!;
        public DbSet<Models.WorkProcess> WorkProcesses { get; set; } = null!;
        public DbSet<Models.Material> Materials { get; set; } = null!;
        public DbSet<Models.Part> Parts { get; set; } = null!;
        public DbSet<Models.Vehicle> Vehicles { get; set; } = null!;

        public ServiceContext(DbContextOptions<ServiceContext> options) : base(options)
        {
        }
    }
}
