using Project.Modules.UsersModes.Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.UsersModes.Services
{
    public class HelperService
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;

        public HelperService(IRepositoryWrapperMariaDB repositoryWrapperMariaDB)
        {
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;
        }

        public string GetKeyCode(string userId, string modeId)
        {
            UserMode userMode = RepositoryWrapperMariaDB.UserModes.FirstOrDefault(x => x.ModeId.Equals(modeId) && x.UserId.Equals(userId));

            if (userMode is null)
            {
                return "";
            }

            return userMode.UserModeKeyCode;
        }
    }
}
