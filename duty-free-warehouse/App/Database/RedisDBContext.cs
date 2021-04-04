using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Project.App.Database
{
    public interface IRedisDatabaseProvider
    {
        IDatabase GetDatabase();
    }

    public class RedisDBContext : IRedisDatabaseProvider
    {
        private ConnectionMultiplexer _redisMultiplexer;
        private readonly IConfiguration _config;
        public RedisDBContext(IConfiguration config)
        {
            _config = config;
        }
        public IDatabase GetDatabase()
        {
            if (_redisMultiplexer == null)
            {
                _redisMultiplexer = ConnectionMultiplexer.Connect(_config["ConnectionStrings:RedisConnection"]);
            }

            return _redisMultiplexer.GetDatabase();
        }
    }
}
