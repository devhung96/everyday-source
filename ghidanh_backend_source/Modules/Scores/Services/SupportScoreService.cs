using Microsoft.EntityFrameworkCore;
using Project.App.DesignPatterns.Repositories;
using Project.Modules.Classes.Entities;
using Project.Modules.CourseSubjects.Entities;
using Project.Modules.Scores.Entities;
using Project.Modules.ScoreTypes.Entities;
using Project.Modules.Students.Entities;
using Project.Modules.Subjects.Entities;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Services
{
    public interface ISupportScoreService
    {
        (object data, string message) ShowScoreForStudent(string classID, string studentID, string semesterID);
    }
    public class SupportScoreService : ISupportScoreService
    {
        private readonly IRepositoryMongoWrapper mongoDb;
        private readonly IRepositoryMariaWrapper mariaDb;
        private readonly IReportScoreService reportScoreService;
        public SupportScoreService(IRepositoryMongoWrapper repositoryMongoWrapper, IRepositoryMariaWrapper repositoryMariaWrapper, IReportScoreService reportScoreService)
        {
            this.mariaDb = repositoryMariaWrapper;
            this.mongoDb = repositoryMongoWrapper;
            this.reportScoreService = reportScoreService;
        }
        public (object data, string message) ShowScoreForStudent(string classID, string studentID, string semesterID)
        {
            // Get subjects off this class
            List<string> subjects = mariaDb.ClassSchedules.FindByCondition(x => x.ClassId.Equals(classID)).Select(x => x.SubjectId).ToList();
            if (subjects.Count < 1)
            {
                return (null, "SucjectNotFound");
            }
            //Check closing 
            
            List<string> reportClosings = mariaDb.ReportClosings.FindByCondition(x =>
            x.StudentId.Equals(studentID)
            && x.ClassId.Equals(classID)
            && x.SemesterId.Equals(semesterID)
            && subjects.Contains(x.SubjectId))
            .Select(x => x.SubjectId)
            .ToList();


            List<string> subjectsnoPegged = reportClosings;
            List<Header> headers = new List<Header>();


            List<CourseSubjectResponse> subjectResponses = new List<CourseSubjectResponse>();
            List<ResponseGetScoreForStudentSubjectClass> responseGetScores = new List<ResponseGetScoreForStudentSubjectClass>();
            Student student = mariaDb.Students.FirstOrDefault(x => x.StudentId.Equals(studentID));
            Class @class = mariaDb.Classes.FirstOrDefault(x => x.ClassId.Equals(classID));



            foreach (var subject in reportClosings)
            {
                var scores = mongoDb.Scores.FindByCondition(x =>
                x.StudentId.Equals(studentID)
                && x.ClassId.Equals(classID)
                && x.SemestersId.Equals(semesterID)
                && x.SubjectId.Equals(subject))
                .ToList();

                List<ScoreType> scoreTypes = mariaDb.ScoreTypes.FindByCondition(x => scores.Select(x => x.ScoreTypeId).ToList().Contains(x.ScoreTypeId)).ToList();


                foreach (var scoreType in scoreTypes)
                {
                    headers.Add(new Header
                    {
                        ScoreType = scoreType,
                        Latch = Latch.Pegged
                    });
                }

                var prepareScores = scores.GroupBy(x => x.SubjectId).Select(x => new { SubjectId  = x.Key, Scores = x.ToList()}).ToList();
        

                foreach (var score in scores)
                {
                    ResponseGetScoreForStudentSubjectClass responseGetScore = new ResponseGetScoreForStudentSubjectClass
                    {
                        ScoreTypeID = score.ScoreTypeId,
                        ScoreTypeName = scoreTypes.FirstOrDefault(x => x.ScoreTypeId.Equals(score.ScoreTypeId))?.ScoreTypeName,
                        ColumnNumber = score.ColumnNumber,
                        Score = score.ScoreSubject,
                        CheckRequired = score.CheckRequired,
                        ScoreTypeMultiplier = score.Multiplier,
                        MediumScore = reportScoreService.AverageScore(score.ScoreSubject.ToArray(), score.CheckRequired)
                    };
                    responseGetScores.Add(responseGetScore);
                }

                Subject subject1 = mariaDb.Subjects.FirstOrDefault(x => x.SubjectId.Equals(subject));
                var tmpCourseSubjectResponse = new CourseSubjectResponse { ScoreTypes = responseGetScores, Subject = subject1 };
                (tmpCourseSubjectResponse.MediumScore, tmpCourseSubjectResponse.Note) = fn_AverageScore(responseGetScores);
                subjectResponses.Add(tmpCourseSubjectResponse);
                subjects.Remove(subject);
            }
            var courseSubjects = mariaDb.CourseSubjects.FindByCondition(x => subjects.Contains(x.SubjectId)).Include(x => x.ScoreType).AsEnumerable().GroupBy(x=> x.SubjectId).ToList();
            foreach (var item in courseSubjects)
            {
                foreach (var courseSubject in item)
                {
                    if (!headers.Select(x => x.ScoreType.ScoreTypeId).Contains(courseSubject.ScoreTypeId))
                    {
                        headers.Add(new Header
                        {
                            ScoreType = mariaDb.ScoreTypes.FirstOrDefault(x => x.ScoreTypeId.Equals(courseSubject.ScoreTypeId)),
                            Latch = Latch.No_Pegged,

                        });
                    }
                    var scores = mongoDb.Scores.FindByCondition(x =>
                    x.StudentId.Equals(studentID)
                    && x.CourseSubjectId.Equals(courseSubject.CourseSubjectId)
                    && x.ClassId.Equals(classID)
                    && x.SubjectId.Equals(courseSubject.SubjectId)
                    && x.SemestersId.Equals(semesterID))
                    .ToList();


                    foreach (var score in scores)
                    {
                        ResponseGetScoreForStudentSubjectClass responseGetScore = new ResponseGetScoreForStudentSubjectClass
                        {
                            ScoreTypeID = courseSubject.ScoreTypeId,
                            ScoreTypeName = courseSubject.ScoreType?.ScoreTypeName,
                            ColumnNumber = courseSubject.ColumnNumber,
                            Score = (score.ScoreSubject is null || score.ScoreSubject.Count == 0) ? reportScoreService.GeneratorDoubleNull(courseSubject.ColumnNumber).ToList() : score.ScoreSubject,
                            CheckRequired = courseSubject.CheckRequired,
                            ScoreTypeMultiplier = courseSubject.ScoreType.ScoreTypeMultiplier,
                            MediumScore = reportScoreService.AverageScore(score.ScoreSubject.ToArray(), score.CheckRequired)
                        };
                        responseGetScores.Add(responseGetScore);
                    }
                }
               
                Subject subject2 = mariaDb.Subjects.FirstOrDefault(x => x.SubjectId.Equals(item.Key));
                var tmpCourseSubjectResponse = new CourseSubjectResponse { ScoreTypes = responseGetScores, Subject = subject2 };
                (tmpCourseSubjectResponse.MediumScore, tmpCourseSubjectResponse.Note) = fn_AverageScore(responseGetScores);
                subjectResponses.Add(tmpCourseSubjectResponse);
            }
            Response response = new Response()
            {
                Class = @class,
                Student = student,
                Subjects = subjectResponses,
                Headers = headers
            };
            return (response, "Success");
        }

        public (double? result, string message) fn_AverageScore(List<ResponseGetScoreForStudentSubjectClass> scores)
        {
            int hasValueScore = scores.Count(x => x.MediumScore.HasValue == false);
            if (hasValueScore > 0) return (null, "Thiếu điểm");
            if (scores.Sum(x => x.ScoreTypeMultiplier) == 0) return (0, "");
            var prepareScoreValue = scores.Where(x => x.MediumScore.HasValue == true).Select(x => x.MediumScore * x.ScoreTypeMultiplier).Sum().Value / scores.Sum(x => x.ScoreTypeMultiplier);
            return (Math.Round(prepareScoreValue, 1), "");
        }

        public class ResponseGetScoreForStudentSubjectClass
        {
            public string ScoreTypeID { get; set; }
            public string ScoreTypeName { get; set; }
            public int ColumnNumber { get; set; }
            public double ScoreTypeMultiplier { get; set; }
            public int CheckRequired { set; get; }
            public List<double?> Score { get; set; }
            public double? MediumScore { get; set; }

        }
        public class Header
        {
            public ScoreType ScoreType { get; set; }
            public Latch Latch { get; set; }
        }
        public class CourseSubjectResponse
        {
            public Subject Subject { get; set; }
            public List<ResponseGetScoreForStudentSubjectClass> ScoreTypes { get; set; }
            public double? MediumScore { get; set; }
            public string Note { get; set; }
        }
        public class Response
        {
            public Class Class { get; set; }
            public Student Student { get; set; }
            public List<Header> Headers { get; set; }
            public List<CourseSubjectResponse> Subjects { get; set; }
        }

        public enum Latch
        {
            Pegged = 1,
            No_Pegged = 0
        }

    }
}
