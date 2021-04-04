using Microsoft.EntityFrameworkCore;
using Project.Modules.RegisterDetects.Entities;
using Project.Modules.Logs.Entities;
using Project.Modules.Devices.Entities;
using Project.Modules.DeviceTypes.Entities;
using Project.Modules.Modes.Entities;
using Project.Modules.TicketDevices.Entities;
using Project.Modules.Tags.Entities;
using Project.Modules.Tickets.Entities;

namespace Project.App.Databases
{
    public class MariaDBContext : DbContext
    {
        public MariaDBContext(DbContextOptions<MariaDBContext> options) : base(options)
        {
        }

        public DbSet<RegisterDetect> RegisterDetects { get; set; }
        public DbSet<RegisterDetectDetail> RegisterDetectDetails { get; set; }
        public DbSet<Mode> Modes { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TicketTypeDevice> TicketDevices { get; set; }
        public DbSet<TicketType> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
