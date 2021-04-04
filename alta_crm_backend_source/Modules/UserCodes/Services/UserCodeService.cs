using Project.Modules.UserCodes.Enities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.UserCodes.Services
{
    public interface IUserCodeService
    {
        public void RemoveCodeExpire();
    }
    public class UserCodeService : IUserCodeService
    {
        private readonly IRepositoryMongoWrapper _repositoryMongo;
        private readonly IRepositoryWrapperMariaDB _repositoryWrapperMariaDB;

        public UserCodeService(IRepositoryMongoWrapper repositoryMongo, IRepositoryWrapperMariaDB repositoryWrapperMariaDB)
        {
            _repositoryMongo = repositoryMongo;
            _repositoryWrapperMariaDB = repositoryWrapperMariaDB;
        }


        public void RemoveCodeExpire()
        {
            var datTimeNow = DateTime.UtcNow;
            List<UserCode> userCodes = _repositoryWrapperMariaDB.UserCodeds.FindByCondition(x => x.UserCodeExpire < datTimeNow).ToList();
            
            _repositoryWrapperMariaDB.UserCodeds.RemoveRange(userCodes);
            _repositoryWrapperMariaDB.SaveChanges();

            //log 
            if (userCodes.Count > 0)
            {
                _repositoryMongo.LogUserCodeJobs.AddRange(userCodes.Select(x => new LogUserCodeJob
                {
                    UserCodeActive = x.UserCodeActive,
                    UserCodeExpire = x.UserCodeExpire,
                    UserCodeId = x.UserCodeId,
                    UserId = x.UserId,
                    UserCodeCreatedAt = x.UserCodeCreatedAt
                }).ToList());
            }

          

        }
    }
}
