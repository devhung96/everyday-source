using Project.App.Databases;
using Project.Modules.OpenVidus.Enities;
using Project.Modules.UserCodes.Enities;
using Project.Modules.Users.Entities;

namespace Repository
{
    public interface IRepositoryMongoWrapper
    {
        IRepositoryMongoBase<SessionStream> SessionStreams { get; }
        IRepositoryMongoBase<LogFile> LogFiles { get; }
        IRepositoryMongoBase<LogUserImport> LogUserImport { get; }
        IRepositoryMongoBase<LogUserCodeJob> LogUserCodeJobs { get; }
    }
    public class RepositoryMongoWrapper : IRepositoryMongoWrapper
    {
        public RepositoryMongoBase<SessionStream> sessionStreams;
        public RepositoryMongoBase<LogFile> logFiles;
        public RepositoryMongoBase<LogUserImport> logUserImports;
        public RepositoryMongoBase<LogUserCodeJob> logUserCodeJobs;
        private readonly MongoDBContext mongoDBContext;

        public RepositoryMongoWrapper(MongoDBContext MongoDBContext)
        {
            mongoDBContext = MongoDBContext;
        }

        public IRepositoryMongoBase<SessionStream> SessionStreams => sessionStreams ?? new RepositoryMongoBase<SessionStream>(mongoDBContext);
        public IRepositoryMongoBase<LogUserCodeJob> LogUserCodeJobs => logUserCodeJobs ?? new RepositoryMongoBase<LogUserCodeJob>(mongoDBContext);

        public IRepositoryMongoBase<LogFile> LogFiles => logFiles??=new RepositoryMongoBase<LogFile>(mongoDBContext);

        public IRepositoryMongoBase<LogUserImport> LogUserImport => logUserImports ??=new RepositoryMongoBase<LogUserImport>(mongoDBContext);
    }
}
