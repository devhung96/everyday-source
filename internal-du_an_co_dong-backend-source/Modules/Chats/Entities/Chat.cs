using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Chats.Entities
{
    public class Chat
    {
        [BsonId]
        public string ChatId { get; set; } = Guid.NewGuid().ToString();
        [BsonElement("event_id")]
        public string Event_Id { get; set; }
        [BsonElement("chat_usersent")]
        public string UserSent { get; set; } // nếu UserSent =  null là addmin
        [BsonElement("chat_chatcontent")]
        public string ChatContent { get; set; }
        [BsonElement("chat_userrecieve")]
        public string UserRecieve { set; get; } // nếu UserRecieve =  null là addmin
    }
}
