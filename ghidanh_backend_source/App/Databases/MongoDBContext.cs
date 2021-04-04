using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Project.App.Databases
{
    public class MongoDBContext : DbContext
    {
        public readonly IMongoDatabase MongoDatabase;
        private readonly IConfiguration Configuration;
        public readonly IClientSessionHandle Session;
        public MongoDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
            MongoClient client = new MongoClient(Configuration["ConnectionSetting:MongoDBSettings:ConnectionStrings"]);
            Session = client.StartSession();
            //MongoDatabase = client.GetDatabase(Configuration["ConnectionSetting:MongoDBSettings:DatabaseNames"]);
            MongoDatabase = Session.Client.GetDatabase(Configuration["ConnectionSetting:MongoDBSettings:DatabaseNames"]);
        }
        public IMongoCollection<dynamic> Users => MongoDatabase.GetCollection<dynamic>("users");
        public IMongoCollection<dynamic> Scores => MongoDatabase.GetCollection<dynamic>("scores");
    }
}
