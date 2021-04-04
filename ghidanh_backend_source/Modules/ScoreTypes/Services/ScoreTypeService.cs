using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Classes.Validations;
using Project.Modules.CourseSubjects.Entities;
using Project.Modules.Scores.Services;
using Project.Modules.ScoreTypes.Entities;
using Project.Modules.ScoreTypes.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ScoreTypes.Services
{
    public interface IScoreTypeService
    {
        (object data, string message) ShowAllScoreType(RequestTable requestTable);
        (ScoreType data, string message) CreateScoreType(CreateScoreTypeRequest valueInput);
        (ScoreType data, string message) DetailScoreType(string scoreTypeId);
        (ScoreType data, string message) DeleteScoreType(string scoreTypeId);
        (ScoreType data, List<Score> scores, string message) EditScoreType(string scoreTypeId, EditScoreTypeRequest valueInput);

    }
    public class ScoreTypeService : IScoreTypeService
    {
        private readonly IRepositoryMariaWrapper repositoryWrapper;
        private readonly IHelperScoreService helperScoreService;
        private readonly IRepositoryMongoWrapper repositoryMongoWrapper;
        public ScoreTypeService(IRepositoryMariaWrapper _repositoryWrapper, IHelperScoreService _helperScoreService, IRepositoryMongoWrapper _repositoryMongoWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
            helperScoreService = _helperScoreService;
            repositoryMongoWrapper = _repositoryMongoWrapper;
        }
        public (ScoreType data, string message) CreateScoreType(CreateScoreTypeRequest valueInput)
        {
            ScoreType scoreType = new ScoreType()
            {
                ScoreTypeName = valueInput.ScoreTypeName,
                ScoreTypeMultiplier = valueInput.ScoreTypeMultiplier,
            };
            repositoryWrapper.ScoreTypes.Add(scoreType);
            repositoryWrapper.SaveChanges();
            return (scoreType, "CreateScoreTypeSuccess");
        }
        public (ScoreType data, string message) DeleteScoreType(string scoreTypeId)
        {
            (ScoreType scoreType,string message) = DetailScoreType(scoreTypeId);
            if (scoreType == null)
            {
                return (scoreType, message);
            }
            (bool checkCondition, string messageCheckCondition) = CheckCondition(scoreType.ScoreTypeId);
            if (checkCondition == false)
            {
                return (null, messageCheckCondition);
            }
            repositoryWrapper.ScoreTypes.RemoveMaria(scoreType);
            repositoryWrapper.SaveChanges();
            return (scoreType, "DeleteScoreTypeSuccess");
        }
        public (ScoreType data, string message) DetailScoreType(string scoreTypeId)
        {
            ScoreType scoreType = ShowAll().FirstOrDefault(x=>x.ScoreTypeId.Equals(scoreTypeId));
            if(scoreType == null)
            {
                return (scoreType, "ScoreTypeIdDoNotExists");
            }
            return (scoreType, "DetailScoreTypeSuccess");
        }
        public (ScoreType data,List<Score> scores ,string message) EditScoreType(string scoreTypeId, EditScoreTypeRequest valueInput)
        {
            (ScoreType scoreType, string message) = DetailScoreType(scoreTypeId);
            if (scoreType == null)
            {
                return (scoreType, null,message);
            }
            if(!valueInput.ScoreTypeName.Equals(scoreType.ScoreTypeName) && repositoryWrapper.ScoreTypes.Any(x=>x.ScoreTypeName.Equals(valueInput.ScoreTypeName)))
            {
                return (null, null, "ScoreTypeNameDuplicate");
            }
            scoreType.ScoreTypeName = valueInput.ScoreTypeName;
            scoreType.ScoreTypeMultiplier = valueInput.ScoreTypeMultiplier;
            repositoryWrapper.ScoreTypes.UpdateMaria(scoreType);
            repositoryWrapper.SaveChanges();
            List<Score> scores = EditPropertiesScoreScoreTypeId(scoreType);
            return (scoreType, scores, "EditScoreTypeSuccess");
        }
        public (bool data, string message) CheckCondition(string ScoreTypeId)
        {
            CourseSubject courseSubject = repositoryWrapper.CourseSubjects.FindByCondition(x=>x.ScoreTypeId.Equals(ScoreTypeId)).FirstOrDefault();
            if(courseSubject != null)
            {
                return (false, "ScoreTypeIdExistsInScoreTypeSubjects");
            }
            return (true,"");
        }
        public List<ScoreType> ShowAll()
        {
            List<ScoreType> scoreTypes = repositoryWrapper.ScoreTypes.FindAll().ToList();
            return scoreTypes;
        }
        public (object data, string message) ShowAllScoreType(RequestTable requestTable)
        {
            List<ScoreType> scoreTypes = ShowAll();
            scoreTypes = scoreTypes.Where(x => String.IsNullOrEmpty(requestTable.Search) || x.ScoreTypeName.ToLower().Contains(requestTable.Search.ToLower())).ToList();
            int totalRecord = scoreTypes.Count();
            if(requestTable.Limit > 0 && requestTable.Page > 0)
            {
                scoreTypes = scoreTypes.Skip((requestTable.Page - 1) * requestTable.Limit).Take(requestTable.Limit).ToList();
            }
            ResponseTable responseTable = new ResponseTable()
            {
                Data = scoreTypes,
                Info = new Info
                {
                    Limit = requestTable.Limit,
                    Page = requestTable.Page,
                    TotalRecord = totalRecord
                }
            };
            return (responseTable, "ShowAllScoreTypeSuccess");
        }
        public List<Score> EditPropertiesScoreScoreTypeId(ScoreType scoreType)
        {
            List<Score> scores = repositoryMongoWrapper.Scores.FindByCondition(x => x.ScoreTypeId.Equals(scoreType.ScoreTypeId)).ToList();
            scores = scores.Where(x => helperScoreService.CheckConditionEditScore(x.ClassId, x.SubjectId, x.SemestersId).data == false).ToList();
            foreach (Score score in scores)
            {
                score.Multiplier = scoreType.ScoreTypeMultiplier;
                repositoryMongoWrapper.Scores.UpdateMongo(x => x.ScoreId.Equals(score.ScoreId), score);
            }
            return scores;
        }
    }
}
