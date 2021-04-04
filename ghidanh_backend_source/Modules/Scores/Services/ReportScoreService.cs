using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Classes.Entities;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.Courses.Entities;
using Project.Modules.CourseSubjects.Entities;
using Project.Modules.Scores.Entities;
using Project.Modules.Scores.Requests;
using Project.Modules.ScoreTypes.Entities;
using Project.Modules.Semesters.Entities;
using Project.Modules.Students.Entities;
using Project.Modules.SubjectGroups.Entities;
using Project.Modules.Subjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using static Project.Modules.Students.Entities.Student;

namespace Project.Modules.Scores.Services
{
    public interface IReportScoreService
    {
        public (TranscriptByClass data, string message) GetTranscript(string classId, string subjectId, string semestersId, string search);
        public (TranscriptByClass data, string message) ClosingPoint(string classId, string subjectId, string semestersId);
        (object data, string message) GetSubjectByClass(string classId, RequestTable requestTable);
        double?[] GeneratorDoubleNull(int length);
        public (double? result, string message) fn_AverageScore(List<ItemChildScore> scores);
        public double? AverageScore(double?[] scores, int checkRequired);

        public (bool result, string message) DestroyClosingPoint(string classId, string subjectId, string semestersId);

        public (TranscriptBySubject data, string message) GetTranscriptByClassAndStudent(string classId, string studentId, string semestersId, string search);
    }
    public class ReportScoreService : IReportScoreService
    {
        private readonly IRepositoryMongoWrapper _repositoryMongoWrapper;
        private readonly IRepositoryMariaWrapper _repositoryMariaWrapper;
        public ReportScoreService(IRepositoryMongoWrapper repositoryMongoWrapper, IRepositoryMariaWrapper repositoryMariaWrapper)
        {
            _repositoryMongoWrapper = repositoryMongoWrapper;
            _repositoryMariaWrapper = repositoryMariaWrapper;
        }


