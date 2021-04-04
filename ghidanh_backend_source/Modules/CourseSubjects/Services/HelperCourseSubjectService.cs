using Project.App.DesignPatterns.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.CourseSubjects.Services
{
    public interface IHelperCourseSubjectService
    {
        public (bool data, string message) CheckCondition(string courseSubjectId);
        public (bool data, string message) CheckCourseSubjectExistsInScore(string courseSubjectId);
        public (bool data, string message) CheckCourseSubjectExistsDuplicate(string subjectId, string scoreTypeId, string coureId);

    }
    public class HelperCourseSubjectService : IHelperCourseSubjectService
    {
        private readonly IRepositoryMariaWrapper repositoryMariaWrapper;
        private readonly IRepositoryMongoWrapper repositoryMongoWrapper;
        public HelperCourseSubjectService(IRepositoryMariaWrapper _repositoryMariaWrapper, IRepositoryMongoWrapper _repositoryMongoWrapper)
        {
            repositoryMariaWrapper = _repositoryMariaWrapper;
            repositoryMongoWrapper = _repositoryMongoWrapper;
        }
        public (bool data, string message) CheckCondition(string courseSubjectId)
        {
            Score courseSubject = repositoryMongoWrapper.Scores.FindByCondition(x => x.CourseSubjectId.Equals(courseSubjectId)).FirstOrDefault();
            if (courseSubject != null)
            {
                return (false, "CourseSubjectIdExistsInScores");
            }
            return (true, "");
        }

        public (bool data, string message) CheckCourseSubjectExistsInScore(string courseSubjectId)
        {
            Score score = repositoryMongoWrapper.Scores.FindByCondition(x => x.CourseSubjectId.Equals(courseSubjectId)).FirstOrDefault();
            if (score == null)
            {
                return (false, "CourseSubjectDoNotExistsInScore"); // không có trong bản điểm
            }
            return (true, "CheckCourseSubjectExistsInScore"); // đã có trong bản điểm
        }

        public (bool data, string message) CheckCourseSubjectExistsDuplicate(string subjectId, string scoreTypeId, string coureId)
        {
            bool courseSubjects = repositoryMariaWrapper.CourseSubjects.FindByCondition(x => x.SubjectId.Equals(subjectId) && x.ScoreTypeId.Equals(scoreTypeId) && x.CourseId.Equals(coureId)).Any();
            if (courseSubjects)
            {
                return (true, "CourseSubjectExistsDuplicate");// check trùng
            }
            return (false, "");
        }
    }
}
