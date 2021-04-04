using AutoMapper;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Project.App.Databases;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Accounts.Entities;
using Project.Modules.Classes.Entities;
using Project.Modules.Classes.Requests;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.Courses.Entities;
using Project.Modules.Lecturers.Entities;
using Project.Modules.Receipts.Entities;
using Project.Modules.SchoolYears.Entities;
using Project.Modules.Students.Entities;
using Project.Modules.SubjectGroups.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Project.Modules.Accounts.Entities.Account;
using static Project.Modules.Classes.Entities.Class;

namespace Project.Modules.Classes.Services
{
    public interface IClassService
    {
        Class Store(StoreClassRequest request, string host);
        Class Update(UpdateClassRequest request, string host);
        (object data, string message) Delete(string classID);
        object ShowClasses(ListClassesRequest request, string host);
        object ShowClassOpen(ListClassesRequest request, string host);
        (object data, string message) ShowClassesForRegister(GetClassRegisterStudent request, string host);
        (object data, string message) GetClassByStudent(ListClassesRequest request, string accountID, string host);
        (object data, string message) EnrollmentClass(string host);
        (object data, string message) ShowClass(string classID, string host);
        List<Surcharge> ShowAllSurcharge(string classId);
        Surcharge Update(UpdateSurchargeRequest request);
        (object data, string message) DeleteSurcharge(string surchargeId);
    }
    public class ClassService : IClassService
    {
        private readonly IRepositoryMariaWrapper repositoryMariaWrapper;
        private readonly IMapper mapper;
        private readonly MongoDBContext mongoDBContext;
        private readonly IRepositoryMongoWrapper repositoryMongoWrapper;
        private readonly IConfiguration configuration;
        private readonly string pathForder = "Class/" + DateTime.UtcNow.ToString("yyyyMMdd");
        public ClassService(IRepositoryMariaWrapper repositoryWrapper, IMapper mapper, MongoDBContext mongoDBContext, IRepositoryMongoWrapper repositoryMongoWrapper, IConfiguration configuration)
        {
            this.repositoryMariaWrapper = repositoryWrapper;
            this.mapper = mapper;
            this.mongoDBContext = mongoDBContext;
            this.repositoryMongoWrapper = repositoryMongoWrapper;
            this.configuration = configuration;
        }
        public Class Store(StoreClassRequest request, string host)
        {
            string path = "";
            if (!(request.ClassImage is null))
            {
                (path, _) = GeneralHelper.UploadFileProAsync(request.ClassImage, pathForder).Result;
            }
            Class @class = new Class()
            {
                ClassCode = request.ClassCode,
                ClassName = request.ClassName,
                ClassQuantityStudent = request.ClassQuantityStudent.Value,
                CourseID = request.CourseID,
                SchoolYearID = request.SchoolYearID,
                ClassDescription = request.ClassDescription,
                ClassAmount = request.ClassAmount.Value,
                ClassAmountDescription = request.ClassAmountDescription,
                ClassImage = path
            };
            repositoryMariaWrapper.Classes.Add(@class);
            if (request.SurchargeData != null && request.SurchargeData.Count > 0)
            {
                foreach (var item in request.SurchargeData)
                {
                    Surcharge surcharge = new Surcharge()
                    {
                        ClassId = @class.ClassId,
                        SurchargeName = item.SurchargeName,
                        SurchargeAmount = item.SurchargeAmount.Value
                    };
                    repositoryMariaWrapper.Surcharges.Add(surcharge);
                }
            }
            repositoryMariaWrapper.SaveChanges();
            @class.ClassImage = !String.IsNullOrEmpty(@class.ClassImage) ? host + @class.ClassImage : @class.ClassImage;
            @class.Course = repositoryMariaWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
            @class.SchoolYear = repositoryMariaWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
            @class.Surcharges = repositoryMariaWrapper.Surcharges.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).ToList();
            return @class;
        }
        public Class Update(UpdateClassRequest request, string host)
        {
            Class @class = repositoryMariaWrapper.Classes.FirstOrDefault(x => x.ClassId.Equals(request.ClassID));
            string path = "";
            if (!(request.Image is null))
            {
                (path, _) = GeneralHelper.UploadFileProAsync(request.Image, pathForder).Result;
                @class.ClassImage = path;
            }
            request.MergeData(@class);
            @class.ClassDescription = request.ClassDescription;
            if (request.SurchargeData != null && request.SurchargeData.Count > 0)
            {
                foreach (var item in request.SurchargeData)
                {
                    Surcharge surcharge = repositoryMariaWrapper.Surcharges.FirstOrDefault(x => x.SurchargeId.Equals(item.SurchargeId));
                    if (surcharge is null)
                    {
                        surcharge = new Surcharge()
                        {
                            ClassId = @class.ClassId,
                            SurchargeName = item.SurchargeName,
                            SurchargeAmount = item.SurchargeAmount.Value
                        };
                        repositoryMariaWrapper.Surcharges.Add(surcharge);
                    }
                    else
                    {
                        item.MergeData(surcharge);
                    }
                }
            }
            repositoryMariaWrapper.SaveChanges();
            @class.ClassImage = !String.IsNullOrEmpty(@class.ClassImage) ? host + @class.ClassImage : @class.ClassImage;
            @class.Course = repositoryMariaWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
            @class.SchoolYear = repositoryMariaWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
            @class.QuantityRegisted = repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).Count();
            @class.Surcharges = repositoryMariaWrapper.Surcharges.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).ToList();
            return @class;
        }
        public Surcharge Update(UpdateSurchargeRequest request)
        {
            Surcharge surcharge = repositoryMariaWrapper.Surcharges.FirstOrDefault(x => x.SurchargeId.Equals(request.SurchargeId));
            request.MergeData(surcharge);
            return (surcharge);
        }
        public (object data, string message) DeleteSurcharge(string surchargeId)
        {
            Surcharge surcharge = repositoryMariaWrapper.Surcharges.FirstOrDefault(x => x.SurchargeId.Equals(surchargeId));
            if (surcharge is null)
            {
                return (null, "SurchargeNotFound");
            }
            Class @class = repositoryMariaWrapper.Classes.FirstOrDefault(x => x.ClassId.Equals(surcharge.ClassId));
            if (@class is null || @class.Admission == STATUS_OPEN.CLOSE)
            {
                return (null, "ClassNotFound");
            }
            repositoryMariaWrapper.Surcharges.RemoveMaria(surcharge);
            repositoryMariaWrapper.SaveChanges();
            return ("Success", "Success");
        }
        public List<Surcharge> ShowAllSurcharge(string classId)
        {
            return repositoryMariaWrapper.Surcharges.FindByCondition(x => x.ClassId.Equals(classId)).ToList();
        }
        public (object data, string message) Delete(string classID)
        {
            Class @class = repositoryMariaWrapper.Classes.FirstOrDefault(x => x.ClassId.Equals(classID));
            if (@class is null)
            {
                return (null, "ClassNotFound");
            }

            using (var transaction = repositoryMariaWrapper.BeginTransaction())
            {
                try
                {
                    List<RegistrationStudy> registrationStudies = repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).ToList();
                    if (registrationStudies.Count > 0)
                    {
                        repositoryMariaWrapper.RegistrationStudies.RemoveRangeMaria(registrationStudies);
                    }
                    List<string> scores = repositoryMongoWrapper.Scores.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).Select(x => x.ScoreId).ToList();
                    repositoryMongoWrapper.Scores.RemoveRangeMongo(x => scores.Contains(x.ScoreId));
                    repositoryMariaWrapper.Classes.RemoveMaria(@class);
                    repositoryMariaWrapper.SaveChanges();
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return (null, ex.Message);
                }
            }
            return ("Success", "Success");
        }
        public object ShowClasses(ListClassesRequest request, string host)
        {

            var classes = repositoryMariaWrapper.Classes.FindByCondition(x =>
                (String.IsNullOrEmpty(request.ClassCode) || x.ClassCode.Equals(request.ClassCode, StringComparison.OrdinalIgnoreCase))
                 &&
                 (String.IsNullOrEmpty(request.ClassName) || x.ClassName.Contains(request.ClassName, StringComparison.OrdinalIgnoreCase))
                &&
                 (String.IsNullOrEmpty(request.CourseID) || x.CourseID.Equals(request.CourseID))
                &&
                 (String.IsNullOrEmpty(request.SchoolYearID) || x.SchoolYearID.Equals(request.SchoolYearID))
                && (String.IsNullOrEmpty(request.Search) || x.ClassCode.Equals(request.Search, StringComparison.OrdinalIgnoreCase) || x.ClassName.Contains(request.Search, StringComparison.OrdinalIgnoreCase))
                 ).ToList();
            foreach (Class @class in classes)
            {
                @class.Course = repositoryMariaWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
                @class.SchoolYear = repositoryMariaWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
                @class.QuantityRegisted = repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).Count();
                @class.ClassImage = !String.IsNullOrEmpty(@class.ClassImage) ? host + @class.ClassImage : @class.ClassImage;
                @class.Surcharges = repositoryMariaWrapper.Surcharges.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).ToList();
            }
            ResponseTable responseTable = new ResponseTable();
            if (request.Type == 1)
            {
                var result = classes.AsQueryable().OrderBy(request.SortField.OrderValue(request.SortOrder));
                responseTable = new ResponseTable()
                {
                    Data = result.Skip((request.Page - 1) * request.Limit).Take(request.Limit),
                    Info = new Info()
                    {
                        Page = request.Page,
                        Limit = request.Limit,
                        TotalRecord = result.Count()
                    }
                };
                return responseTable;
            }
            else
            {
                responseTable = new ResponseTable()
                {
                    Data = classes,
                    Info = new Info
                    {
                        Page = 0,
                        Limit = 0,
                        TotalRecord = classes.Count()
                    }
                };
                return responseTable;
            }
        }

        public (object data, string message) ShowClassesForRegister(GetClassRegisterStudent request, string host)
        {
            List<Class> classReponse = new List<Class>();

            if (repositoryMariaWrapper.RegistrationStudies.FirstOrDefault(x => x.StudentId.Equals(request.StudentID)) is null)
            {
                List<Class> classes = repositoryMariaWrapper.Classes.FindByCondition(x => x.Admission == STATUS_OPEN.OPEN
                    && (String.IsNullOrEmpty(request.CourseID) || x.CourseID.Equals(request.CourseID))
                    && (String.IsNullOrEmpty(request.SchoolYearID) || x.SchoolYearID.Equals(request.SchoolYearID))
                    ).ToList();


                foreach (var @class in classes)
                {
                    ClassSchedule classSchedule = repositoryMariaWrapper.ClassSchedules.FirstOrDefault(m => m.ClassId.Equals(@class.ClassId));
                    int slotInClass = repositoryMariaWrapper.RegistrationStudies.FindByCondition(m => m.ClassId.Equals(@class.ClassId)).Count();
                    if (!(classSchedule is null) && (slotInClass < @class.ClassQuantityStudent))
                    {
                        @class.Course = repositoryMariaWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
                        @class.SchoolYear = repositoryMariaWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
                        @class.QuantityRegisted = repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).Count();
                        @class.ClassImage = !String.IsNullOrEmpty(@class.ClassImage) ? host + @class.ClassImage : @class.ClassImage;
                        @class.Surcharges = repositoryMariaWrapper.Surcharges.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).ToList();
                        classReponse.Add(@class);
                    }


                }
                return (classReponse, "Success");
            }
            else
            {
                List<string> classIDs = repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.StudentId.Equals(request.StudentID)).Select(x => x.ClassId).ToList();
                List<Class> classes = repositoryMariaWrapper.Classes.FindByCondition(x => !classIDs.Contains(x.ClassId)
                 && x.Admission == STATUS_OPEN.OPEN
                 && (String.IsNullOrEmpty(request.CourseID) || x.CourseID.Equals(request.CourseID))
                 && (String.IsNullOrEmpty(request.SchoolYearID) || x.SchoolYearID.Equals(request.SchoolYearID))
                 ).ToList();
                foreach (var @class in classes)
                {
                    ClassSchedule classSchedule = repositoryMariaWrapper.ClassSchedules.FirstOrDefault(m => m.ClassId.Equals(@class.ClassId));
                    int slotInClass = repositoryMariaWrapper.RegistrationStudies.FindByCondition(m => m.ClassId.Equals(@class.ClassId)).Count();
                    if (!(classSchedule is null) && (slotInClass < @class.ClassQuantityStudent))
                    {
                        @class.Course = repositoryMariaWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
                        @class.SchoolYear = repositoryMariaWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
                        @class.QuantityRegisted = repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).Count();
                        @class.ClassImage = !String.IsNullOrEmpty(@class.ClassImage) ? host + @class.ClassImage : @class.ClassImage;
                        @class.Surcharges = repositoryMariaWrapper.Surcharges.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).ToList();
                        classReponse.Add(@class);
                    }

                }
                return (classReponse, "Success");
            }
        }

        public object ShowClassOpen(ListClassesRequest request, string host)
        {
            List<Class> results = new List<Class>();
            var classes = repositoryMariaWrapper.Classes.FindByCondition(x => x.Admission == STATUS_OPEN.OPEN &&
               (String.IsNullOrEmpty(request.ClassCode) || x.ClassCode.Equals(request.ClassCode))
                &&
                (String.IsNullOrEmpty(request.ClassName) || x.ClassName.Equals(request.ClassName))
               &&
                (String.IsNullOrEmpty(request.CourseID) || x.CourseID.Equals(request.CourseID))
               &&
                (String.IsNullOrEmpty(request.SchoolYearID) || x.SchoolYearID.Equals(request.SchoolYearID))
               &&
                (String.IsNullOrEmpty(request.Search) || x.ClassCode.Equals(request.Search, StringComparison.OrdinalIgnoreCase) || x.ClassName.Contains(request.Search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            foreach (Class @class in classes)
            {
                ClassSchedule classSchedule = repositoryMariaWrapper.ClassSchedules.FirstOrDefault(m => m.ClassId.Equals(@class.ClassId));
                int slotInClass = repositoryMariaWrapper.RegistrationStudies.FindByCondition(m => m.ClassId.Equals(@class.ClassId)).Count();
                if (!(classSchedule is null) && (slotInClass < @class.ClassQuantityStudent))
                {
                    @class.Course = repositoryMariaWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
                    @class.SchoolYear = repositoryMariaWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
                    @class.QuantityRegisted = repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).Count();
                    @class.ClassImage = !String.IsNullOrEmpty(@class.ClassImage) ? host + @class.ClassImage : @class.ClassImage;
                    @class.Surcharges = repositoryMariaWrapper.Surcharges.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).ToList();
                    results.Add(@class);
                }
            }
            ResponseTable responseTable = new ResponseTable();
            if (request.Type == 1)
            {
                var result = results.AsQueryable().OrderBy(request.SortField.OrderValue(request.SortOrder));
                responseTable = new ResponseTable()
                {
                    Data = result.Skip((request.Page - 1) * request.Limit).Take(request.Limit),
                    Info = new Info()
                    {
                        Page = request.Page,
                        Limit = request.Limit,
                        TotalRecord = result.Count()
                    }
                };
                return responseTable;
            }
            else
            {
                responseTable = new ResponseTable()
                {
                    Data = results,
                    Info = new Info
                    {
                        Page = 0,
                        Limit = 0,
                        TotalRecord = results.Count()
                    }
                };
                return responseTable;
            }
        }

        public (object data, string message) GetClassByStudent(ListClassesRequest request, string accountID, string host)
        {
            Account account = repositoryMariaWrapper.Accounts.FirstOrDefault(x => x.AccountId.Equals(accountID) && x.AccountType == TYPE_ACCOUNT.STUDENT);
            if (account is null)
            {
                return (null, "AccountNotFound");
            }
            Student student = repositoryMariaWrapper.Students.FirstOrDefault(x => x.StudentCode.Equals(account.AccountCode));
            if (student is null)
            {
                return (null, "StudentNotFound");
            }
            List<string> classIDs = repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.StudentId.Equals(student.StudentId)).Select(x => x.ClassId).ToList();
            List<ClassSchedule> classSchedules = repositoryMariaWrapper.ClassSchedules.FindByCondition(x => classIDs.Contains(x.ClassId)).ToList();
            var classes = repositoryMariaWrapper.Classes.FindByCondition(x => classIDs.Contains(x.ClassId)).ToList();
            foreach (Class @class in classes)
            {
                @class.Course = repositoryMariaWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
                @class.SchoolYear = repositoryMariaWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
                @class.QuantityRegisted = repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).Count();
                @class.ClassImage = !String.IsNullOrEmpty(@class.ClassImage) ? host + @class.ClassImage : @class.ClassImage;
                @class.Surcharges = repositoryMariaWrapper.Surcharges.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).ToList();
                @class.ClassSchedule = classSchedules.FirstOrDefault(x => x.ClassId.Equals(@class.ClassId));
                Receipt receipt = repositoryMariaWrapper.Receipts.FirstOrDefault(m => m.StudentId.Equals(student.StudentId) && m.ClassId.Equals(@class.ClassId));
                if (!(receipt is null))
                {
                    @class.RegistrationTuition = EnumTuition.CollectedTuition;
                }

            }
            ResponseTable responseTable = new ResponseTable();
            if (request.Type == 1)
            {
                var result = classes.AsQueryable().OrderBy(request.SortField.OrderValue(request.SortOrder));
                responseTable = new ResponseTable()
                {
                    Data = result.Skip((request.Page - 1) * request.Limit).Take(request.Limit),
                    Info = new Info()
                    {
                        Page = request.Page,
                        Limit = request.Limit,
                        TotalRecord = result.Count()
                    }
                };
                return (responseTable, "Success");
            }
            else
            {
                responseTable = new ResponseTable()
                {
                    Data = classes,
                    Info = new Info
                    {
                        Page = 0,
                        Limit = 0,
                        TotalRecord = classes.Count()
                    }
                };
                return (responseTable, "Success");
            }
        }

        public (object data, string message) EnrollmentClass(string host)
        {
            List<string> classes = repositoryMariaWrapper.Classes.FindByCondition(x => x.Admission == STATUS_OPEN.OPEN).Select(x => x.ClassId).ToList();
            List<ClassSchedule> classSchedules = repositoryMariaWrapper.ClassSchedules.FindByCondition(x => classes.Contains(x.ClassId)).ToList();
            List<ResponseEnrollmentClass> reponses = new List<ResponseEnrollmentClass>();
            foreach (var classSchedule in classSchedules)
            {
                Class @class = repositoryMariaWrapper.Classes.FindByCondition(x => x.ClassId.Equals(classSchedule.ClassId)).AsNoTracking().FirstOrDefault();
                @class.ClassImage = !String.IsNullOrEmpty(@class.ClassImage) ? host + @class.ClassImage : @class.ClassImage;
                Course course = repositoryMariaWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
                SchoolYear schoolYear = repositoryMariaWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
                Lecturer lecturer = repositoryMariaWrapper.Lecturers.FirstOrDefault(x => x.LecturerId.Equals(classSchedule.LecturerId));
                ResponseEnrollmentClass response = new ResponseEnrollmentClass
                {
                    Class = @class,
                    Course = course,
                    SchoolYear = schoolYear,
                    Lecturer = lecturer,
                    DayOfWeek = classSchedule.DayOfWeek,
                    TimeStart = classSchedule.DateStart
                };
                reponses.Add(response);
            }
            return (reponses, "Success");
        }

        public (object data, string message) ShowClass(string classID, string host)
        {
            Class @class = repositoryMariaWrapper.Classes.FirstOrDefault(x => x.ClassId.Equals(classID));
            if (@class is null)
            {
                return (null, "ClassNotFound");
            }
            @class.Course = repositoryMariaWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
            @class.SchoolYear = repositoryMariaWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
            @class.QuantityRegisted = repositoryMariaWrapper.RegistrationStudies.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).Count();
            @class.ClassImage = !String.IsNullOrEmpty(@class.ClassImage) ? host + @class.ClassImage : @class.ClassImage;
            @class.Surcharges = repositoryMariaWrapper.Surcharges.FindByCondition(x => x.ClassId.Equals(@class.ClassId)).ToList();
            return (@class, "Success");
        }

        public class ResponseEnrollmentClass
        {
            public Class Class { get; set; }
            public Course Course { get; set; }
            public SchoolYear SchoolYear { get; set; }
            public Lecturer Lecturer { get; set; }
            public string DayOfWeek { get; set; }
            public DateTime? TimeStart { get; set; }
            public ClassSchedule ClassSchedule { get; set; }
            public string SubjectGroupId { get; set; }
        }
    }
}