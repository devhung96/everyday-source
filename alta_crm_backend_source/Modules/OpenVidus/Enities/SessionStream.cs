using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.OpenVidus.Enities
{
    public class SessionStream
    {
        [BsonId, BsonElement("session_id")]
        public string SessionId { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("session_name")]
        public string SessionName { get; set; }

        [BsonElement("session_code")]
        public string SessionCode { get; set; }

        [BsonElement("session_settings")]
        public string SessionSettings { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
    }
}
