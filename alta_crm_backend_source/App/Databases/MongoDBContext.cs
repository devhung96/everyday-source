using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Project.Modules.UserCodes.Enities;
using Project.Modules.Users.Entities;

namespace Project.App.Databases
{
    public class MongoDBContext : DbContext
    {
        public IMongoDatabase MongoDatabase;
        public MongoDBContext(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration["ConnectionSetting:MongoDBSettings:ConnectionStrings"]);
            MongoDatabase = client.GetDatabase(configuration["ConnectionSetting:MongoDBSettings:DatabaseNames"]);
        }

        public IMongoCollection<dynamic> Users => MongoDatabase.GetCollection<dynamic>("users");
        public IMongoCollection<LogFile> LogFiles => MongoDatabase.GetCollection<LogFile>("log_files");
        public IMongoCollection<LogUserImport> LogUserImports => MongoDatabase.GetCollection<LogUserImport>("log_user_imports");
        public IMongoCollection<LogUserCodeJob> LogUserCodeJobs => MongoDatabase.GetCollection<LogUserCodeJob>("log_remove_code_users");
    }
}
