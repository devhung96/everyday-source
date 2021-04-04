using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Logs.Entities
{
    public enum LogStatus
    {
        Success = 1,
        Fail = 0
    }
    public class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("log_name")]
        public string LogName { get; set; } 
        [BsonElement("first_name")]
        public string FirstName { get; set; }     
        [BsonElement("last_name")]
        public string LastName { get; set; }
        [BsonElement("device_id")]
        public string DeviceId { get; set; }
        [BsonElement("log_access")]
        public string LogAccess { get; set; }
        [BsonElement("log_access_time")]
        public DateTime LogAccessTime { get; set; }
        [BsonElement("log_region")]
        public string LogRegion { get; set; }
        [BsonElement("user_id")]
        public string UserId { get; set; }
        [BsonElement("log_status")]
        public LogStatus LogStatus { get; set; }
        [BsonElement("log_message")]
        public string LogMessage { get; set; }
        [BsonElement("log_created_at")]
        public DateTime LogCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
