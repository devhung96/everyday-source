using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Project.App.Databases;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Accounts.Entities;
using Project.Modules.Classes.Entities;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.Courses.Entities;
using Project.Modules.Receipts.Entities;
using Project.Modules.Receipts.Requests;
using Project.Modules.Receipts.Services;
using Project.Modules.Students.Entities;
using Project.Modules.Students.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Project.Modules.Accounts.Entities.Account;

namespace Project.Modules.Students.Services
{
    public interface IStudentService
    {
        long GetNumberStudent();
        Task<(Student student, string message)> Store(AddStudent request);
        (Student student, string message) Detail(string id);
        (Student student, string message) Delete(string id);
        Task<(Student student, string message)> Update(UpdateStudentRequest request, string studentId);
        Task<(RegistrationStudy study, string message)> RegistrationAsync(AddRegistration registration);
        Task<(RegistrationStudy study, string message)> RegistrationStudentAsync(AddRegistration registration);
        ResponseTable ShowStudent(RequestTable request);
        ResponseTable StudentNotYetTuition(RequestTable request);
        //ResponseTable ClassStudentNotYetTuition (RequestTable request, string studentId);
        ResponseTable ShowClassOfStudent(RequestTable request, string studentId);
        (ResponseTable response, string message) GetStudentsInClass(string classId, RequestTable request);
        (RegistrationStudy study, string message) DeleteStudentFromClass(DeleteStudentFromClassRequest request);

        (RegistrationStudy data, string message) RegistrationAsync2(Registration2Request request, string accountId);
        (object data, string message) CollectedTuition(CollectedTuitionRequest valueInput, string accountId);
    }
    public class StudentService : IStudentService
    {
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        private readonly MariaDBContext mariaDBContext;
        private readonly IRepositoryMariaWrapper repository;
        private readonly TokenHelper tokenHelper;
        private readonly IReceiptService receiptService;
        private readonly string pathForder = "Student/" + DateTime.Now.ToString("yyyyMMdd");
        public StudentService(IRepositoryMariaWrapper repository, IMapper mapper, MariaDBContext mariaDBContext, IConfiguration _configuration, IReceiptService _receiptService)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.mariaDBContext = mariaDBContext;
            this.configuration = _configuration;
            tokenHelper = new TokenHelper(this.configuration);
            receiptService = _receiptService;
        }

        public (Student student, string message) Delete(string id)
        {
            (Student student, string message) = Detail(id);
            if (student is null)
            {
                return (student, message);
            }
            if (!String.IsNullOrEmpty(student.StudentImage))
            {
                _ = GeneralHelper.DeleteFile(student.StudentImage);
            }

            List<RegistrationStudy> registrationStudies = repository.RegistrationStudies
                                                                    .FindByCondition(m => m.StudentId.Equals(student.StudentId))
                                                                    .ToList();

            repository.RegistrationStudies.RemoveRangeMaria(registrationStudies);
            repository.SaveChanges();

            repository.Students.RemoveMaria(student);
            repository.SaveChanges();

            List<AccountPermission> accountPermissions = repository.AccountPermissions
                                                                    .FindByCondition(m => m.AccountId.Equals(student.AccountId))
                                                                    .ToList();
            repository.AccountPermissions.RemoveRangeMaria(accountPermissions);
            repository.SaveChanges();

            Account account = repository.Accounts.FirstOrDefault(m => m.AccountId.Equals(student.AccountId));
            repository.Accounts.RemoveMaria(account);
            repository.SaveChanges();

            return (student, "DeleleSuccess");
        }

        public (RegistrationStudy study, string message) DeleteStudentFromClass(DeleteStudentFromClassRequest request)
        {
            RegistrationStudy registrationStudy = repository.RegistrationStudies.FirstOrDefault(m => m.StudentId.Equals(request.StudentId) && m.ClassId.Equals(request.ClassId));
            repository.RegistrationStudies.RemoveMaria(registrationStudy);
            repository.SaveChanges();
            return (registrationStudy, "DeleteSuccess");
        }