        public (TranscriptByClass data, string message) GetTranscript(string classId, string subjectId, string semestersId, string search)
        {
            TranscriptByClass transcriptByClass = new TranscriptByClass();

            Class classObject = _repositoryMariaWrapper.Classes.FirstOrDefault(x => x.ClassId == classId);
            if (classObject is null) return (null, "ClassNotFound");

            Subject subject = _repositoryMariaWrapper.Subjects.FirstOrDefault(x => x.SubjectId == subjectId);
            if (subject is null) return (null, "SubjectNotFound");


            Semester semester = _repositoryMariaWrapper.Semesters.FirstOrDefault(x => x.SemesterId == semestersId);
            if (semester is null) return (null, "SemesterNotFound");

            // Get header
            transcriptByClass.ClassName = classObject.ClassName;
            transcriptByClass.Headers = this.GetHeaderTranscript(classObject, subject, semester, out bool closed);
            transcriptByClass.Closed = closed;
            //get student in class 
            List<Student> students = GetStudentsByClassId(classObject);


            var perpareScore = _repositoryMongoWrapper.Scores.FindByCondition(x => x.ClassId == classId && x.SemestersId == semester.SemesterId && x.SubjectId == subject.SubjectId).ToList();
            
            if (closed)
            {
                var scores = perpareScore.GroupBy(x => x.StudentId).Select(x => new { StudentId = x.Key, Scores = x.ToList() }).ToList();

                foreach (var item in students)
                {
                    ItemTranscriptByClass itemTranscriptByClass = new ItemTranscriptByClass
                    {
                        StudentId = item.StudentId,
                        StudentCode = item.StudentCode,
                        StudentLastName = item.StudentLastName,
                        StudentFistName = item.StudentFirstName,
                        StudentGender = item.StudentGender,
                        StudentName = item.StudentFirstName + "" + item.StudentLastName
                    };

                    var tmpScore = scores.FirstOrDefault(x => x.StudentId == item.StudentId);
                    if(tmpScore != null)
                    {
                        foreach (var score in tmpScore.Scores)
                        {
                            ItemChildScore itemChildScore = new ItemChildScore
                            {
                                ScoreTypeId = score.ScoreTypeId,
                                ScoreTypeMultiplier = score.Multiplier,
                                ScoreTypeNumber = score.ColumnNumber,
                                CheckRequired = score.CheckRequired,
                                CourseSubjectId = score.CourseSubjectId,
                                ScoreValue = score.ScoreSubject.ToArray(),
                                //MediumValue = AverageScore(score.ScoreSubject.ToArray(), score.CheckRequired)
                            };

                            itemTranscriptByClass.Scores.Add(itemChildScore);
                        }
                    } 
                    (itemTranscriptByClass.MediumScore, itemTranscriptByClass.Note) = fn_AverageScore(itemTranscriptByClass.Scores);
                    transcriptByClass.Data.Add(itemTranscriptByClass);
                }
            }
            else
            {

                var scores = perpareScore.GroupBy(x => new { StudentId  = x.StudentId, ScoreTypeId = x.ScoreTypeId } ).Select(x => new { StudentId = x.Key.StudentId, ScoreTypeId = x.Key.ScoreTypeId ,  Scores = x.ToList() }).ToList();
                // chưa chot
                foreach (var item in students)
                {
                    ItemTranscriptByClass itemTranscriptByClass = new ItemTranscriptByClass
                    {
                        StudentId = item.StudentId,
                        StudentCode = item.StudentCode,
                        StudentLastName = item.StudentLastName,
                        StudentFistName = item.StudentFirstName,
                        StudentGender = item.StudentGender,
                        StudentName = item.StudentFirstName + "" + item.StudentLastName
                    };

                    foreach (var itemHeader in transcriptByClass.Headers)
                    {
                        var tmpScore = scores.FirstOrDefault(x => x.StudentId == item.StudentId && x.ScoreTypeId == itemHeader.ScoreTypeId);
                        if(tmpScore is null)
                        {
                            ItemChildScore itemChildScore = new ItemChildScore
                            {
                                ScoreTypeId = itemHeader.ScoreTypeId,
                                ScoreTypeMultiplier = itemHeader.ScoreTypeMultiplier,
                                ScoreTypeNumber = itemHeader.ScoreTypeNumber,
                                CheckRequired = itemHeader.CheckRequired,
                                ScoreValue = GeneratorDoubleNull(itemHeader.ScoreTypeNumber),
                                //MediumValue = null,
                                CourseSubjectId = itemHeader.CourseSubjectId
                            };
                            itemTranscriptByClass.Scores.Add(itemChildScore);

                        }
                        else
                        {
                            foreach (var score in tmpScore.Scores)
                            {
                                ItemChildScore itemChildScore = new ItemChildScore
                                {
                                    ScoreTypeId = score.ScoreTypeId,
                                    ScoreTypeMultiplier = score.Multiplier,
                                    ScoreTypeNumber = score.ColumnNumber,
                                    CheckRequired = score.CheckRequired,
                                    CourseSubjectId = score.CourseSubjectId,
                                    ScoreValue = score.ScoreSubject.ToArray(),
                                    //MediumValue = AverageScore(score.ScoreSubject.ToArray(), score.CheckRequired)
                                };

                                itemTranscriptByClass.Scores.Add(itemChildScore);
                            }
                        }
                        
                    }
                    (itemTranscriptByClass.MediumScore, itemTranscriptByClass.Note) = fn_AverageScore(itemTranscriptByClass.Scores);
                    transcriptByClass.Data.Add(itemTranscriptByClass);
                }
            }
            transcriptByClass.Data = transcriptByClass.Data.Where(x => String.IsNullOrEmpty(search) || x.StudentName.Contains(search) ).ToList();
            return (transcriptByClass, "GetSuccess");
        }


        public double?[] GeneratorDoubleNull(int length)
        {
            double?[] resutl = new double?[length];
            for (int i = 0; i < length; i++)
            {
                resutl[i] = null;
            }
            return resutl;
        }


