using Microsoft.EntityFrameworkCore;
using Project.Modules.App.Entities;
using Project.Modules.Devices.Entities;
using Project.Modules.DeviceTypes.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Medias.Entities;
using Project.Modules.PlayLists.Entities;
using Project.Modules.Schedules.Entities;
using Project.Modules.Settings.Entitites;
using Project.Modules.TemplateDetails.Entities;
using Project.Modules.Templates.Entities;
using Project.Modules.Users.Entities;

namespace Project.App.Databases
{
    public class MariaDBContext : DbContext
    {
        public MariaDBContext(DbContextOptions<MariaDBContext> options) : base(options)
        {

        }


        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<BlacklistTokens> BlacklistTokens { get; set; }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateDetail> TemplateDetails { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<MediaType> MediaTypes { get; set; }
        public DbSet<MediaGroup> MediaGroups { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceTokens> DeviceTokens { get; set; }

        public DbSet<PlayList> PlayLists { get; set; }
        public DbSet<PlayListDetail> PlayListDetails { get; set; }
        public DbSet<ScheduleDevice> ScheduleDevices { get; set; }
        public DbSet<DataScheduleTmp> DataScheduleTmps { get; set; }

        public DbSet<TemplateShare> TemplateShares { get; set; }

        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