        public (Student student, string message) Detail(string id)
        {
            Student student = repository.Students.GetById(id);
            if (student is null)
            {
                return (null, "StudentNotExist");
            }
            var registrationStudies = repository.RegistrationStudies
                                                .FindByCondition(m => m.StudentId.Equals(student.StudentId))
                                                .ToList();
            if (registrationStudies.Count > 0)
            {
                List<string> classIds = registrationStudies.Select(m => m.ClassId).ToList();
                var classes = repository.Classes.FindByCondition(m => classIds.Contains(m.ClassId)).ToList();
                student.Classes = classes.Count > 0 ? classes.Select(m => new ClassResponse(m)).ToList() : null;
                foreach (var item in student.Classes)
                {
                    item.CourseName = repository.Courses.GetById(item.CourseId).CourseName;
                }
            }
            return (student, "ShowSuccess");
        }

        public long GetNumberStudent()
        {
            return repository.Students.FindAll().Count();
        }

        public (ResponseTable response, string message) GetStudentsInClass(string classId, RequestTable request)
        {
            var @class = repository.Classes.GetById(classId);
            if (@class is null)
                return (null, "ClassNotExist");
            List<StudyResponse> studyResponses = repository.RegistrationStudies
                                                .FindByCondition(m => m.ClassId.Equals(classId))
                                                .Include(m => m.Class)
                                                .Include(m => m.Student)
                                                .Select(m => new StudyResponse(m))
                                                .ToList();


            studyResponses = studyResponses.Where(m => String.IsNullOrEmpty(request.Search) || (
                                                     (!String.IsNullOrEmpty(m.StudentCode) && m.StudentCode.Contains(request.Search.ToUpper()))
                                                  || (m.StudentFirstName  + " " + m.StudentLastName).ToUpper().Contains(request.Search.ToUpper())
                                                  || ( !String.IsNullOrEmpty(m.StudentPhone)  && m.StudentPhone.Contains(request.Search))
                                                  || (!String.IsNullOrEmpty(m.StudentEmail) && m.StudentEmail.ToUpper().Contains(request.Search.ToUpper()))
                                                  )).ToList();
            ResponseTable response = new ResponseTable
            {
                Data = (request.Page == 0) ? studyResponses : studyResponses
                                                            .Skip((request.Page - 1) * request.Limit)
                                                            .Take(request.Limit)
                                                            .ToList(),
                Info = new Info
                {
                    Limit = (request.Page == 0) ? studyResponses.Count : studyResponses
                                                                        .Skip((request.Page - 1) * request.Limit)
                                                                        .Take(request.Limit)
                                                                        .Count(),
                    Page = request.Page,
                    TotalRecord = studyResponses.Count,
                }
            };

            return (response, "Success");

        }

        public async Task<(RegistrationStudy study, string message)> RegistrationAsync(AddRegistration request)
        {
            Class @class = repository.Classes.GetById(request.ClassId);
            if (@class is null)
            {
                return (null, "ClassNotExist");
            }

            int so_hv_trong_lop = repository.RegistrationStudies
                                            .FindByCondition(m => m.ClassId.Equals(@class.ClassId))
                                            .Count();

            if (so_hv_trong_lop >= @class.ClassQuantityStudent)
            {
                return (null, "ClassHasEnoughStudents");
            }
            if (@class.Admission == Class.STATUS_OPEN.CLOSE)
            {
                return (null, "ClassHasEndedEnrollment");
            }
            Student student;
            if (!String.IsNullOrEmpty(request.StudentId))
            {
                student = repository.Students.GetById(request.StudentId);
                if (student is null)
                {
                    return (null, "StudentNotExist");
                }
                RegistrationStudy isStudent = repository.RegistrationStudies.FirstOrDefault(m => m.ClassId.Equals(@class.ClassId) && m.StudentId.Equals(student.StudentId));
                if (!(isStudent is null))
                {
                    return (null, "StudentAreadlyExistInClass");
                }
                ClassSchedule classSchedule = repository.ClassSchedules.FirstOrDefault(m => m.ClassId.Equals(request.ClassId));
                if (classSchedule is null)
                {
                    return (null, "ClassScheduleIsNull");
                }
                if (IsDuplicateTime(student.StudentId, @class.ClassId))
                {
                    return (null, "DuplicateClassSchedule");
                }
            }
            else
            {
                AddStudent requestStudent = new AddStudent(request);
                (Student studentNew, string message) = await Store(requestStudent);
                if (studentNew is null)
                    return (null, message);
                student = studentNew;
            }

            RegistrationStudy registration = new RegistrationStudy(student.StudentId, @class.ClassId);
            repository.RegistrationStudies.Add(registration);
            repository.SaveChanges();
            return (registration, "RegistrationStudySuccess");
        }

