using MongoDB.Driver;
using Project.App.Database;
using Project.Modules.Chats.Entities;
using Project.Modules.Chats.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Chats.Services
{
    public interface IChatService 
    {
        (Chat data, string message) Stored(StoredChatRequest value);
        (List<Chat> data, string message) ShowAllUserSent();
        List<Chat> ShowAll();
    }
    public class ChatService : IChatService
    {
        private readonly MongoDBContext _mongoDBContext;
        public ChatService(MongoDBContext mongoDBContext)
        {
            _mongoDBContext = mongoDBContext;
        }

        public List<Chat> ShowAll()
        {
            List<Chat> chats = _mongoDBContext.Chats.Find(x => true).ToList();
            return chats;
        }

        public (List<Chat> data, string message) ShowAllUserSent()
        {
            List<Chat> chats = ShowAll();
            throw new NotImplementedException();
        }

        public (Chat data, string message) Stored(StoredChatRequest value)
        {
            Chat chat = new Chat();
            chat.UserSent = value.UserSent;
            chat.UserRecieve = value.UserRecieve;
            chat.ChatContent = value.ChatContent;
            chat.Event_Id = value.Event_Id;
            _mongoDBContext.Chats.InsertOne(chat);
            return (chat,"Sent Message Success!!!");
        }
    }
}
