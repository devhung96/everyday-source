using Microsoft.EntityFrameworkCore.Storage;
using Project.App.Databases;
using Project.Modules.Groups.Enities;
using Project.Modules.Medias.Entities;
using Project.Modules.Schedules.Entities;
using Project.Modules.Ratings.Enities;
using Project.Modules.Tags.Enities;
using Project.Modules.Users.Entities;
using Project.Modules.UserTagModes.Entities;
using System.Threading.Tasks;
using Project.Modules.UsersModes.Entities;
using Project.Modules.UserCodes.Enities;

namespace Repository
{
    public interface IRepositoryWrapperMariaDB
    {
        IRepositoryBaseMariaDB<Group> Groups { get; }
        IRepositoryBaseMariaDB<Tag> Tags { get; }
        IRepositoryBaseMariaDB<ModeAuthentication> ModeAuthentications { get; }
        IRepositoryBaseMariaDB<User> Users { get; }
        IRepositoryBaseMariaDB<Media> Medias { get; }
        IRepositoryBaseMariaDB<TicketType> Tickets { get; }
        IRepositoryBaseMariaDB<Robot> Robots { get; }
        IRepositoryBaseMariaDB<Rating> Ratings { get; }
        IDbContextTransaction BeginTransaction();
        IRepositoryBaseMariaDB<Schedule> Schedules { get; }
        IRepositoryBaseMariaDB<UserTagMode> UserTagModes { get; }
        IRepositoryBaseMariaDB<UserMode> UserModes { get; }
        IRepositoryBaseMariaDB<UserCode> UserCodeds { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }
    public class RepositoryWrapperMariaDB : IRepositoryWrapperMariaDB
    {
        private readonly MariaDBContext DbContext;
        private RepositoryBaseMariaDB<Group> groups;
        private RepositoryBaseMariaDB<Tag> tasks;
        private RepositoryBaseMariaDB<ModeAuthentication> modeAuthentications;
        private RepositoryBaseMariaDB<User> users;
        private RepositoryBaseMariaDB<Media> medias;
        private RepositoryBaseMariaDB<Schedule> schedules;
        private RepositoryBaseMariaDB<TicketType> tickets;
        private RepositoryBaseMariaDB<Robot> robots;
        private RepositoryBaseMariaDB<Rating> ratings;
        private RepositoryBaseMariaDB<UserTagMode> userTagModes;
        private RepositoryBaseMariaDB<UserMode> userModes;
        private RepositoryBaseMariaDB<UserCode> userCodes;
        public RepositoryWrapperMariaDB(MariaDBContext dbContext)
        {
            DbContext = dbContext;
        }
        public IDbContextTransaction BeginTransaction()
        {
            return DbContext.Database.BeginTransaction();
        }

        public IRepositoryBaseMariaDB<UserCode> UserCodeds
        {
            get
            {
                return userCodes ??
                    (userCodes = new RepositoryBaseMariaDB<UserCode>(DbContext));
            }
        }

        public IRepositoryBaseMariaDB<Media> Medias
        {
            get
            {
                return medias ??
                    (medias = new RepositoryBaseMariaDB<Media>(DbContext));
            }
        }
       

        public IRepositoryBaseMariaDB<UserTagMode> UserTagModes
        {
            get
            {
                return userTagModes ??
                    (userTagModes = new RepositoryBaseMariaDB<UserTagMode>(DbContext));
            }
        }
        public IRepositoryBaseMariaDB<Rating> Ratings
        {
            get
            {
                return ratings ??
                    (ratings = new RepositoryBaseMariaDB<Rating>(DbContext));
            }
        }
        public IRepositoryBaseMariaDB<Robot> Robots
        {
            get
            {
                return robots ??
                    (robots = new RepositoryBaseMariaDB<Robot>(DbContext));
            }
        }
        public IRepositoryBaseMariaDB<TicketType> Tickets
        {
            get
            {
                return tickets ??
                    (tickets = new RepositoryBaseMariaDB<TicketType>(DbContext));
            }
        }
        public IRepositoryBaseMariaDB<User> Users
        {
            get
            {
                return users ??
                    (users = new RepositoryBaseMariaDB<User>(DbContext));
            }
        }
        public IRepositoryBaseMariaDB<Group> Groups
        {
            get
            {
                return groups ??
                    (groups = new RepositoryBaseMariaDB<Group>(DbContext));
            }
        }
        public IRepositoryBaseMariaDB<ModeAuthentication> ModeAuthentications
        {
            get
            {
                return modeAuthentications ??
                    (modeAuthentications = new RepositoryBaseMariaDB<ModeAuthentication>(DbContext));
            }
        }
        
        public IRepositoryBaseMariaDB<Tag> Tags
        {
            get
            {
                return tasks ??
                    (tasks = new RepositoryBaseMariaDB<Tag>(DbContext));
            }
        }

        public IRepositoryBaseMariaDB<Schedule> Schedules
        {
            get
            {
                return schedules ?? (schedules = new RepositoryBaseMariaDB<Schedule>(DbContext));
            }
        }

        public IRepositoryBaseMariaDB<UserMode> UserModes
        {
            get
            {
                return userModes ?? (userModes = new RepositoryBaseMariaDB<UserMode>(DbContext));
            }
        }

        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }
        public async Task SaveChangesAsync()
        {
            await DbContext.SaveChangesAsync();
        }
    }
}