        public (TranscriptByClass data, string message) ClosingPoint(string classId, string subjectId, string semestersId)
        {
            TranscriptByClass transcriptByClass = new TranscriptByClass();
            Class classObject = _repositoryMariaWrapper.Classes.FirstOrDefault(x => x.ClassId == classId);
            if (classObject is null) return (null, "ClassNotFound");

            Subject subject = _repositoryMariaWrapper.Subjects.FirstOrDefault(x => x.SubjectId == subjectId);
            if (subject is null) return (null, "SubjectNotFound");


            Semester semester = _repositoryMariaWrapper.Semesters.FirstOrDefault(x => x.SemesterId == semestersId);
            if (subject is null) return (null, "SemesterNotFound");

            ReportClosing reportClosing = _repositoryMariaWrapper.ReportClosings.FirstOrDefault(x => x.ClassId == classObject.ClassId && x.SubjectId == subject.SubjectId && x.SemesterId == semester.SemesterId);
            if (reportClosing != null) return (null, "ClosingPoint");


            transcriptByClass.Headers = this.GetHeaderTranscript(classObject, subject, semester,out bool closed);

            var perpareScore = _repositoryMongoWrapper.Scores.FindByCondition(x => x.ClassId == classId && x.SemestersId == semester.SemesterId && x.SubjectId == subject.SubjectId).ToList();
            var scores = perpareScore.GroupBy(x => new { StudentId = x.StudentId, ScoreTypeId = x.ScoreTypeId }).Select(x => new { StudentId = x.Key.StudentId, ScoreTypeId = x.Key.ScoreTypeId, Scores = x.ToList() }).ToList();
            //get student in class 
            List<Student> students = GetStudentsByClassId(classObject);

            foreach (var item in students)
            {
                ItemTranscriptByClass itemTranscriptByClass = new ItemTranscriptByClass
                {
                    StudentId = item.StudentId,
                    StudentCode = item.StudentCode,
                    StudentLastName = item.StudentLastName,
                    StudentFistName = item.StudentFirstName,
                    StudentGender = item.StudentGender,
                    StudentName = item.StudentFirstName + "" + item.StudentLastName
                };
                foreach (var itemHeader in transcriptByClass.Headers)
                {
                    var tmpScore = scores.FirstOrDefault(x => x.StudentId == item.StudentId && x.ScoreTypeId == itemHeader.ScoreTypeId);
                    if (tmpScore is null)
                    {
                        ItemChildScore itemChildScore = new ItemChildScore
                        {
                            ScoreTypeId = itemHeader.ScoreTypeId,
                            ScoreTypeMultiplier = itemHeader.ScoreTypeMultiplier,
                            ScoreTypeNumber = itemHeader.ScoreTypeNumber,
                            CheckRequired = itemHeader.CheckRequired,
                            ScoreValue = GeneratorDoubleNull(itemHeader.ScoreTypeNumber),
                            //MediumValue = null,
                            CourseSubjectId = itemHeader.CourseSubjectId
                        };
                        itemTranscriptByClass.Scores.Add(itemChildScore);

                        // Them vao db 
                        Score newScore = new Score
                        {
                            ClassId = classId,
                            ScoreSubject = itemChildScore.ScoreValue.ToList(),
                            ScoreTypeId = itemChildScore.ScoreTypeId,
                            SemestersId = semester.SemesterId,
                            SubjectId = subjectId, 
                            StudentId = item.StudentId,
                            Multiplier  = itemChildScore.ScoreTypeMultiplier,
                            CheckRequired = itemChildScore.CheckRequired,
                            ColumnNumber = itemHeader.ScoreTypeNumber,
                            CourseSubjectId = itemHeader.CourseSubjectId
                        };
                        _repositoryMongoWrapper.Scores.Add(newScore);
                    }
                    else
                    {
                        foreach (var score in tmpScore.Scores)
                        {
                            ItemChildScore itemChildScore = new ItemChildScore
                            {
                                ScoreTypeId = score.ScoreTypeId,
                                ScoreTypeMultiplier = score.Multiplier,
                                ScoreTypeNumber = score.ColumnNumber,
                                CheckRequired = score.CheckRequired,
                                CourseSubjectId = score.CourseSubjectId,
                                ScoreValue = score.ScoreSubject.ToArray(),
                                //MediumValue = AverageScore(score.ScoreSubject.ToArray(), score.CheckRequired)
                            };
                            itemTranscriptByClass.Scores.Add(itemChildScore);
                        }
                    }

                   
                }

                (itemTranscriptByClass.MediumScore, itemTranscriptByClass.Note) = fn_AverageScore(itemTranscriptByClass.Scores);
                transcriptByClass.Data.Add(itemTranscriptByClass);

                //Tinh diem trung binh
                AverageScore tmpAerageScore = _repositoryMongoWrapper.AverageScores.FindByCondition(x => x.ClassId == classObject.ClassId && x.SubjectId == item.StudentId && x.SubjectId == subject.SubjectId && x.SemesterId == semester.SemesterId).FirstOrDefault();
                if (tmpAerageScore is null)
                {
                    AverageScore averageScore = new AverageScore
                    {
                        ClassId = classObject.ClassId,
                        StudentId = item.StudentId,
                        SubjectId = subject.SubjectId,
                        AvergeScoreValue = itemTranscriptByClass.MediumScore,
                        SemesterId = semestersId
                    };
                    _repositoryMongoWrapper.AverageScores.Add(averageScore);

                }
                else
                {
                    tmpAerageScore.AvergeScoreValue = itemTranscriptByClass.MediumScore;
                    _repositoryMongoWrapper.AverageScores.UpdateMongo(x => x.AvergeScoreId == tmpAerageScore.AvergeScoreId, tmpAerageScore);
                }

                _repositoryMariaWrapper.ReportClosings.Add(new ReportClosing
                {
                    ClassId = classObject.ClassId,
                    StudentId = item.StudentId,
                    SemesterId = semester.SemesterId,
                    SubjectId = subject.SubjectId
                });
                _repositoryMariaWrapper.SaveChanges();

            }
            return (transcriptByClass, "SuccessfulClosing");
        }


