using Project.App.Database;
using Project.App.Providers;
using Project.Modules.WhitelistIps.Entities;
using Project.Modules.WhitelistIps.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Project.Modules.WhitelistIps.Services
{
    public interface IWhiteListIpService
    {
        public IEnumerable<WhitelistIp> ShowAll();
        public WhitelistIp FindId(string id = null, string ipAddress = null);
        public WhitelistIp AddNewWhitelistIp(WhitelistIp whitelistIp);
        public WhitelistIp UpdateWhitelistIp(WhitelistIp whitelistIp);
    }
    public class WhiteListIpService : IWhiteListIpService
    {
        private readonly MariaDBContext dBContext;
        public WhiteListIpService(MariaDBContext DBContext)
        {
            dBContext = DBContext;
        }

        public IEnumerable<WhitelistIp> ShowAll()
        {
            return dBContext.WhitelistIps.Where(x => x.DeletedAt == null);
        }

        public WhitelistIp FindId(string id = null, string ipAddress = null)
        {
            return dBContext.WhitelistIps.FirstOrDefault(
                x => x.DeletedAt == null && 
                (x.WhitelistId.Equals(id) || id == null) &&
                (x.IpAddress.Equals(ipAddress) || ipAddress == null)
            );
        }

        public WhitelistIp AddNewWhitelistIp(WhitelistIp whitelistIp)
        {
            TransportPatternProvider.Instance.Emit("WriteLogAccess", new AddLogRequest
            {
                TypeLog = 1,
                IpAddress = "192.168.1.1",
                UserName = "1231"
            });

            dBContext.WhitelistIps.Add(whitelistIp);
            dBContext.SaveChanges();
            return whitelistIp;
        }

        public WhitelistIp UpdateWhitelistIp(WhitelistIp whitelistIp)
        {
            dBContext.WhitelistIps.Update(whitelistIp);
            dBContext.SaveChanges();
            return whitelistIp;
        }
    }
}
