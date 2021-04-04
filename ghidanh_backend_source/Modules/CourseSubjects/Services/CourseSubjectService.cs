using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Courses.Entities;
using Project.Modules.CourseSubjects.Entities;
using Project.Modules.CourseSubjects.Requests;
using Project.Modules.Scores.Entities;
using Project.Modules.Scores.Services;
using Project.Modules.ScoreTypes.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Project.Modules.CourseSubjects.Services
{
    public interface ICourseSubjectService
    {
        public (object courseSubjects, string message) ShowAllCourseSubject(RequestTable requestTable);
        public (CourseSubject courseSubject, string message) ShowDetailCourseSubject(string courseSubjectId);
        public (List<CourseSubject> courseSubject, string message) CreateCourseSubject(List<CreateCourseSubjectRequest> valueInputs);
        public (CourseSubject courseSubject, string message) DeleteCourseSubject(string courseSubjectId);
        public (CourseSubject courseSubject,List<Score> scores, string message) EditCourseSubject(string courseSubjectId, EditCourseSubjectRequest valueInput);
        public (List<CourseSubjectResponse> courseSubjects, string message) FilterCourseSubject(FilterCourseSubjectRequest valueInput);

    }
    public class CourseSubjectService : ICourseSubjectService
    {
        private readonly IRepositoryMariaWrapper repositoryMariaWrapper;
        private readonly IRepositoryMongoWrapper repositoryMongoWrapper;
        private readonly IHelperScoreService helperScoreService;
        private readonly IHelperCourseSubjectService helperCourseSubjectService;

        public CourseSubjectService(IRepositoryMariaWrapper _repositoryMariaWrapper, IRepositoryMongoWrapper _repositoryMongoWrapper, IHelperScoreService _helperScoreService, IHelperCourseSubjectService _helperCourseSubjectService)
        {
            repositoryMariaWrapper = _repositoryMariaWrapper;
            repositoryMongoWrapper = _repositoryMongoWrapper;
            helperScoreService = _helperScoreService;
            helperCourseSubjectService = _helperCourseSubjectService;
        }
        
        public (List<CourseSubject> courseSubject, string message) CreateCourseSubject(List<CreateCourseSubjectRequest> valueInputs)
        {
            List<CourseSubject> courseSubjects = new List<CourseSubject>();
            foreach (CreateCourseSubjectRequest valueInput in valueInputs)
            {
                (bool checkCourseSubjectExistsDuplicate, string messageCheckCourseSubjectExistsDuplicate) = helperCourseSubjectService.CheckCourseSubjectExistsDuplicate(valueInput.SubjectId,valueInput.ScoreTypeId,valueInput.CourseId);
                if(checkCourseSubjectExistsDuplicate)
                {
                    return (null, messageCheckCourseSubjectExistsDuplicate);
                }
                CourseSubject courseSubject = new CourseSubject()
                {
                    CourseId = valueInput.CourseId,
                    SubjectId = valueInput.SubjectId,
                    ColumnNumber = valueInput.ColumnNumber,
                    ScoreTypeId = valueInput.ScoreTypeId,
                    CheckRequired = valueInput.CheckRequired
                };
                repositoryMariaWrapper.CourseSubjects.Add(courseSubject);
                repositoryMariaWrapper.SaveChanges();
                courseSubjects.Add(courseSubject);
            }
            return (courseSubjects, "CreateCourseSubjectSuccess");
        }
        
        public (CourseSubject courseSubject, string message) DeleteCourseSubject(string courseSubjectId)
        {
            (CourseSubject courseSubject, string message) = ShowDetailCourseSubject(courseSubjectId);
            if (courseSubject == null)
            {
                return (courseSubject, "courseSubjectIdDoNotExists");
            }
            (bool data, string messagecourseSubject) = helperCourseSubjectService.CheckCondition(courseSubject.CourseSubjectId);
            if(data == false)
            {
                return (null, messagecourseSubject);
            }
            repositoryMariaWrapper.CourseSubjects.RemoveMaria(courseSubject);
            repositoryMariaWrapper.SaveChanges();
            return (courseSubject, "DeleteCourseSubjectSuccess");
        }
        
        public (CourseSubject courseSubject, List<Score> scores, string message) EditCourseSubject(string courseSubjectId, EditCourseSubjectRequest valueInput)
        {
            (CourseSubject courseSubject, string message) = ShowDetailCourseSubject(courseSubjectId);
            if (courseSubject == null)
            {
                return (courseSubject,null, message);
            }
            if(!courseSubject.SubjectId.Equals(valueInput.SubjectId) || !courseSubject.ScoreTypeId.Equals(valueInput.ScoreTypeId) || !courseSubject.CourseId.Equals(valueInput.CourseId))
            {
                (bool checkCourseSubjectExistsDuplicate, string messageCheckCourseSubjectExistsDuplicate) = helperCourseSubjectService.CheckCourseSubjectExistsDuplicate(valueInput.SubjectId, valueInput.ScoreTypeId, valueInput.CourseId);
                if (checkCourseSubjectExistsDuplicate)
                {
                    return (null,null,messageCheckCourseSubjectExistsDuplicate);
                }
            }
            courseSubject.CourseId = valueInput.CourseId;
            courseSubject.SubjectId = valueInput.SubjectId;
            courseSubject.ColumnNumber = valueInput.ColumnNumber;
            courseSubject.ScoreTypeId = valueInput.ScoreTypeId;
            courseSubject.CheckRequired = valueInput.CheckRequired;
            repositoryMariaWrapper.CourseSubjects.UpdateMaria(courseSubject);
            repositoryMariaWrapper.SaveChanges();
            List<Score> scores = EditPropertiesScore(courseSubject);
            return (courseSubject,scores, "EditCourseSubjectSuccess");
        }
        
        public List<CourseSubject> ShowAll()
        {
            List<CourseSubject> courseSubjects = repositoryMariaWrapper.CourseSubjects.FindAll().ToList();
            return courseSubjects;
        }
        
        public (object courseSubjects, string message) ShowAllCourseSubject(RequestTable requestTable)
        {
            List<CourseSubject> courseSubjects = ShowAll();
            List<CourseSubjectResponse> courseSubjectResponses = new List<CourseSubjectResponse>();
            foreach (CourseSubject courseSubject in courseSubjects)
            {
                string CourseName = repositoryMariaWrapper.Courses.FindByCondition(x => x.CourseId.Equals(courseSubject.CourseId)).Select(s => s.CourseName).FirstOrDefault();
                string SubjectName = repositoryMariaWrapper.Subjects.FindByCondition(x => x.SubjectId.Equals(courseSubject.SubjectId)).Select(s => s.SubjectName).FirstOrDefault();
                ScoreType scoreType = repositoryMariaWrapper.ScoreTypes.FindByCondition(x => x.ScoreTypeId.Equals(courseSubject.ScoreTypeId)).FirstOrDefault();
                CourseSubjectResponse courseSubjectResponse = new CourseSubjectResponse(courseSubject)
                {
                    CourseName = CourseName,
                    SubjectName = SubjectName,
                    ScoreTypeName = scoreType == null ? null : scoreType.ScoreTypeName,
                    Multiplier = scoreType == null ? null : (double?)scoreType.ScoreTypeMultiplier
                };
                courseSubjectResponses.Add(courseSubjectResponse);
            }
            courseSubjectResponses = courseSubjectResponses.Where(x => String.IsNullOrEmpty(requestTable.Search) || x.SubjectName.ToUpper().Contains(requestTable.Search.ToUpper())).ToList();
            int totalRecord = courseSubjectResponses.Count();
            if(requestTable.Page > 0 && requestTable.Limit > 0 )
            {
                courseSubjectResponses = courseSubjectResponses.Skip((requestTable.Page - 1) * requestTable.Limit).Take(requestTable.Limit).ToList(); 
            }
            if (!String.IsNullOrEmpty(requestTable.SortOrder) && !String.IsNullOrEmpty(requestTable.SortField))
            {
                courseSubjectResponses = courseSubjectResponses.AsQueryable().OrderBy(requestTable.SortField.OrderValue(requestTable.SortOrder)).ToList();
            }
            ResponseTable responseTable = new ResponseTable()
            {
                Data = courseSubjectResponses,
                Info = new Info
                {
                    Limit = requestTable.Limit,
                    Page = requestTable.Page,
                    TotalRecord = totalRecord
                }
            };
            return (responseTable, "ShowAllCourseSubjectSuccess");
        }
        
        public (CourseSubject courseSubject, string message) ShowDetailCourseSubject(string courseSubjectId)
        {
            CourseSubject courseSubject = repositoryMariaWrapper.CourseSubjects.FindByCondition(x => x.CourseSubjectId.Equals(courseSubjectId)).FirstOrDefault();
            if(courseSubject == null)
            {
                return (courseSubject, "courseSubjectIdDoNotExists");
            }
            return (courseSubject, "ShowDetailCourseSubjectSuccess");
        }
        
        public (List<CourseSubjectResponse> courseSubjects, string message) FilterCourseSubject(FilterCourseSubjectRequest valueInput)
        {
            List<CourseSubject> courseSubjects = ShowAll().Where(x => x.SubjectId.Equals(valueInput.SubjectId) && x.CourseId.Equals(valueInput.CourseId)).ToList();
            List<CourseSubjectResponse> courseSubjectResponses = new List<CourseSubjectResponse>();
            foreach (CourseSubject courseSubject in courseSubjects)
            {
                string CourseName = repositoryMariaWrapper.Courses.FindByCondition(x => x.CourseId.Equals(courseSubject.CourseId)).Select(s => s.CourseName).FirstOrDefault();
                string SubjectName = repositoryMariaWrapper.Subjects.FindByCondition(x => x.SubjectId.Equals(courseSubject.SubjectId)).Select(s => s.SubjectName).FirstOrDefault();
                ScoreType scoreType = repositoryMariaWrapper.ScoreTypes.FindByCondition(x => x.ScoreTypeId.Equals(courseSubject.ScoreTypeId)).FirstOrDefault();
                CourseSubjectResponse courseSubjectResponse = new CourseSubjectResponse(courseSubject)
                {
                    CourseName = CourseName,
                    SubjectName = SubjectName,
                    ScoreTypeName = scoreType == null ? null : scoreType.ScoreTypeName,
                    Multiplier = scoreType == null ? null : (double?)scoreType.ScoreTypeMultiplier
                };
                courseSubjectResponses.Add(courseSubjectResponse);
            }
            return (courseSubjectResponses, "FilterCourseSubjectSuccess");
        }

        public List<Score> EditPropertiesScore(CourseSubject courseSubject)
        {
            CourseSubjectResponse courseSubjectResponse = courseSubjectResponseDetail(courseSubject);
            repositoryMariaWrapper.SaveChanges();
            List<Score> scores = repositoryMongoWrapper.Scores.FindByCondition(x => x.CourseSubjectId.Equals(courseSubject.CourseSubjectId)).ToList();
            scores = scores.Where(x => helperScoreService.CheckConditionEditScore(x.ClassId, x.SubjectId, x.SemestersId).data == false).ToList();
            foreach (Score score in scores)
            {
                score.ColumnNumber = courseSubjectResponse.ColumnNumber;
                score.CheckRequired = courseSubjectResponse.CheckRequired;
                score.SubjectId = courseSubjectResponse.SubjectId;
                score.ScoreTypeId = courseSubjectResponse.ScoreTypeId;
                score.Multiplier = (int)courseSubjectResponse.Multiplier;
                if (score.ScoreSubject.Count() < score.ColumnNumber)
                {
                    int length = score.ColumnNumber - score.ScoreSubject.Count();
                    for (int i = 0; i < length; i++)
                    {
                        score.ScoreSubject.Add(null);
                    }
                }
                repositoryMongoWrapper.Scores.UpdateMongo(x=>x.ScoreId.Equals(score.ScoreId),score);
            }
            return scores;
        }

        public CourseSubjectResponse courseSubjectResponseDetail(CourseSubject valueInput)
        {
            string CourseName = repositoryMariaWrapper.Courses.FindByCondition(x => x.CourseId.Equals(valueInput.CourseId)).Select(s => s.CourseName).FirstOrDefault();
            string SubjectName = repositoryMariaWrapper.Subjects.FindByCondition(x => x.SubjectId.Equals(valueInput.SubjectId)).Select(s => s.SubjectName).FirstOrDefault();
            ScoreType scoreType = repositoryMariaWrapper.ScoreTypes.FindByCondition(x => x.ScoreTypeId.Equals(valueInput.ScoreTypeId)).FirstOrDefault();
            CourseSubjectResponse courseSubjectResponse = new CourseSubjectResponse(valueInput)
            {
                CourseName = CourseName,
                SubjectName = SubjectName,
                ScoreTypeName = scoreType == null ? null : scoreType.ScoreTypeName,
                Multiplier = scoreType == null ? null : (double?)scoreType.ScoreTypeMultiplier
            };
            return (courseSubjectResponse);
        }

    }
}