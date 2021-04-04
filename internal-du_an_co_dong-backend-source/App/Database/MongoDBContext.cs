using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Project.App.Providers;
using Project.Modules.Chats.Entities;
using Project.Modules.Logs.Entities;
using Project.Modules.Question.Entities;
//using Project.Modules.Surveys.Entities;

namespace Project.App.Database
{
    public class MongoDBContext : DbContext
    {
        private readonly IMongoDatabase _mongoDBContext;
        public IConfiguration _config;

        private readonly MongoClient _client;
        public MongoDBContext(IConfiguration configuration)
        {
            _config = configuration;
            _client = new MongoClient(_config["ConnectionSetting:MongoDBSettings:ConnectionStrings"]);
            _mongoDBContext = _client.GetDatabase(_config["ConnectionSetting:MongoDBSettings:DatabaseNames"]);
        }

        //public IMongoCollection<HistorySpin> HistorySpins => _mongoDBContext.GetCollection<HistorySpin>("knorr_history_spin");

        public MongoClient MongoClient()
        {
            return _client;
        }
        //public IMongoCollection<SurveyCode> SurveyCodes => _mongoDBContext.GetCollection<SurveyCode>("daihoicodong_survey_code");
        public IMongoCollection<ResultAnswers> ResultAnswers => _mongoDBContext.GetCollection<ResultAnswers>("daihoicodong_result_answers");
        public IMongoCollection<LogAccess> LogAccesses => _mongoDBContext.GetCollection<LogAccess>("daihoicodong_log_access");
        public IMongoCollection<Chat> Chats => _mongoDBContext.GetCollection<Chat>("daihoicodong_chat");
    }
}
