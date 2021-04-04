using Microsoft.EntityFrameworkCore.Storage;
using Project.App.Databases;
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

namespace Project.App.DesignPatterns.Reponsitories
{
    public interface IRepositoryWrapperMariaDB
    {

        IRepositoryBaseMariaDB<Setting> Settings { get; }
        IRepositoryBaseMariaDB<Schedule> Schedules { get; }
        IRepositoryBaseMariaDB<DeviceType> DeviceTypes { get; }
        IRepositoryBaseMariaDB<PlayList> PlayLists { get; }
        IRepositoryBaseMariaDB<PlayListDetail> PlayListDetails { get; }
        IRepositoryBaseMariaDB<User> Users { get; }
        IRepositoryBaseMariaDB<Group> Groups { get; }
        IRepositoryBaseMariaDB<Permission> Permissions { get; }
        IRepositoryBaseMariaDB<UserPermission> UserPermissions { get; }
        IRepositoryBaseMariaDB<Role> Roles { get; }
        IRepositoryBaseMariaDB<RolePermission> RolePermissions { get; }
        IRepositoryBaseMariaDB<BlacklistTokens> BlacklistTokens { get; }
        IRepositoryBaseMariaDB<Device> Devices { get; }
        IRepositoryBaseMariaDB<DeviceTokens> DeviceTokens { get; }
        IRepositoryBaseMariaDB<Media> Medias { get; }
        IRepositoryBaseMariaDB<MediaType> MediaTypes { get; }
        IRepositoryBaseMariaDB<MediaGroup> MediaGroups { get; }

        void SaveChanges();
        IDbContextTransaction BeginTransaction();
        IRepositoryBaseMariaDB<Template> Templates { get; }
        IRepositoryBaseMariaDB<TemplateDetail> TemplateDetails { get; }
        IRepositoryBaseMariaDB<ScheduleDevice> ScheduleDevices { get; }
        IRepositoryBaseMariaDB<DataScheduleTmp> DataScheduleTmps { get; }
        IRepositoryBaseMariaDB<TemplateShare> TemplateShares { get; }
    }
    public class RepositoryWrapperMariaDB : IRepositoryWrapperMariaDB
    {
        private readonly MariaDBContext DbContext;
        private RepositoryBaseMariaDB<Setting> settings;
        private RepositoryBaseMariaDB<Schedule> schedules;

        private RepositoryBaseMariaDB<DeviceType> deviceTypes;
        private RepositoryBaseMariaDB<Template> templates;
        private RepositoryBaseMariaDB<TemplateDetail> templateDetails;
        RepositoryBaseMariaDB<User> users;
        RepositoryBaseMariaDB<Group> groups;
        RepositoryBaseMariaDB<Role> roles;
        private RepositoryBaseMariaDB<RolePermission> rolePermissions;
        RepositoryBaseMariaDB<Permission> permissions;
        RepositoryBaseMariaDB<UserPermission> userPermissions;
        RepositoryBaseMariaDB<BlacklistTokens> blacklistTokens;
        RepositoryBaseMariaDB<Device> devices;
        RepositoryBaseMariaDB<DeviceTokens> deviceTokens;
        RepositoryBaseMariaDB<PlayList> playLists;
        RepositoryBaseMariaDB<PlayListDetail> playListDetails;
        RepositoryBaseMariaDB<Media> medias;
        RepositoryBaseMariaDB<MediaType> mediaTypes;
        RepositoryBaseMariaDB<MediaGroup> mediaGroups;
        RepositoryBaseMariaDB<DataScheduleTmp> dataScheduleTmps;
        RepositoryBaseMariaDB<TemplateShare> templateShares;

        private RepositoryBaseMariaDB<ScheduleDevice> scheduleDevices;

        public RepositoryWrapperMariaDB(MariaDBContext dbContext)
        {
            DbContext = dbContext;
        }

        public IRepositoryBaseMariaDB<DeviceType> DeviceTypes
        {
            get
            {
                return deviceTypes ??= new RepositoryBaseMariaDB<DeviceType>(DbContext);
            }
        }

