using MongoDB.Driver;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.CourseSubjects.Entities;
using Project.Modules.CourseSubjects.Services;
using Project.Modules.SchoolYears.Entities;
using Project.Modules.Scores.Entities;
using Project.Modules.Scores.Requests;
using Project.Modules.ScoreTypes.Entities;
using Project.Modules.ScoreTypes.Services;
using Project.Modules.Subjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Services
{
    public interface IScoreService
    {
        (object data, string message) ShowAllScore(RequestTable requestTable);
        (List<Score> data, string message) CreateScore(List<CreateScoreRequest> valueInputs);
        (Score data, string message) Detail(string scoreId);
        (ScoreReponse data, string message) DetailScore(string scoreId);
        (Score data, string message) DeleteScore(string scoreId);
        (Score data, string message) EditScore(string scoreId, EditCreateScoreRequest valueInput);
        (object data, string message) EditListScore(List<EditListScoreRequest> valueInputs);
        List<Score> ShowAll();
    }
    public class ScoreService : IScoreService
    {
        private readonly IRepositoryMongoWrapper repositoryMongoWrapper;
        private readonly ISocreTypeSubjectService IsocreTypeSubjectService;
        private readonly IRepositoryMariaWrapper repositoryWrapper;
        private readonly ICourseSubjectService courseSubjectService;
        private readonly IScoreTypeService IScoreTypeService;
        private readonly IHelperScoreService helperScoreService;


        public ScoreService(
                IRepositoryMongoWrapper _repositoryMongoWrapper, 
                ISocreTypeSubjectService _IsocreTypeSubjectService, 
                IRepositoryMariaWrapper _repositoryWrapper, 
                ICourseSubjectService _courseSubjectService, 
                IScoreTypeService _IScoreTypeService,
                IHelperScoreService _helperScoreService
            )
        {
            repositoryMongoWrapper = _repositoryMongoWrapper;
            IsocreTypeSubjectService = _IsocreTypeSubjectService;
            repositoryWrapper = _repositoryWrapper;
            courseSubjectService = _courseSubjectService;
            IScoreTypeService = _IScoreTypeService;
            helperScoreService = _helperScoreService;
        }

        public List<Score> ShowAll()
        {
            List<Score> scores = repositoryMongoWrapper.Scores.FindAll().ToList();
            return scores;
        }

        public ( Score data, string message) Create(CreateScoreRequest valueInput)
        {
            (CourseSubject courseSubject, _) = courseSubjectService.ShowDetailCourseSubject(valueInput.CourseSubjectId);
            (ScoreType scoreType, _) = IScoreTypeService.DetailScoreType(courseSubject.ScoreTypeId);
            if (valueInput.ScoreSubject.Count() < courseSubject.ColumnNumber)
            {
                int length = courseSubject.ColumnNumber - valueInput.ScoreSubject.Count();
                for (int i = 0; i < length; i++)
                {
                    valueInput.ScoreSubject.Add(null);
                }    
            }
            (bool checkCondition, string messageCheckCondition) = helperScoreService.CheckConditionEditScore(valueInput.ClassId, courseSubject.SubjectId, valueInput.SemestersId);
            if (checkCondition == true)
            {
                return (null, messageCheckCondition);
            }
            (bool checkClassSubject, string messageCheckClassSubject) = helperScoreService.CheckClassSubject(valueInput.ClassId, courseSubject.SubjectId);
            if(checkClassSubject == false)
            {
                return (null, messageCheckClassSubject);
            }
            Score score = new Score()
            {
                CourseSubjectId = valueInput.CourseSubjectId,
                StudentId = valueInput.StudentId,
                ClassId = valueInput.ClassId,
                ScoreSubject = valueInput.ScoreSubject,
                SemestersId = valueInput.SemestersId,
                SubjectId = courseSubject.SubjectId,
                CheckRequired = courseSubject.CheckRequired,
                ColumnNumber = courseSubject.ColumnNumber,
                Multiplier = scoreType.ScoreTypeMultiplier,
                ScoreTypeId = courseSubject.ScoreTypeId
            };
            repositoryMongoWrapper.Scores.Add(score);
            return (score, "CreateScoreSuccess");
        }

        public (List<Score> data, string message) CreateScore(List<CreateScoreRequest> valueInputs)
        {
            List<Score> scores = new List<Score>();
            foreach (CreateScoreRequest valueInput in valueInputs)
            {
                Score scoreValue = ShowAll()
                    .FirstOrDefault(x =>
                            x.CourseSubjectId.Equals(valueInput.CourseSubjectId)
                            && x.StudentId.Equals(valueInput.StudentId)
                            && x.ClassId.Equals(valueInput.ClassId)
                            && x.SemestersId.Equals(valueInput.SemestersId)
                            );
                if (scoreValue == null)
                {
                    (Score createScore, string messageCreate) = Create(valueInput);
                    if(createScore == null)
                    {
                        return (null,messageCreate);
                    }
                    scores.Add(createScore);
                }
                else
                {
                    (bool checkCondition, string messageCheckCondition) = helperScoreService.CheckConditionEditScore(scoreValue.ClassId, scoreValue.SubjectId, scoreValue.SemestersId);
                    if (checkCondition == true)
                    {
                        return (null, messageCheckCondition);
                    }
                    double? scoreSubject = valueInput.ScoreSubject.FirstOrDefault();
                    for (int i = 0; i < scoreValue.ScoreSubject.Count(); i++)
                    {
                        if (scoreValue.ScoreSubject[i] == null)
                        {
                            scoreValue.ScoreSubject[i] = scoreSubject;
                            break;
                        }
                    }
                    repositoryMongoWrapper.Scores.UpdateMongo(x=>x.ScoreId.Equals(scoreValue.ScoreId),scoreValue);
                    scores.Add(scoreValue);
                }
            }
            return (scores, "CreateScoreSuccess");
        }

        public (object data, string message) ShowAllScore(RequestTable requestTable)
        {
            List<Score> scores = ShowAll();
            List<ScoreReponse> scoreReponses = new List<ScoreReponse>();
            foreach (Score score in scores)
            {
                string ClassName = repositoryWrapper.Classes.FindByCondition(x => x.ClassId.Equals(score.ClassId)).Select(s => s.ClassName).FirstOrDefault();
                string StudentName = repositoryWrapper.Students.FindByCondition(x => x.StudentId.Equals(score.StudentId)).Select(s => (s.StudentLastName + s.StudentFirstName)).FirstOrDefault();
                string SubjectName = repositoryWrapper.Subjects.FindByCondition(x => x.SubjectId.Equals(score.SubjectId)).Select(s => s.SubjectName).FirstOrDefault();
                string SemeterName = repositoryWrapper.Semesters.FindByCondition(x => x.SemesterId.Equals(score.SubjectId)).Select(s => s.SemesterName).FirstOrDefault();
                string scoreTypeName = repositoryWrapper.ScoreTypes.FindByCondition(x => x.ScoreTypeId.Equals(score.ScoreTypeId)).Select(s => s.ScoreTypeName).FirstOrDefault();
                ScoreReponse scoreReponse = new ScoreReponse(score)
                {
                    ClassName = ClassName,
                    StudentName = StudentName,
                    SubjectName = SubjectName,
                    SemeterName = SemeterName,
                    ScoreTypeName = scoreTypeName
                };
                scoreReponses.Add(scoreReponse);
            }
            int totalRecord = scoreReponses.Count();
            if (requestTable.Limit > 0 && requestTable.Page > 0)
            {
                scoreReponses = scoreReponses.Skip((requestTable.Page - 1) * requestTable.Limit).Take(requestTable.Limit).ToList();
            }
            ResponseTable responseTable = new ResponseTable()
            {
                Data = scoreReponses,
                Info = new Info
                {
                    Limit = requestTable.Limit,
                    Page = requestTable.Page,
                    TotalRecord = totalRecord
                }
            };
            return (responseTable, "ShowAllScoreSuccess");
        }

        public (Score data, string message) Detail(string scoreId)
        {
            Score score = ShowAll().FirstOrDefault(x => x.ScoreId.Equals(scoreId));
            if (score == null)
            {
                return (null, "ScoreIdDoNotExists");
            }
            return (score, "DetailScoreSuccess");
        }

        public (Score data, string message) EditScore(string scoreId, EditCreateScoreRequest valueInput)
        {
            (Score score, string message) = Detail(scoreId);
            DateTime date = DateTime.Now;
            if (score == null)
            {
                return (null, message);
            }
            score.CourseSubjectId = valueInput.CourseSubjectId;
            score.StudentId = valueInput.StudentId;
            score.ClassId = valueInput.ClassId;
            score.ScoreSubject = valueInput.ScoreSubject;
            score.SemestersId = valueInput.SemestersId;
            (CourseSubject courseSubject, _) = courseSubjectService.ShowDetailCourseSubject(score.CourseSubjectId);
            (ScoreType scoreType, _) = IScoreTypeService.DetailScoreType(courseSubject.ScoreTypeId);
            score.SubjectId = courseSubject.SubjectId;
            score.CheckRequired = courseSubject.CheckRequired;
            score.ColumnNumber = courseSubject.ColumnNumber;
            score.Multiplier = scoreType.ScoreTypeMultiplier;
            score.ScoreTypeId = courseSubject.ScoreTypeId;
            repositoryMongoWrapper.Scores.UpdateMongo(x => x.ScoreId.Equals(score.ScoreId), score);
            return (score, "EditScoreSuccess");
        }

        public (bool data, string message) CheckSchoolYear(string scoreTypeSubjectId, DateTime data)
        {
            List<ScoreTypeSubject> scoreTypeSubjects = IsocreTypeSubjectService.ShowAll();
            ScoreTypeSubject scoreTypeSubject = scoreTypeSubjects.FirstOrDefault(x => x.ScoreTypeSubjectId.Equals(scoreTypeSubjectId));
            SchoolYear schoolYear = repositoryWrapper.SchoolYears.FindByCondition(x => x.SchoolYearId.Equals(scoreTypeSubject.SchoolYearId)).FirstOrDefault();
            if ((int)data.Year >= schoolYear.TimeStart && (int)data.Year >= schoolYear.TimeEnd)
            {
                return (true, "");
            }
            return (false, "DoNotEditTheScore");
        }

        public (Score data, string message) DeleteScore(string scoreId)
        {
            (Score score, string message) = Detail(scoreId);
            if (score == null)
            {
                return (score, message);
            }
            (bool checkCondition, string messageCheckCondition) = helperScoreService.CheckConditionEditScore(score.ClassId,score.SubjectId,score.SemestersId);
            if(checkCondition == true)
            {
                return (null, messageCheckCondition);
            }
            repositoryMongoWrapper.Scores.RemoveMongo(x => x.ScoreId.Equals(score.ScoreId));
            return (score, "DeleteScoreSuccess");
        }

        public (ScoreReponse data, string message) DetailScore(string scoreId)
        {
            (Score score, string message) = Detail(scoreId);
            if (score == null)
            {
                return (null, message);
            }
            string ClassName = repositoryWrapper.Classes.FindByCondition(x => x.ClassId.Equals(score.ClassId)).Select(s => s.ClassName).FirstOrDefault();
            string StudentName = repositoryWrapper.Students.FindByCondition(x => x.StudentId.Equals(score.StudentId)).Select(s => (s.StudentLastName + s.StudentFirstName)).FirstOrDefault();
            string SubjectName = repositoryWrapper.Subjects.FindByCondition(x => x.SubjectId.Equals(score.SubjectId)).Select(s => s.SubjectName).FirstOrDefault();
            string SemeterName = repositoryWrapper.Semesters.FindByCondition(x => x.SemesterId.Equals(score.SemestersId)).Select(s => s.SemesterName).FirstOrDefault();
            string scoreTypeName = repositoryWrapper.ScoreTypes.FindByCondition(x => x.ScoreTypeId.Equals(score.ScoreTypeId)).Select(s => s.ScoreTypeName).FirstOrDefault();
            ScoreReponse scoreReponse = new ScoreReponse(score)
            {
                ClassName = ClassName,
                StudentName = StudentName,
                SubjectName = SubjectName,
                SemeterName = SemeterName,
                ScoreTypeName = scoreTypeName
            };
            return (scoreReponse, message);
        }

        public (object data, string message) EditListScore(List<EditListScoreRequest> valueInputs)
        {
            List<Score> scores = new List<Score>();
            List<Score> scoreResponse = new List<Score>();
            foreach (EditListScoreRequest valueInput in valueInputs)
            {
                scores = ShowAll()
                   .Where(x =>
                           x.ClassId.Equals(valueInput.ClassId)
                           && x.StudentId.Equals(valueInput.StudentId)
                           && x.SemestersId.Equals(valueInput.SemestersId)
                       ).ToList();
                foreach (CourseSubjectList CourseSubjectList in valueInput.CourseSubjectList)
                {
                    Score score = scores.Where(x => x.CourseSubjectId.Equals(CourseSubjectList.CourseSubjectId)).FirstOrDefault();
                    if(score == null)
                    {
                        
                        CreateScoreRequest createScoreRequest = new CreateScoreRequest()
                        {
                            CourseSubjectId = CourseSubjectList.CourseSubjectId,
                            StudentId = valueInput.StudentId,
                            ClassId = valueInput.ClassId,
                            SemestersId = valueInput.SemestersId,
                            ScoreSubject = CourseSubjectList.ScoreSubject
                        };
                        (Score scoreCreate,string messageCreate) = Create(createScoreRequest);
                        if (scoreCreate == null)
                        {
                            return (null, messageCreate);
                        }
                        scoreResponse.Add(scoreCreate);
                    }
                    else
                    {
                        (bool checkCondition, string messageCheckCondition) = helperScoreService.CheckConditionEditScore(score.ClassId, score.SubjectId, score.SemestersId);
                        if (checkCondition == true)
                        {
                            return (null, messageCheckCondition); // kiểm tra điểm có được chốt hay chưa
                        }
                        (bool checkClassSubject, string messageCheckClassSubject) = helperScoreService.CheckClassSubject(score.ClassId, score.SubjectId);
                        if (checkClassSubject == false)
                        {
                            return (null, messageCheckClassSubject); // kiểm tra môn có trong lớp
                        }
                        score.ScoreSubject = CourseSubjectList.ScoreSubject;
                        repositoryMongoWrapper.Scores.UpdateMongo(x => x.ScoreId.Equals(score.ScoreId), score);
                        scoreResponse.Add(score);
                    }
                }
            }
            return (scoreResponse, "EditListScore");
        }
    }
}
