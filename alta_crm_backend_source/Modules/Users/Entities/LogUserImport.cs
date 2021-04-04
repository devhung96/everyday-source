using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    public class LogUserImport
    {
        [BsonId]
        [BsonElement("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [BsonElement("log_file_id")]
        public string LogFileId { get; set; }
        [BsonElement("is_success")]
        public bool IsSuccess { get; set; } = false;
        [BsonElement("error_message")]
        public List<string> ErrorMessage { get; set; } = new List<string>();
    
        [BsonElement("is_schedule")]
        public bool IsSchedule { get; set; }
        [BsonElement("message_schedule")]
        public string MessageSchedule { get; set; }
        /// <summary>
        /// Thông tin file excel
        /// </summary>
        /// 
        [BsonElement("user_id")]
        public string UserId { get; set; }   
        [BsonElement("user_code")]
        public string UserCode { get; set; }
        [BsonElement("first_name")]
        public string FirstName { get; set; }
        [BsonElement("last_name")]
        public string LastName { get; set; }
        [BsonElement("user_gender")]
        public int  Gender { get; set; }
        [BsonElement("user_images")]
        public List<InfoImage> Images { get; set; } = new List<InfoImage> { };
        [BsonElement("user_phone")]
        public string  Phone { get; set; }
        [BsonElement("user_email")]
        public string  Email { get; set; }
        [BsonElement("group_code")]
        public string GroupCode { get; set; }  
        [BsonElement("tag_code")]
        public string TagCode { get; set; }


    }
    public class InfoImage
    {
        public string path { get; set; }
        public string pathSuccess { get; set; }
        public bool isFace { get; set; } = false;
        public string message { get; set; }
    }
}