        public (bool result, string message) DestroyClosingPoint(string classId, string subjectId, string semestersId)
        {

            TranscriptByClass transcriptByClass = new TranscriptByClass();
            Class classObject = _repositoryMariaWrapper.Classes.FirstOrDefault(x => x.ClassId == classId);
            if (classObject is null) return (false, "ClassNotFound");

            Subject subject = _repositoryMariaWrapper.Subjects.FirstOrDefault(x => x.SubjectId == subjectId);
            if (subject is null) return (false, "SubjectNotFound");


            Semester semester = _repositoryMariaWrapper.Semesters.FirstOrDefault(x => x.SemesterId == semestersId);
            if (subject is null) return (false, "SemesterNotFound");


            ReportClosing reportClosing = _repositoryMariaWrapper.ReportClosings.FirstOrDefault(x => x.ClassId == classObject.ClassId && x.SubjectId == subject.SubjectId && x.SemesterId == semester.SemesterId);
            if (reportClosing is null ) return (false, "NoTranscriptsHaveNotBeenFinalized");

            List<ReportClosing> reportClosings = _repositoryMariaWrapper.ReportClosings.FindByCondition(x => x.ClassId == classId && x.SubjectId == subjectId && x.SemesterId == semestersId).ToList();
            //_repositoryMariaWrapper.ReportClosings.RemoveMaria(reportClosings.FirstOrDefault());
            _repositoryMariaWrapper.ReportClosings.RemoveRangeMaria(reportClosings);

            int averageScores = _repositoryMongoWrapper.AverageScores.FindByCondition(x => x.ClassId == classId && x.SubjectId == subjectId && x.SemesterId == semestersId).Count();

            if (averageScores > 0)
            {
                _repositoryMongoWrapper.AverageScores.RemoveRangeMongo(x => x.ClassId == classId && x.SubjectId == subjectId && x.SemesterId == semestersId);
            }
            _repositoryMariaWrapper.SaveChanges();
            return (true, "DestroyClosingPointSuccess");

        }

        public (object data, string message) GetSubjectByClass(string classId, RequestTable requestTable)
        {
            List<Subject> subjects = new List<Subject>();

            var classSchedules = _repositoryMariaWrapper.ClassSchedules.FindByCondition(x => x.ClassId == classId).GroupBy(x=>  x.SubjectId ).Select(x=>x.Key).ToList();
            foreach (var item in classSchedules)
            {
                Subject subject = _repositoryMariaWrapper.Subjects.FirstOrDefault(x => x.SubjectId == item);
                if(subject != null)
                {
                    string subjectGroupName = _repositoryMariaWrapper.SubjectGroups.FindByCondition(x => x.SubjectGroupId.Equals(subject.SubjectGroupId)).Select(x => x.SubjectGroupName).FirstOrDefault();
                    subject.SubjectGroupName = subjectGroupName;
                    subjects.Add(subject);
                }
            }
            subjects = subjects.Where(x => String.IsNullOrEmpty(requestTable.Search) || x.SubjectName.ToUpper().Contains(requestTable.Search.ToUpper())).ToList();
            int totalRecord = subjects.Count();
            if (requestTable.Page > 0 && requestTable.Limit > 0)
            {
                subjects = subjects.Skip((requestTable.Page - 1) * requestTable.Limit).Take(requestTable.Limit).ToList();
            }
            ResponseTable responseTable = new ResponseTable()
            {
                Data = subjects,
                Info = new Info
                {
                    Limit = requestTable.Limit,
                    Page = requestTable.Page,
                    TotalRecord = totalRecord
                }
            };
            return (responseTable, "GetSubjectByClassSuccess");
        }