        public (RegistrationStudy data, string message) RegistrationAsync2(Registration2Request request, string accountId)
        {
            Class @class = repository.Classes.FirstOrDefault(x => x.ClassId == request.ClassId);
            if (@class is null)
            {
                return (null, "ClassNotExist");
            }
            Account account = repository.Accounts.FirstOrDefault(x => x.AccountId == accountId && x.AccountType == TYPE_ACCOUNT.STUDENT);
            if (account is null)
            {
                return (null, "AccountNotExist");
            }
            Student student = repository.Students.FirstOrDefault(x => x.StudentCode == account.AccountCode);
            if (student is null)
            {
                return (null, "StudentNotExist");
            }

            int so_hv_trong_lop = repository.RegistrationStudies
                                            .FindByCondition(m => m.ClassId.Equals(@class.ClassId))
                                            .Count();

            if (so_hv_trong_lop >= @class.ClassQuantityStudent)
            {
                return (null, "ClassHasEnoughStudents");
            }
            if (@class.Admission == Class.STATUS_OPEN.CLOSE)
            {
                return (null, "ClassHasEndedEnrollment");
            }
            RegistrationStudy isStudent = repository.RegistrationStudies.FirstOrDefault(m => m.ClassId.Equals(@class.ClassId) && m.StudentId.Equals(student.StudentId));
            if (!(isStudent is null))
            {
                return (null, "StudentAreadlyExistInClass");
            }
            ClassSchedule classSchedule = repository.ClassSchedules.FirstOrDefault(m => m.ClassId.Equals(request.ClassId));
            if (classSchedule is null)
            {
                return (null, "ClassScheduleIsNull");
            }
            if (IsDuplicateTime(student.StudentId, @class.ClassId))
            {
                return (null, "DuplicateClassSchedule");
            }
            RegistrationStudy registration = new RegistrationStudy(student.StudentId, @class.ClassId);
            repository.RegistrationStudies.Add(registration);
            repository.SaveChanges();
            return (registration, "RegistrationStudySuccess");
        }

        public ResponseTable ShowClassOfStudent(RequestTable request, string studentId)
        {
            Student IsStudent = repository.Students.GetById(studentId);
            if (IsStudent is null)
                return null;
            List<RegistrationStudy> registrationStudies = repository.RegistrationStudies
                                               .FindByCondition(m => m.StudentId.Equals(studentId))
                                               .ToList();
            List<string> classIds = registrationStudies.Select(m => m.ClassId).ToList();

            List<ClassResponse> classes = repository.Classes.FindByCondition(m => classIds.Contains(m.ClassId))
                                                            .Select(m => new ClassResponse(m))
                                                            .ToList();
            classes = classes.Where(m => String.IsNullOrEmpty(request.Search)
                                        || (m.ClassName.ToLower().Contains(request.Search.ToLower()) || m.ClassCode.ToLower().Contains(request.Search.ToLower()))
                                    ).ToList();

            foreach (var item in classes)
            {
                item.CourseName = repository.Courses.GetById(item.CourseId).CourseName;
                item.Surcharges = repository.Surcharges
                                                .FindByCondition(m => m.ClassId.Equals(item.ClassId))
                                                .ToList();
                item.SumSurcharges = repository.Surcharges
                                                .FindByCondition(m => m.ClassId.Equals(item.ClassId))
                                                .Sum(m => m.SurchargeAmount);

                item.StatusTuition = registrationStudies.FirstOrDefault(m => m.ClassId.Equals(item.ClassId) && m.StudentId.Equals(studentId))
                                                        .RegistrationTuition;
            }

            ResponseTable response = new ResponseTable
            {
                Data = request.Page == 0 ? classes : classes.Skip((request.Page - 1) * request.Limit).Take(request.Limit),
                Info = new Info
                {
                    Limit = request.Page == 0 ? classes.Count : classes.Skip((request.Page - 1) * request.Limit).Take(request.Limit).Count(),
                    Page = request.Page,
                    TotalRecord = classes.Count,
                }
            };
            return response;
        }

