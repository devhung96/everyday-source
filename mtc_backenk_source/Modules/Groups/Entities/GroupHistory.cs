using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Entities
{
    public class GroupHistory
    {
        [BsonId]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [BsonElement("group_id")]
        public string GroupId { get; set; }
        [BsonElement("gtroup_name")]
        public string GroupName { get; set; }
     
        [BsonElement("user_id")]
        public string UserId { get; set; }
        [BsonElement("user_name")]
        public string UserName { get; set; }
        [BsonElement("expired_new")]
        public DateTime ExpiredNew { get; set; }
        [BsonElement("expired_old")]
        public DateTime? ExpiredOld { get; set; }
        [BsonElement("change_at")]
        public DateTime ChangedAt { get; set; }
    }
}