        public (double? result, string message) fn_AverageScore(List<ItemChildScore> scores)
        {

            //vi pham da ra
            int invalidScore = scores.Where(x => x.ScoreValue.Count(y => y.HasValue == true) < x.CheckRequired).Count();
            if(invalidScore> 0) return (null, "Thiếu điểm");


            // tinh diem 
            var preareScore = scores
                .Where(x => 
                            x.ScoreValue.Count(y => y.HasValue == true) >= x.CheckRequired && 
                            x.ScoreValue.Count(y => y.HasValue == true) > 0
                       )
                .ToList();

            double scoreMedium = preareScore.Sum(x => x.ScoreTypeMultiplier * x.ScoreValue.Sum().Value) / preareScore.Sum(x => x.ScoreValue.Count(y => y.HasValue == true) * x.ScoreTypeMultiplier);

            if (double.IsNaN(scoreMedium)) return (0, "");
            return (Math.Round(scoreMedium, 1), "");
        }

        public (double? result, string message) fn_AverageScore(List<ItemChildScoreBySubject> scores)
        {
            //vi pham da ra
            int invalidScore = scores.Where(x => x.ScoreValue.Count(y => y.HasValue == true) < x.CheckRequired).Count();
            if (invalidScore > 0) return (null, "Thiếu điểm");


            // tinh diem 
            var preareScore = scores
                .Where(x =>
                            x.ScoreValue.Count(y => y.HasValue == true) >= x.CheckRequired &&
                            x.ScoreValue.Count(y => y.HasValue == true) > 0
                       )
                .ToList();

            double scoreMedium = preareScore.Sum(x => x.ScoreTypeMultiplier * x.ScoreValue.Sum().Value) / preareScore.Sum(x => x.ScoreValue.Count(y => y.HasValue == true) * x.ScoreTypeMultiplier);

            if (double.IsNaN(scoreMedium)) return (0, "");
            return (Math.Round(scoreMedium, 1), "");
        }

        public double? AverageScore(double?[] scores, int checkRequired)
        {
            int hasValueScore = scores.Count(x => x.HasValue == true);
            int hasNotValueScore = scores.Count(x => x.HasValue == false);

            if (checkRequired == 0)
            {
                var prepareScores = scores.Where(x => x.HasValue == true).ToList();
                if (prepareScores.Count() == 0) return null;
                return prepareScores.Sum() / prepareScores.Count();
            }
            else if (hasValueScore >= checkRequired)
            {
                var prepareScores = scores.Where(x => x.HasValue == true).ToList();
                return prepareScores.Sum() / prepareScores.Count();
            }
            else
            {
                    return null;
            }

        }

        public List<Student> GetStudentsByClassId(Class classObject)
        {
            List<Student> students = new List<Student>();
            if (classObject is null) return students;
            List<string> idStudents = _repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.ClassId == classObject.ClassId).Select(x => x.StudentId).ToList();
            return _repositoryMariaWrapper.Students.FindByCondition(x => idStudents.Contains(x.StudentId)).ToList(); ;
        }


