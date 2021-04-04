using Project.App.DesignPatterns.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Services
{
    public interface IHelperScoreService
    {
        public (bool data, string message) CheckConditionEditScore(string classId, string subjectId, string semesterId);
        public (bool data, string message) CheckClassSubject(string classId, string subjectId);
    }
    public class HelperScoreService : IHelperScoreService
    {
        private readonly IRepositoryMariaWrapper repositoryMariaWrapper;
        public HelperScoreService(IRepositoryMariaWrapper _repositoryMariaWrapper)
        {
            repositoryMariaWrapper = _repositoryMariaWrapper;
        }
        public (bool data, string message) CheckConditionEditScore(string classId, string subjectId, string semesterId)
        {
            bool reportClosing = repositoryMariaWrapper.ReportClosings.FindByCondition(
                    x =>
                        x.ClassId.Equals(classId)
                        && x.SubjectId.Equals(subjectId)
                        && x.SemesterId.Equals(semesterId)
                ).Any();
            if (reportClosing == true)
            {
                return (reportClosing, "reportClosingExists");
            }
            return (reportClosing,"");
        }
        public (bool data, string message) CheckClassSubject(string classId, string subjectId)
        {
            bool classSchedule = repositoryMariaWrapper.ClassSchedules.FindByCondition(x => x.ClassId.Equals(classId) && x.SubjectId.Equals(subjectId)).Any();
            if (classSchedule == false)
                return (false, "SubjectNotInClass");
            return (true, "");
        }
    }
}
