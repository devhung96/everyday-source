using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Project.App.Databases
{
    public interface IRedisDatabaseProvider
    {
        IDatabase GetDatabase();
    }

    public class RedisDBContext : IRedisDatabaseProvider
    {
        private ConnectionMultiplexer ConnectionMultiplexer;
        private readonly IConfiguration Configuration;
        public RedisDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IDatabase GetDatabase()
        {
            if (ConnectionMultiplexer == null)
            {
                ConnectionMultiplexer = ConnectionMultiplexer.Connect(Configuration["ConnectionStrings:RedisConnection"]);
            }

            return ConnectionMultiplexer.GetDatabase();
        }
    }
}