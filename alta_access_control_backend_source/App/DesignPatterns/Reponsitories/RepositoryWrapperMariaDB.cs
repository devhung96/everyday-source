using Microsoft.EntityFrameworkCore.Storage;
using Project.App.Databases;
using Project.Modules.Devices.Entities;
using Project.Modules.DeviceTypes.Entities;
using Project.Modules.Modes.Entities;
using Project.Modules.RegisterDetects.Entities;
using Project.Modules.TicketDevices.Entities;
using Project.Modules.Tags.Entities;
using Project.Modules.Tickets.Entities;

namespace Project.App.DesignPatterns.Reponsitories
{
    public interface IRepositoryWrapperMariaDB
    {
        IRepositoryBaseMariaDB<Mode> Modes { get; }
        IRepositoryBaseMariaDB<RegisterDetect> RegisterDetects { get; }
        IRepositoryBaseMariaDB<RegisterDetectDetail> RegisterDetectDetails { get; }
        IRepositoryBaseMariaDB<Device> Devices { get; }
        IRepositoryBaseMariaDB<DeviceType> DeviceTypes { get; }
        IRepositoryBaseMariaDB<Tag> Tags { get; }
        IRepositoryBaseMariaDB<TicketTypeDevice> TicketTypeDevices { get; }
        IRepositoryBaseMariaDB<TicketType> TicketTypes { get; }
        void SaveChanges();
        IDbContextTransaction BeginTransaction();
    }
    public class RepositoryWrapperMariaDB : IRepositoryWrapperMariaDB
    {
        private readonly MariaDBContext DbContext;

        private RepositoryBaseMariaDB<RegisterDetect> registerDetects;
        private RepositoryBaseMariaDB<RegisterDetectDetail> registerDetectDetails;

        private RepositoryBaseMariaDB<Mode> modes;
        private RepositoryBaseMariaDB<Device> devices;
        private RepositoryBaseMariaDB<DeviceType> deviceTypes;
        private RepositoryBaseMariaDB<Tag> tags;
        private RepositoryBaseMariaDB<TicketTypeDevice> ticketDevices;
        private RepositoryBaseMariaDB<TicketType> tickets;
        public RepositoryWrapperMariaDB(MariaDBContext dbContext)
        {
            DbContext = dbContext;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return DbContext.Database.BeginTransaction();
        }
        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }


        public IRepositoryBaseMariaDB<Mode> Modes
        {
            get
            {
                return modes ??= new RepositoryBaseMariaDB<Mode>(DbContext);
            }
        }


        public IRepositoryBaseMariaDB<RegisterDetect> RegisterDetects
        {
            get
            {
                return registerDetects ??
                    (registerDetects = new RepositoryBaseMariaDB<RegisterDetect>(DbContext));
            }
        }

        public IRepositoryBaseMariaDB<RegisterDetectDetail> RegisterDetectDetails
        {
            get
            {
                return registerDetectDetails ??
                    (registerDetectDetails = new RepositoryBaseMariaDB<RegisterDetectDetail>(DbContext));
            }
        }
        public IRepositoryBaseMariaDB<Device> Devices
        {
            get
            {
                return devices ??= new RepositoryBaseMariaDB<Device>(DbContext);
            }
        }

        public IRepositoryBaseMariaDB<DeviceType> DeviceTypes
        {
            get
            {
                return deviceTypes ??= new RepositoryBaseMariaDB<DeviceType>(DbContext);
            }
        }

        public IRepositoryBaseMariaDB<Tag> Tags
        {
            get
            {
                return tags ??= new RepositoryBaseMariaDB<Tag>(DbContext);
            }
        }

        public IRepositoryBaseMariaDB<TicketTypeDevice> TicketTypeDevices
        {
            get
            {
                return ticketDevices ??= new RepositoryBaseMariaDB<TicketTypeDevice>(DbContext);
            }
        }

        public IRepositoryBaseMariaDB<TicketType> TicketTypes
        {
            get
            {
                return tickets ??= new RepositoryBaseMariaDB<TicketType>(DbContext);
            }
        }
    }
}
