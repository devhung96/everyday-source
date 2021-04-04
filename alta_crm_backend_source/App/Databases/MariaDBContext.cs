using Microsoft.EntityFrameworkCore;
using Project.Modules.Groups.Enities;
using Project.Modules.Medias.Entities;
using Project.Modules.Schedules.Entities;
using Project.Modules.Ratings.Enities;
using Project.Modules.Tags.Enities;
using Project.Modules.Users.Entities;
using Project.Modules.UserTagModes.Entities;
using Project.Modules.UsersModes.Entities;
using Project.Modules.UserCodes.Enities;

namespace Project.App.Databases
{
    public class MariaDBContext : DbContext
    {
        public MariaDBContext(DbContextOptions<MariaDBContext> options) : base(options)
        {

        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Tag> TaskUsers { get; set; }
        public DbSet<ModeAuthentication> ModeAuthentications { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TicketType> Tickets { get; set; }
        public DbSet<Robot> Robots { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<UserTagMode> UserTagModes { get; set; }
        public DbSet<UserMode> UserModes { get; set; }
        public DbSet<UserCode> UserCodes { get; set; }
    }
}
