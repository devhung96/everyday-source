using Project.App.Helpers;
using Project.App.Paginations;
using Project.Modules.Medias.Services;
using Project.Modules.Users.Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Service
{
    public interface ILogImportSevice
    {
        (PaginationResponse<LogFile> data, string message) ShowAllFileLog(PaginationRequest request);
        (PaginationResponse<LogUserImport> data, string message) DataInFile(PaginationRequest request, string logFileId);
        (User data, string message) LoadFile(string path);
    }
    public class LogSevice : ILogImportSevice
    {
        public readonly IRepositoryMongoWrapper mongoDB;
        public readonly IRepositoryWrapperMariaDB mariaDB;
        public readonly IMediaService mediaSevice;
        public LogSevice(IRepositoryMongoWrapper mongoDB, IRepositoryWrapperMariaDB mariaDB, IMediaService mediaSevice)
        {
            this.mongoDB = mongoDB;
            this.mariaDB = mariaDB;
            this.mediaSevice = mediaSevice;
        }

        public (PaginationResponse<LogUserImport> data, string message) DataInFile(PaginationRequest request, string logFileId)
        {
            LogFile logFile = mongoDB.LogFiles.FirstOrDefault(x => x.LogFileId.Equals(logFileId));
            if (logFile is null)
            {
                return (null, "FileNoFound");
            }
            var userImports = mongoDB.LogUserImport.FindByCondition(x => x.LogFileId.Equals(logFileId));
            PaginationHelper<LogUserImport> dataUser = PaginationHelper<LogUserImport>.ToPagedList(userImports, request.PageNumber, request.PageSize);

            PaginationResponse<LogUserImport> paginationResponse = new PaginationResponse<LogUserImport>(dataUser, dataUser.PageInfo);
            return (paginationResponse, "ShowDataSuccess");
        }

        public (User data, string message) LoadFile(string path)
        {
            User user = mariaDB.Users.FindByCondition(x => x.UserImage.Equals(path)).FirstOrDefault();
            if (user is null)
            {
                return (null, "UserNotExist");
            }
            return (user, "Success");
        }

        public (PaginationResponse<LogFile> data, string message) ShowAllFileLog(PaginationRequest request)
        {
            var log = mongoDB.LogFiles.FindAll();
            PaginationHelper<LogFile> dataUser = PaginationHelper<LogFile>.ToPagedList(log, request.PageNumber, request.PageSize);

            PaginationResponse<LogFile> paginationResponse = new PaginationResponse<LogFile>(dataUser, dataUser.PageInfo);
            return (paginationResponse, "ShowAllFileSuccess");
        }
    }
}
