using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Project.App.Database
{
    public class MongoDBContext : DbContext
    {
       
        public MongoDBContext(IConfiguration config)
        {
         
            var client = new MongoClient(config["ConnectionSetting:MongoDBSettings:ConnectionStrings"]);
            _ = client.GetDatabase(config["ConnectionSetting:MongoDBSettings:DatabaseNames"]);
        }

    }
}
