using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.ScoreTypes.Entities;
using Project.Modules.ScoreTypes.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ScoreTypes.Services
{
    public interface ISocreTypeSubjectService
    {
        (object data, string message) ShowAllSocreTypeSubject(RequestTable requestTable);
        (ScoreTypeSubject data, string message) DetailSocreTypeSubject(string scoreTypeSubjectId);
        (ScoreTypeSubject data, string message) DeleteSocreTypeSubject(string scoreTypeSubjectId);
        (ScoreTypeSubject data, string message) CreateSocreTypeSubject(CreateSocreTypeSubjectRequest valueInput);
        (ScoreTypeSubject data, string message) EditSocreTypeSubject(EditScoreTypeSubjectRequest valueInput,string scoreTypeSubjectId);
        public List<ScoreTypeSubject> ShowAll();

    }
    public class SocreTypeSubjectService : ISocreTypeSubjectService
    {
        private readonly IRepositoryMariaWrapper repositoryWrapper;
        private readonly IRepositoryMongoWrapper repositoryMongoWrapper;
        public SocreTypeSubjectService(IRepositoryMariaWrapper _repositoryWrapper, IRepositoryMongoWrapper _repositoryMongoWrapper)
        {
            repositoryWrapper = _repositoryWrapper;
            repositoryMongoWrapper = _repositoryMongoWrapper;
        }

        public (ScoreTypeSubject data, string message) CreateSocreTypeSubject(CreateSocreTypeSubjectRequest valueInput)
        {
            ScoreTypeSubject scoreTypeSubject = new ScoreTypeSubject()
            {
                ScoreTypeId = valueInput.ScoreTypeId,
                SubjectId = valueInput.SubjectId,
                SchoolYearId = valueInput.SchoolYearId
            };
            repositoryWrapper.ScoreTypeSubjects.Add(scoreTypeSubject);
            repositoryWrapper.SaveChanges();
            return (scoreTypeSubject, "CreateSocreTypeSubjectSuccess");
        }

        public (ScoreTypeSubject data, string message) DeleteSocreTypeSubject(string scoreTypeSubjectId)
        {
            (ScoreTypeSubject scoreTypeSubject, string message) = DetailSocreTypeSubject(scoreTypeSubjectId);
            if (scoreTypeSubject == null)
            {
                return (scoreTypeSubject,message);
            }
            (bool checkConditionScoreTypeSubject, string messagecheckConditionScoreTypeSubject) = CheckConditionScoreTypeSubject(scoreTypeSubject.ScoreTypeSubjectId);
            if(checkConditionScoreTypeSubject == false)
            {
                return (null, messagecheckConditionScoreTypeSubject);
            }
            repositoryWrapper.ScoreTypeSubjects.RemoveMaria(scoreTypeSubject);
            repositoryWrapper.SaveChanges();
            return (scoreTypeSubject, " DeleteSocreTypeSubjectSuccess");
        }

        public (ScoreTypeSubject data, string message) DetailSocreTypeSubject(string scoreTypeSubjectId)
        {
            ScoreTypeSubject scoreTypeSubject = ShowAll().FirstOrDefault(x => x.ScoreTypeSubjectId.Equals(scoreTypeSubjectId));
            if(scoreTypeSubject == null)
            {
                return (scoreTypeSubject, "ScoreTypeSubjectIdDoNotExists");
            }
            return (scoreTypeSubject, "DetailSocreTypeSubjectSuccess");
        }

        public (ScoreTypeSubject data, string message) EditSocreTypeSubject(EditScoreTypeSubjectRequest valueInput, string scoreTypeSubjectId)
        {
            (ScoreTypeSubject scoreTypeSubject, string message) = DetailSocreTypeSubject(scoreTypeSubjectId);
            if (scoreTypeSubject == null)
            {
                return (scoreTypeSubject, message);
            }
            (bool checkConditionScoreTypeSubject, string messagecheckConditionScoreTypeSubject) = CheckConditionScoreTypeSubject(scoreTypeSubject.ScoreTypeSubjectId);
            if (checkConditionScoreTypeSubject == false)
            {
                return (null, messagecheckConditionScoreTypeSubject);
            }
            scoreTypeSubject.ScoreTypeId = valueInput.ScoreTypeId;
            scoreTypeSubject.SubjectId = valueInput.SubjectId;
            scoreTypeSubject.SchoolYearId = valueInput.SchoolYearId;
            repositoryWrapper.ScoreTypeSubjects.UpdateMaria(scoreTypeSubject);
            repositoryWrapper.SaveChanges();
            return (scoreTypeSubject, "EditSocreTypeSubjectSuccess");
        }
        

        public List<ScoreTypeSubject> ShowAll()
        {
            List<ScoreTypeSubject> socreTypeSubjectServices = repositoryWrapper.ScoreTypeSubjects.FindAll().ToList();
            return socreTypeSubjectServices;
        }
        public (object data, string message) ShowAllSocreTypeSubject(RequestTable requestTable)
        {
            List<ScoreTypeSubject> socreTypeSubjectServices = ShowAll();
            int totalRecord = socreTypeSubjectServices.Count();
            if (requestTable.Limit > 0 && requestTable.Page > 0)
            {
                socreTypeSubjectServices = socreTypeSubjectServices.Skip((requestTable.Page - 1) * requestTable.Limit).Take(requestTable.Limit).ToList();
            }
            ResponseTable responseTable = new ResponseTable()
            {
                Data = socreTypeSubjectServices,
                Info = new Info
                {
                    Limit = requestTable.Limit,
                    Page = requestTable.Page,
                    TotalRecord = totalRecord
                }
            };
            return (responseTable, "ShowAllSocreTypeSubjectSuccess");
        }
        public (bool data, string message) CheckConditionScoreTypeSubject(string ScoreTypeSubjectId)
        {
            //Score score = repositoryMongoWrapper.Scores.FindByCondition(x => x.ScoreTypeSubjectId.Equals(ScoreTypeSubjectId)).FirstOrDefault();
            //if (score != null)
            //{
            //    return (false, "ScoreTypeSubjectIdExists");
            //}
            return (true, "");
        }
    }
}
