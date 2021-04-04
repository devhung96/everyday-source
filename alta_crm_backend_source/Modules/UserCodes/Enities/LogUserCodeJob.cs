using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.UserCodes.Enities
{
    public class LogUserCodeJob
    {

        [BsonId]
        [BsonElement("log_id")]
        public string LogFileId { get; set; } = Guid.NewGuid().ToString();


        [BsonElement("user_code_id")]
        public string UserCodeId { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("user_code_active")]
        public string UserCodeActive { get; set; }

        [BsonElement("user_code_expire")]
        public DateTime? UserCodeExpire { get; set; }

        [BsonElement("user_code_created_at")]
        public DateTime? UserCodeCreatedAt { get; set; } = DateTime.UtcNow;

    }
}
