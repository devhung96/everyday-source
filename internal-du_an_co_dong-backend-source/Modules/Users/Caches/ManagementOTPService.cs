using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Caches
{

    public interface IManagementOTPService
    {
        public object GetOTP(string key);
        public object SetOTP(string key, string idUser);
        public void RemoveOTP(string key);
    }
    public class ManagementOTPService: IManagementOTPService
    {
        private readonly IMemoryCache _memoryCache;

        private readonly IConfiguration _configuration;
        protected long _timeExpired; // second


        public ManagementOTPService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _timeExpired = _configuration["TimeExpiredOTP"].TimeExpiredOTP();
        }

        /// <summary>
        /// Kiểm tra OTP còn thời gian sử dụng không nếu hết sẽ trả về null. còn có thì trả về mã OTP đó. (theo user)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetOTP(string key)
        {
            object data;
            if (_memoryCache.TryGetValue(key, out data))
            {
                return data;
            }
            return data;
        }


        public object SetOTP(string key, string idUser)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(_timeExpired));
            _memoryCache.Set(key, idUser, cacheEntryOptions);
            return idUser;
        }

        public void RemoveOTP(string key)
        {
            _memoryCache.Remove(key);
        }



    }
}