        public IRepositoryBaseMariaDB<DataScheduleTmp> DataScheduleTmps
        {
            get
            {
                return dataScheduleTmps ??= new RepositoryBaseMariaDB<DataScheduleTmp>(DbContext);
            }
        }

        public IRepositoryBaseMariaDB<Setting> Settings
        {
            get
            {
                return settings ??= new RepositoryBaseMariaDB<Setting>(DbContext);
            }
        }

        public IRepositoryBaseMariaDB<Schedule> Schedules
        {
            get
            {
                return schedules ??= new RepositoryBaseMariaDB<Schedule>(DbContext);
            }
        }

        public IRepositoryBaseMariaDB<User> Users => users ?? new RepositoryBaseMariaDB<User>(DbContext);

        public IRepositoryBaseMariaDB<Group> Groups => groups ?? new RepositoryBaseMariaDB<Group>(DbContext);

        public IRepositoryBaseMariaDB<Permission> Permissions => permissions ?? new RepositoryBaseMariaDB<Permission>(DbContext);

        public IRepositoryBaseMariaDB<UserPermission> UserPermissions => userPermissions ?? new RepositoryBaseMariaDB<UserPermission>(DbContext);

        public IRepositoryBaseMariaDB<Role> Roles => roles ?? new RepositoryBaseMariaDB<Role>(DbContext);

        public IRepositoryBaseMariaDB<BlacklistTokens> BlacklistTokens => blacklistTokens ?? new RepositoryBaseMariaDB<BlacklistTokens>(DbContext);


        public IRepositoryBaseMariaDB<Device> Devices => devices ?? new RepositoryBaseMariaDB<Device>(DbContext);

        public IRepositoryBaseMariaDB<DeviceTokens> DeviceTokens => deviceTokens ?? new RepositoryBaseMariaDB<DeviceTokens>(DbContext);
        public IRepositoryBaseMariaDB<Media> Medias => medias ?? new RepositoryBaseMariaDB<Media>(DbContext);
        public IRepositoryBaseMariaDB<MediaType> MediaTypes => mediaTypes ?? new RepositoryBaseMariaDB<MediaType>(DbContext);
        public IRepositoryBaseMariaDB<MediaGroup> MediaGroups => mediaGroups ?? new RepositoryBaseMariaDB<MediaGroup>(DbContext);
        public IRepositoryBaseMariaDB<PlayList> PlayLists => playLists ?? new RepositoryBaseMariaDB<PlayList>(DbContext);
        public IRepositoryBaseMariaDB<PlayListDetail> PlayListDetails => playListDetails ?? new RepositoryBaseMariaDB<PlayListDetail>(DbContext);
        public IRepositoryBaseMariaDB<RolePermission> RolePermissions => rolePermissions ?? new RepositoryBaseMariaDB<RolePermission>(DbContext);


        public IDbContextTransaction BeginTransaction()
        {
            return DbContext.Database.BeginTransaction();
        }
        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }

        public IRepositoryBaseMariaDB<Template> Templates
        {
            get
            {
                return templates ?? (templates = new RepositoryBaseMariaDB<Template>(DbContext));
            }
        }
        public IRepositoryBaseMariaDB<TemplateDetail> TemplateDetails
        {
            get
            {
                return templateDetails ?? (templateDetails = new RepositoryBaseMariaDB<TemplateDetail>(DbContext));
            }
        }

        public IRepositoryBaseMariaDB<ScheduleDevice> ScheduleDevices
        {
            get
            {
                return scheduleDevices ?? (scheduleDevices = new RepositoryBaseMariaDB<ScheduleDevice>(DbContext));
            }
        }

        public IRepositoryBaseMariaDB<TemplateShare> TemplateShares
        {
            get
            {
                return templateShares ?? (templateShares = new RepositoryBaseMariaDB<TemplateShare>(DbContext));
            }
        }

    }
}