        public List<ItemHeader> GetHeaderTranscript(Class classObject, Subject subject, Semester semesters, out bool closed)
        {
            closed = false;
            List<ItemHeader> resultHeader = new List<ItemHeader>();
            if (classObject is null) return resultHeader;

            // Kiem tra chot diem chua
            ReportClosing reportClosing = _repositoryMariaWrapper.ReportClosings.FindByCondition(x => x.ClassId == classObject.ClassId && x.SubjectId == subject.SubjectId && x.SemesterId == semesters.SemesterId).FirstOrDefault();
            if (reportClosing == null)
            {
                // Chưa chốt thì lấy hiện hành
                List<CourseSubject> courseSubjects = _repositoryMariaWrapper.CourseSubjects.FindByCondition(x => x.SubjectId == subject.SubjectId && x.CourseId == classObject.CourseID).Include(x => x.ScoreType).ToList();
                foreach (var item in courseSubjects)
                {
                    var itemHeader = new ItemHeader
                    {
                        ScoreTypeId = item.ScoreTypeId,
                        ScoreTypeName = item.ScoreType.ScoreTypeName,
                        ScoreTypeMultiplier = item.ScoreType.ScoreTypeMultiplier,
                        ScoreTypeNumber = item.ColumnNumber,
                        CheckRequired = item.CheckRequired,
                        CourseSubjectId = item.CourseSubjectId
                    };
                    resultHeader.Add(itemHeader);
                }
            }
            else
            {
                closed = true;
                // Chốt rồi thì lấy thông tin trong bảng điểm
                List<Score> scores = _repositoryMongoWrapper.Scores.FindByCondition(x => x.ClassId == classObject.ClassId && x.SubjectId == subject.SubjectId).ToList();
                if (scores is null) return resultHeader;

                var groupScore = scores.GroupBy(x => x.StudentId).Select(x => new { StudentId = x.Key, Scores = x.ToList() }).FirstOrDefault();
                if (groupScore is null) return resultHeader;

                List<ScoreType> allScoreTypes = _repositoryMariaWrapper.ScoreTypes.FindAll().ToList();

                foreach (var item in groupScore.Scores)
                {
                    var itemHeader = new ItemHeader
                    {
                        ScoreTypeId = item.ScoreTypeId,
                    };
                    ScoreType tmpScoreType = allScoreTypes.FirstOrDefault(x => x.ScoreTypeId == item.ScoreTypeId);
                    if (tmpScoreType is null) continue;
                    itemHeader.ScoreTypeName = tmpScoreType.ScoreTypeName;
                    itemHeader.ScoreTypeMultiplier = tmpScoreType.ScoreTypeMultiplier;
                    itemHeader.ScoreTypeNumber = item.ColumnNumber;
                    itemHeader.CheckRequired = item.CheckRequired;
                    itemHeader.CourseSubjectId = item.CourseSubjectId;
                    resultHeader.Add(itemHeader);
                }
            }
            return resultHeader;
        }


