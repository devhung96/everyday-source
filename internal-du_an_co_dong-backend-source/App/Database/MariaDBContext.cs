using Microsoft.EntityFrameworkCore;
using Project.Modules.Authorities.Entities;
using Project.Modules.Medias.Entities;
//using Project.Modules.Surveys.Entities;
using Project.Modules.WhitelistIps.Entities;
using Project.Modules.Users.Entities;
using Project.Modules.Permissions.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.PermissonUsers;
using Project.Modules.PermissionGroups.Entities;
using Project.Modules.Events.Entities;
using Project.Modules.Sessions.Entities;
using Project.Modules.Organizes.Entities;
using Project.Modules.Parameters.Entities;
using Project.Modules.Invitations.Entities;
using Project.Modules.Question.Entities;
using Project.Modules.Functions.Entities;
using Project.Modules.Documents.Entities;
using Project.Modules.UserPermissionEvents.Entities;
using Project.Modules.PermissionOrganizes.Entities;
using Project.Modules.GroupOrganizes.Entities;
using static Project.Modules.Users.Entities.User;

namespace Project.App.Database
{
    public class MariaDBContext : DbContext
    {
        public MariaDBContext(DbContextOptions<MariaDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Survey>().HasQueryFilter(p => p.Enable);
            modelBuilder.Entity<Organize>().HasQueryFilter(x => x.OrganizeStatus != ORGANIZE_STATUS.DELETED);
            modelBuilder.Entity<Event>().HasQueryFilter(x => x.EventStatus != EVENT_STATUS.DELETED);
            modelBuilder.Entity<Session>().HasQueryFilter(x => x.SessionStatus != SESSION_STATUS.DELETED);
           // modelBuilder.Entity<User>().HasQueryFilter(x => x.UserDelete != DELETE_STATUS.DELETED);
            //  modelBuilder.Entity<Survey>().HasQueryFilter(p => p.Enable);
            modelBuilder.Entity<Questions>().HasQueryFilter(p => p.Status == StatusQuestion.SHOW);
        }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Authority> Authorities { get; set; }
        //public DbSet<Survey> Surveys { get; set; }
        //public DbSet<SurveyUsers> SurveyUsers { get; set; }
        public DbSet<WhitelistIp> WhitelistIps { get; set; }
        public DbSet<UserSuper> UserSupers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Modules.Permissions.Entities.PermissionOrganize> Permissions { get; set; }
        public DbSet<Modules.PermissionOrganizes.Entities.PermissionOrganize> PermissionOrganizes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupOrganize> GroupOrganizes { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<FunctionOrganize> FunctionOrganizes { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleOrganize> ModuleOrganizes { get; set; }
        public DbSet<PermissionUser> PermissionUsers { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }
        public DbSet<PermissionGroupOrganize> PermissionGroupOrganizes { get; set; }
     //   public DbSet<PermissionOrganize> PermissionOrganizes { get; set; }


        public DbSet<UserPermissionEvent> UserPermissionEvents { get; set; }
        public DbSet<UserPermissionOrganize> UserPermissionOrganizes { get; set; }
        public DbSet<QuestionClient> QuestionClients { get; set; }
        public DbSet<QuestionCommentClient> QuestionCommentClients { get; set; }
        public DbSet<Organize> Organizes { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }
        public DbSet<MiddleQuestion> MiddleQuestions { get; set; }
        public DbSet<DocumentFile> DocumentFiles { get; set; }
    }
}
