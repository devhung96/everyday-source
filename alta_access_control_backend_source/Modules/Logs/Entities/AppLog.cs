using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Logs.Entities
{
    public class AppLog
    {
        [BsonId]
        [BsonElement("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [BsonElement("key")]
        public string Key { get; set; }
        [BsonElement("value")]
        public string Value { get; set; }
    }
}