        public (TranscriptBySubject data, string message) GetTranscriptByClassAndStudent(string classId, string studentId, string semestersId, string search)
        {
            TranscriptBySubject transcriptBySubject = new TranscriptBySubject();

            Class classObject = _repositoryMariaWrapper.Classes.FirstOrDefault(x => x.ClassId == classId);
            if (classObject is null) return (null, "ClassNotFound");

            transcriptBySubject.ClassObject = classObject;

            Student student = _repositoryMariaWrapper.Students.FirstOrDefault(x => x.StudentId == studentId);
            if (student is null) return (null, "StudentNotFound");
            transcriptBySubject.Student = student;

            var perpareScore = _repositoryMongoWrapper.Scores.FindByCondition(x => x.ClassId == classId && x.SemestersId == semestersId && x.StudentId == studentId).ToList();
            

            var classSchedules = _repositoryMariaWrapper.ClassSchedules.FindByCondition(x => x.ClassId == classId).GroupBy(x => x.SubjectId).Select(x => x.Key).ToList();
            var scoresSubject = perpareScore.GroupBy(x => new { SubjectId = x.SubjectId }).Select(x => new { SubjectId = x.Key.SubjectId, Scores = x.ToList() }).ToList();
            var scoresSubjectType = perpareScore.GroupBy(x => new { SubjectId = x.SubjectId, ScoreTypeId = x.ScoreTypeId }).Select(x => new { SubjectId = x.Key.SubjectId, ScoreTypeId = x.Key.ScoreTypeId, Scores = x.FirstOrDefault() }).ToList();

            foreach (var subjectId in classSchedules)
            {
                Subject tmpSubject = _repositoryMariaWrapper.Subjects.FindByCondition(x => x.SubjectId == subjectId).FirstOrDefault();
                ItemTranscriptBySubject itemTranscriptBySubject = new ItemTranscriptBySubject
                {
                    SubjectId = subjectId,
                    SubjectName = tmpSubject?.SubjectName,
                    SubjectCode = tmpSubject?.SubjectCode
                };

                // Mon hoc chot chua
                ReportClosing reportClosing = _repositoryMariaWrapper.ReportClosings.FirstOrDefault(x => x.ClassId == classObject.ClassId && x.SubjectId == subjectId && x.SemesterId == semestersId);
                if(reportClosing != null)
                {
                    
                    var tmpScore = scoresSubject.FirstOrDefault(x => x.SubjectId == subjectId);
                    if(tmpScore != null)
                    {
                        foreach (var item in tmpScore.Scores)
                        {

                            var tmpHeader = transcriptBySubject.Headers.FirstOrDefault(x => x.ScoreTypeId == item.ScoreTypeId);
                            if(tmpHeader is null)
                            {
                                ScoreType scoreType = _repositoryMariaWrapper.ScoreTypes.FindByCondition(x => x.ScoreTypeId == item.ScoreTypeId).FirstOrDefault();
                                ItemHeaderBySubject newHeader = new ItemHeaderBySubject
                                {
                                    ScoreTypeId = item.ScoreTypeId,
                                };
                                if(scoreType != null)
                                {
                                    newHeader.ScoreTypeName = scoreType.ScoreTypeName;
                                    newHeader.ScoreTypeMultiplier = scoreType.ScoreTypeMultiplier;
                                }
                                transcriptBySubject.Headers.Add(newHeader);
                            }
                            ItemChildScoreBySubject itemChildScoreBySubject = new ItemChildScoreBySubject
                            {
                                ScoreTypeId = item.ScoreTypeId,
                                ScoreValue = item.ScoreSubject.ToArray(),
                                CheckRequired = item.CheckRequired,
                                ScoreTypeNumber = item.ColumnNumber,
                                //MediumValue = AverageScore(item.ScoreSubject.ToArray(), item.CheckRequired),
                                ScoreTypeMultiplier = item.Multiplier
                            };
                            itemTranscriptBySubject.Scores.Add(itemChildScoreBySubject);
                        }
                    }
                   
                }
                else
                {
                    
                    List<CourseSubject> courseSubjects = _repositoryMariaWrapper.CourseSubjects.FindByCondition(x => x.SubjectId == subjectId).Include(x=> x.ScoreType).ToList();
                    foreach (var typeScore in courseSubjects)
                    {
                        var tmpScore = scoresSubjectType.Where(x => x.ScoreTypeId == typeScore.ScoreTypeId && x.SubjectId == subjectId).FirstOrDefault();
                        if(tmpScore is null)
                        {

                            var tmpHeader = transcriptBySubject.Headers.FirstOrDefault(x => x.ScoreTypeId == typeScore.ScoreTypeId);
                            if (tmpHeader is null)
                            {
                                ScoreType scoreType = _repositoryMariaWrapper.ScoreTypes.FindByCondition(x => x.ScoreTypeId == typeScore.ScoreTypeId).FirstOrDefault();
                                ItemHeaderBySubject newHeader = new ItemHeaderBySubject
                                {
                                    ScoreTypeId = typeScore.ScoreTypeId,
                                };
                                if (scoreType != null)
                                {
                                    newHeader.ScoreTypeName = scoreType.ScoreTypeName;
                                    newHeader.ScoreTypeMultiplier = scoreType.ScoreTypeMultiplier;
                                }
                                transcriptBySubject.Headers.Add(newHeader);
                            }
                            ItemChildScoreBySubject itemChildScoreBySubject = new ItemChildScoreBySubject
                            {
                                ScoreTypeId = typeScore.ScoreTypeId,
                                ScoreValue = GeneratorDoubleNull(typeScore.ColumnNumber),
                                CheckRequired = typeScore.CheckRequired,
                                ScoreTypeNumber = typeScore.ColumnNumber,
                                //MediumValue = null,
                                ScoreTypeMultiplier = typeScore.ScoreType.ScoreTypeMultiplier
                            };
                            itemTranscriptBySubject.Scores.Add(itemChildScoreBySubject);
                        }
                        else
                        {
                            // get diem ra 


                            var tmpHeader = transcriptBySubject.Headers.FirstOrDefault(x => x.ScoreTypeId == typeScore.ScoreTypeId);
                            if (tmpHeader is null)
                            {
                                ScoreType scoreType = _repositoryMariaWrapper.ScoreTypes.FindByCondition(x => x.ScoreTypeId == typeScore.ScoreTypeId).FirstOrDefault();
                                ItemHeaderBySubject newHeader = new ItemHeaderBySubject
                                {
                                    ScoreTypeId = typeScore.ScoreTypeId,
                                };
                                if (scoreType != null)
                                {
                                    newHeader.ScoreTypeName = scoreType.ScoreTypeName;
                                    newHeader.ScoreTypeMultiplier = scoreType.ScoreTypeMultiplier;
                                }
                                transcriptBySubject.Headers.Add(newHeader);
                            }
                            ItemChildScoreBySubject itemChildScoreBySubject = new ItemChildScoreBySubject
                            {
                                ScoreTypeId = typeScore.ScoreTypeId,
                                ScoreValue = tmpScore.Scores.ScoreSubject.ToArray(),
                                CheckRequired = typeScore.CheckRequired,
                                ScoreTypeNumber = typeScore.ColumnNumber,
                                //MediumValue = AverageScore(tmpScore.Scores.ScoreSubject.ToArray(), typeScore.CheckRequired),
                                ScoreTypeMultiplier = typeScore.ScoreType.ScoreTypeMultiplier
                            };
                            itemTranscriptBySubject.Scores.Add(itemChildScoreBySubject);
                        }
                    }
                }


                (itemTranscriptBySubject.MediumScore, itemTranscriptBySubject.Note) = fn_AverageScore(itemTranscriptBySubject.Scores);
                transcriptBySubject.Subjects.Add(itemTranscriptBySubject);

                
            }
            

            transcriptBySubject.Subjects = transcriptBySubject.Subjects.Where(x => String.IsNullOrEmpty(search) || x.SubjectName.Contains(search)).ToList();

            return (transcriptBySubject, "GetTranscriptSuccess");


        }


    }

    public class TranscriptByClass
    {
        public List<ItemHeader> Headers { get; set; }
        public string ClassName { get; set; }
        public bool Closed { get; set; }
        public List<ItemTranscriptByClass> Data { get; set; } = new List<ItemTranscriptByClass>();

    }

    public class ItemHeader
    {
        public int ScoreTypeNumber { set; get; }
        public int CheckRequired { set; get; }
        public double ScoreTypeMultiplier { get; set; }
        public string ScoreTypeName { get; set; }
        public string ScoreTypeId { get; set; }
        public string CourseSubjectId { set; get; }
    }

    public class ItemTranscriptByClass
    {
        public string StudentId { get; set; }
        public string StudentCode { get; set; }
        public string StudentLastName { get; set; }
        public string StudentFistName { get; set; }
        public string StudentName { get; set; }
        public GENDER StudentGender { get; set; }
        public List<ItemChildScore> Scores { get; set; } = new List<ItemChildScore>();
        public double? MediumScore { get; set; }
        public string Note { get; set; }

    }
    public class ItemChildScore
    {
        public string ScoreTypeId { get; set; }
        public double?[] ScoreValue { get; set; }
        //public double? MediumValue { get; set; } = null;
        public double ScoreTypeMultiplier { set; get; }
        public int ScoreTypeNumber { set; get; }
        public int CheckRequired { set; get; }
        public string CourseSubjectId { set; get; }
    }

  


    public class TranscriptBySubject
    {
        public Class ClassObject { get; set; }
        public Student Student { get; set; }
        public List<ItemHeaderBySubject> Headers { get; set; } = new List<ItemHeaderBySubject>();

        public List<ItemTranscriptBySubject> Subjects { get; set; } = new List<ItemTranscriptBySubject>();
    }

    public class ItemHeaderBySubject
    {
        public int ScoreTypeNumber { set; get; }
        public int CheckRequired { set; get; }
        public double ScoreTypeMultiplier { get; set; }
        public string ScoreTypeName { get; set; }
        public string ScoreTypeId { get; set; }
        public string CourseSubjectId { set; get; }
    }


    public class ItemTranscriptBySubject
    {
        public string SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public List<ItemChildScoreBySubject> Scores { get; set; } = new List<ItemChildScoreBySubject>();
        public double? MediumScore { get; set; }
        public string Note { get; set; }

    }

    public class ItemChildScoreBySubject
    {
        public string ScoreTypeId { get; set; }
        public double?[] ScoreValue { get; set; }
        //public double? MediumValue { get; set; } = null;
        public double ScoreTypeMultiplier { set; get; }
        public int ScoreTypeNumber { set; get; }
        public int CheckRequired { set; get; }
        public string CourseSubjectId { set; get; }
    }






}
