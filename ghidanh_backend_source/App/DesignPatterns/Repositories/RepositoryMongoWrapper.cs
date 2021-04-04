using MongoDB.Driver;
using Project.App.Databases;
using Project.Modules;
using Project.Modules.Scores.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.DesignPatterns.Repositories
{
    public interface IRepositoryMongoWrapper
    {
        IRepositoryRoot<Score> Scores { get; }
        IRepositoryRoot<AverageScore> AverageScores { get; }
        IClientSessionHandle Transaction();
    }
    public class RepositoryMongoWrapper : IRepositoryMongoWrapper
    {
        private  RepositoryMongoBase<Score> score;
        private  RepositoryMongoBase<AverageScore> averageScore;
        private readonly  MongoDBContext mongoDBContext;

        public RepositoryMongoWrapper(MongoDBContext MongoDBContext)
        {
            mongoDBContext = MongoDBContext;
        }
  
        public IRepositoryRoot<Score> Scores
        {
            get
            {
                return score ??
                    (score = new RepositoryMongoBase<Score>(mongoDBContext));
            }
        }
        public IRepositoryRoot<AverageScore> AverageScores
        {
            get
            {
                return averageScore ??
                    (averageScore = new RepositoryMongoBase<AverageScore>(mongoDBContext));
            }
        }
        public IClientSessionHandle Transaction()
        {
            return mongoDBContext.Session;
        }
    }

}
