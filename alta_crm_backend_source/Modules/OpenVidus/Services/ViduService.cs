using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Project.App.Helpers;
using Project.Modules.OpenVidus.Enities;
using Repository;
using System;
using System.Net.Http;
namespace Project.Modules.OpenVidus.Services
{
    public interface IViduService
    {
        (int? status, string data, SessionStream sessionStream) RegisterUserToOpenViduAsync(string account, string eventId = null, bool IsPublisher = false);
        (int? status, string message) RemoveRoomVidu(string accountId, string eventID, string Token, bool IsPublisher = false);
        SessionStream InsertStream(SessionStream sessionStream);
    }
    public class ViduService : IViduService
    {
        private readonly IConfiguration Configuration;
        private readonly string OpenViduUrl;
        private readonly IRepositoryMongoWrapper MongoDb;
        public ViduService(IConfiguration configuration, IRepositoryMongoWrapper mongoDb)
        {
            Configuration = configuration;
            OpenViduUrl = Configuration["OpenViduUrl"] ?? "";
            MongoDb = mongoDb;
        }

        public SessionStream InsertStream(SessionStream sessionStream)
        {
            MongoDb.SessionStreams.Add(sessionStream);
            return sessionStream;
        }

        public (int? status, string data, SessionStream sessionStream) RegisterUserToOpenViduAsync(string userId, string roomId = null, bool IsPublisher = false)
        {
            string url = OpenViduUrl + "/api/get-token";
            object body = new
            {
                role = !IsPublisher ? "SUBSCRIBER" : "PUBLISHER",
                user = userId ?? "pikachu" + Guid.NewGuid().ToString(),
                room = roomId
            };
            SessionStream sessionStream = new SessionStream
            {
                SessionCode = roomId,
                SessionName = userId + "-" + roomId
            };
            
            (string responseData, int? responseStatusCode) = HttpMethod.Post.SendRequestWithStringContent(url, JsonConvert.SerializeObject(body));
            if (responseStatusCode == 200)
            {
                //sessionStream = InsertStream(sessionStream);
                return (responseStatusCode.Value, responseData, null);
            }
            return (responseStatusCode, responseData, null);
        }
        public (int? status, string message) RemoveRoomVidu(string userId, string roomId, string Token, bool IsPublisher = false)
        {
            string url = OpenViduUrl + "/api/remove-user";

            object content = new
            {
                role = !IsPublisher ? "SUBSCRIBER" : "PUBLISHER",
                user = userId,
                room = roomId,
                token = Token
            };
            (string responseData, int? responseStatusCode) = HttpMethod.Post.SendRequestWithStringContent(url, JsonConvert.SerializeObject(content));
            if (responseStatusCode == 200)
            {
                return (responseStatusCode.Value, responseData);
            }
            return (null, responseData);
        }
    }
}