        public ResponseTable ShowStudent(RequestTable request)
        {
            List<Student> students = repository.Students.FindAll().OrderByDescending(m => m.CreateAt).ToList();
            students = students.Where(m => String.IsNullOrEmpty(request.Search) || (
                                                        m.StudentCode.ToLower().Contains(request.Search.ToLower())
                                                    || (!string.IsNullOrEmpty(m.StudentEmail) && m.StudentEmail.ToLower().Contains(request.Search.ToLower()))
                                                    || (!string.IsNullOrEmpty(m.StudentPhone) && m.StudentPhone!.ToLower().Contains(request.Search.ToLower()))
                                                    )).ToList();
            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                students = students.AsQueryable().OrderBy(request.SortField.OrderValue(request.SortOrder)).ToList();
            }
            ResponseTable response = new ResponseTable
            {
                Data = (request.Page == 0) ? students : students.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList(),
                Info = new Info
                {
                    Limit = (request.Page == 0) ? students.Count : students.Skip((request.Page - 1) * request.Limit).Take(request.Limit).Count(),
                    Page = request.Page,
                    TotalRecord = students.Count,
                }
            };

            return response;
        }

        //public ResponseTable ShowTable(RequestTable request)
        //{

        //    List<StudyResponse> studyResponses = repository.RegistrationStudies.FindAll().Include(m => m.Student).Include(m => m.Class)
        //                                                            .ThenInclude(x=>x.Course).Select(m => new StudyResponse(m)).ToList();



        //    studyResponses = studyResponses.Where(m => String.IsNullOrEmpty(request.Search) || (
        //                                                m.StudentCode.Contains(request.Search)
        //                                            || (m.StudentLastName + " " + m.).Contains(request.Search)
        //                                            || m.ClassName.Contains(request.Search)
        //                                            || m.CourseName.Contains(request.Search)
        //                                            )).ToList();
        //    ResponseTable response = new ResponseTable
        //    {
        //        Data = (request.Page == -1) ? studyResponses : studyResponses.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList(),
        //        Info = new Info
        //        {
        //            Limit = (request.Page == -1) ? studyResponses.Count() : studyResponses.Skip((request.Page - 1) * request.Limit).Take(request.Limit).Count(),
        //            Page = request.Page,
        //            TotalRecord = studyResponses.Count(),
        //        }
        //    };

        //    return response;
        //}

        public async Task<(Student student, string message)> Store(AddStudent request)
        {

            string studentCode = AutomaticStudentCode();
            using IDbContextTransaction transaction = mariaDBContext.Database.BeginTransaction();
            try
            {
                #region Thêm tài khoản đăng nhập và thêm quyền

                string saft = 5.RandomString();
                Account account = new Account
                {
                    AccountCode = studentCode,
                    Password = (request.Password + saft).HashPassword(),
                    Saft = saft,
                    AccountType = Account.TYPE_ACCOUNT.STUDENT,
                    GroupCode = PERMISSION_DEFAULT.Student
                };
                repository.Accounts.Add(account);
                repository.SaveChanges();

                List<string> permissionCodes = repository.GroupPermissions.FindByCondition(m => m.GroupId.Equals(PERMISSION_DEFAULT.Student)).Select(m => m.PermissionCode).ToList();
                foreach (string permissionCode in permissionCodes)
                {
                    AccountPermission accountPermission = new AccountPermission(account.AccountId, permissionCode);
                    repository.AccountPermissions.Add(accountPermission);
                }
                repository.SaveChanges();

                #endregion
                string path = "";
                if (!(request.Image is null))
                {
                    (path, _) = await GeneralHelper.UploadFileProAsync(request.Image, pathForder);
                }
                Student student = new Student(path, account.AccountId, studentCode);
                mapper.Map(request, student);
                repository.Students.Add(student);
                repository.SaveChanges();
                transaction.Commit();
                return (student, "StoreStudentSuccess");
            }
            catch (Exception)
            {
                transaction.Rollback();
                return (null, "StoreStudentFaild");
            }
        }

        public async Task<(Student student, string message)> Update(UpdateStudentRequest request, string studentId)
        {
            Student student = repository.Students.GetById(studentId);
            if (student is null)
            {
                return (null, "StudentNotExist");
            }
            student = mapper.Map(request, student);
            student.StudentAddress = request.StudentAddress;

            string path = "";

            if (!(request.Image is null) && !String.IsNullOrEmpty(student.StudentImage))
            {
                _ = GeneralHelper.DeleteFile(student.StudentImage);
            }

            if (!(request.Image is null))
            {
                (path, _) = await GeneralHelper.UploadFileProAsync(request.Image, pathForder);
                student.StudentImage = path;
            }

            #region Update AccountPermssion
            #endregion

            repository.Students.UpdateMaria(student);
            repository.SaveChanges();
            return (student, "UpdateSuccess");
        }

        bool IsDuplicateTime(string studentId, string classId) //fasle không trùng
        {
            List<string> classIds = repository.RegistrationStudies.FindByCondition(m => m.StudentId.Equals(studentId)).Select(m => m.ClassId).ToList();
            List<ClassSchedule> schedules = repository.ClassSchedules.FindByCondition(m => classIds.Contains(m.ClassId)).ToList();
            ClassSchedule plus = repository.ClassSchedules.FirstOrDefault(m => m.ClassId.Equals(classId));
            List<string> dayClass = plus.DayOfWeek.Split(",").ToList();
            foreach (ClassSchedule last in schedules)
            {
                foreach (string date in dayClass)
                {
                    if (last.DayOfWeek.Contains(date))
                    {
                        if ((last.TimeStart <= plus.TimeStart) && (plus.TimeStart <= last.TimeEnd)) return true;

                        if (last.TimeStart > plus.TimeStart && last.TimeStart <= plus.TimeEnd) return true;

                    }
                }
            }
            return false;
        }

        string BuildToken(string accountId)
        {
            List<string> permissionCodes = repository.AccountPermissions
                                                     .FindByCondition(p => p.AccountId.Equals(accountId))
                                                     .Select(p => p.PermissionCode)
                                                     .ToList();

            Account account = repository.Accounts.GetById(accountId);
            account.Token = tokenHelper.GenerateToken(account, permissionCodes);
            account.LoginAt = DateTime.Now;
            repository.Accounts.UpdateMaria(account);
            repository.SaveChanges();
            return account.Token;
        }

        public async Task<(RegistrationStudy study, string message)> RegistrationStudentAsync(AddRegistration request)
        {
            Class @class = repository.Classes.GetById(request.ClassId);
            if (@class is null)
            {
                return (null, "ClassNotExist");
            }

            int so_hv_trong_lop = repository.RegistrationStudies
                                            .FindByCondition(m => m.ClassId.Equals(@class.ClassId))
                                            .Count();

            if (so_hv_trong_lop >= @class.ClassQuantityStudent)
            {
                return (null, "ClassHasEnoughStudents");
            }
            if (@class.Admission == Class.STATUS_OPEN.CLOSE)
            {
                return (null, "ClassHasEndedEnrollment");
            }

            AddStudent requestStudent = new AddStudent(request);
            (Student studentNew, string message) = await Store(requestStudent);
            if (studentNew is null)
                return (null, message);
            RegistrationStudy registration = new RegistrationStudy(studentNew.StudentId, @class.ClassId);
            repository.RegistrationStudies.Add(registration);
            repository.SaveChanges();
            registration.Token = BuildToken(studentNew.AccountId);
            return (registration, "RegistrationStudySuccess");
        }

        public (object data, string message) CollectedTuition(CollectedTuitionRequest valueInput, string accountId)
        {
            RegistrationStudy registrationStudy = repository.RegistrationStudies
                .FindByCondition(x => x.StudentId.Equals(valueInput.StudentId) && x.ClassId.Equals(valueInput.ClassId) && x.RegistrationTuition == EnumTuition.NoTuition)
                .Include(x => x.Student)
                .Include(x => x.Class)
                .FirstOrDefault();
            if (registrationStudy is null)
            {
                return (null, "registrationStudyNoTuition");
            }
            ReceiptRequest receiptRequest = new ReceiptRequest();
            mapper.Map(valueInput, receiptRequest);
            receiptRequest.AccountId = accountId;
            (Receipt receipt, string messageReceipt) = receiptService.Store(receiptRequest);

            if (receipt is null)
            {
                return (null, messageReceipt);
            }
            Course course = repository.Courses.FirstOrDefault(x => x.CourseId.Equals(registrationStudy.Class.CourseID));
            registrationStudy.RegistrationTuition = EnumTuition.CollectedTuition;
            registrationStudy.RegistrationTuitionDate = DateTime.Now;
            repository.RegistrationStudies.UpdateMaria(registrationStudy);
            repository.SaveChanges();
            registrationStudy.Class.Course = course;
            StudyResponse studyResponse = new StudyResponse(registrationStudy);
            List<object> response = new List<object>()
            {
                new
                {
                    studyResponse = new {
                        data = studyResponse,
                        message = "CollectedTuition"
                    },
                    receipt = new
                    {
                        data = receipt,
                        message = messageReceipt
                    }
                }
            };
            return (response, "CollectedTuition");

        }


        public string AutomaticStudentCode()
        {
            string StudentCode = "";
            string time= DateTime.UtcNow.AddHours(7).ToString("yyyyMM");

            int CountStudent = repository.Students
                                         .FindByCondition(
                                                m => m.CreateAt.Value.Year == DateTime.UtcNow.AddHours(7).Year 
                                                && m.CreateAt.Value.Month == DateTime.UtcNow.AddHours(7).Month
                                          ).Count();
            StudentCode = time + "-" + IntToString(CountStudent +1);
            bool check = false;
            while (check==false)
            {
                Student student = repository.Students.FirstOrDefault(m => m.StudentCode.Equals(StudentCode));
                if (student is null)
                {
                    check = true;
                }
                else
                {
                    CountStudent++;
                    StudentCode = time + "-" + IntToString(CountStudent);
                }
            }
            return StudentCode;
        }

        public string IntToString(int i)
        {
            string result = i.ToString();

            while (result.Length < 5)
            {
                result = result.Insert(0, "0");
            }
            return result;
        }

        public ResponseTable StudentNotYetTuition(RequestTable request)
        {
            List<string> studentIds = repository.RegistrationStudies
                                                .FindByCondition(m => m.RegistrationTuition == EnumTuition.NoTuition)
                                                .Select(m => m.StudentId)
                                                .ToList();

            studentIds = studentIds.Distinct().ToList();

            List<Student> students = repository.Students
                                               .FindByCondition(m => studentIds.Contains(m.StudentId))
                                               .OrderByDescending(m => m.CreateAt)
                                               .ToList();

            students = students.Where(m => String.IsNullOrEmpty(request.Search) || (
                                                        m.StudentCode.ToLower().Contains(request.Search.ToLower())
                                                    || (!string.IsNullOrEmpty(m.StudentEmail) && m.StudentEmail.ToLower().Contains(request.Search.ToLower()))
                                                    || (!string.IsNullOrEmpty(m.StudentPhone) && m.StudentPhone!.ToLower().Contains(request.Search.ToLower()))
                                                    )).ToList();
            if (!string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder))
            {
                students = students.AsQueryable().OrderBy(request.SortField.OrderValue(request.SortOrder)).ToList();
            }

            ResponseTable response = new ResponseTable
            {
                Data = (request.Page == 0) ? students : students.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList(),
                Info = new Info
                {
                    Limit = (request.Page == 0) ? students.Count : students.Skip((request.Page - 1) * request.Limit).Take(request.Limit).Count(),
                    Page = request.Page,
                    TotalRecord = students.Count,
                }
            };

            return response;
        }

        //public ResponseTable ClassStudentNotYetTuition(RequestTable request, string studentId)
        //{
        //    List<string> classIds = repository.RegistrationStudies
        //                                        .FindByCondition(m => m.StudentId.Equals(studentId) && m.RegistrationTuition == EnumTuition.NoTuition)
        //                                        .Select(m => m.ClassId)
        //                                        .ToList();

        //    List<Class> classes = repository.Classes.FindByCondition(m => classIds.Contains(m.ClassId))
        //                                            .Where(m=>string.IsNullOrEmpty(request.Search)
        //                                            || (! string.IsNullOrEmpty(m.ClassCode) && m.ClassCode.ToLower().Contains(request.Search.ToLower()))
        //                                            || (!string.IsNullOrEmpty(m.ClassName) && m.ClassName.ToLower().Contains(request.Search.ToLower())))
        //                                            .OrderByDescending(m => m.ClassCreatedAt)
        //                                            .ToList();

        //    foreach ( Class @class in classes)
        //    {
        //        @class.SumSurcharges = repository.Surcharges
        //                                         .FindByCondition(m => m.ClassId.Equals(@class.ClassId))
        //                                         .Sum(m => m.SurchargeAmount);
        //    }

        //    ResponseTable response = new ResponseTable
        //    {
        //        Data = (request.Page == 0) ? classes : classes.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList(),
        //        Info = new Info
        //        {
        //            Limit = (request.Page == 0) ? classes.Count : classes.Skip((request.Page - 1) * request.Limit).Take(request.Limit).Count(),
        //            Page = request.Page,
        //            TotalRecord = classes.Count,
        //        }
        //    };

        //    return response;
        //}
    }
}
