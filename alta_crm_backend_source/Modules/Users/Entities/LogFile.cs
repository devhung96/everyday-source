using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    public class LogFile
    {
        [BsonId]
        [BsonElement("log_file_id")]
        public string LogFileId { get; set; } = Guid.NewGuid().ToString();
        [BsonElement("file_name")]
        public string FileName { get; set; }

        [BsonElement("train_tags")]
        public List<TrainTag> TrainTags { get; set; } = new List<TrainTag>();
        [BsonElement("file_path ")]
        public string FilePath { get; set; }
        [BsonElement("import_date")]
        public DateTime ImportDate { get; set; }
    }
    public class TrainTag
    {
        public string TagCode { get; set; } 
        public string Message { get; set; }
        public bool IsTrainSuccess { get; set; } = false;
    }
}
