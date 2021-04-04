using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.App.Services;
using Project.Modules.Classes.Entities;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.Courses.Entities;
using Project.Modules.Lecturers.Entities;
using Project.Modules.Receipts.Entities;
using Project.Modules.Reports.Requests;
using Project.Modules.SchoolYears.Entities;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.Students.Entities;
using Project.Modules.SubjectGroups.Entities;
using Project.Modules.SubjectGroups.Requests;
using Project.Modules.SubjectGroups.Responses;
using Project.Modules.Subjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static Project.Modules.Classes.Services.ClassService;

namespace Project.Modules.SubjectGroups.Services
{
    public interface ISubjectGroupService : IService<SubjectGroup>
    {
        string GetSubjectGroupName(string subjectGroupId);
        List<SubjectGroup> GetAll();
        ResponseTable ShowAll(SearchSubjectGroupRequest request);
        SubjectGroup Store(StoreSubjectGroupRequest request);
        SubjectGroup Update(UpdateSubjectGroupRequest request, SubjectGroup subjectGroup);
        List<ResponseEnrollmentClass> GetClassWithSubjectGroupId(SubjectGroup subjectGroup);
        List<RegistrationStudy> GetRegister(string accountId);
        List<ResponseEnrollmentClass> GetClassAllClassOpen();
        List<ClassSchedule> GetClassSchedules();
        List<ExportReportMonth> ExportReport(ExportReceipt exportReceipt);
        List<Class> GetAllClassOpen();
    }

    public class SubjectGroupService : ISubjectGroupService
    {
        private readonly IRepositoryMariaWrapper RepositoryWrapper;
        private readonly IMapper Mapper;

        public SubjectGroupService(IRepositoryMariaWrapper repositoryWrapper, IMapper mapper)
        {
            RepositoryWrapper = repositoryWrapper;
            Mapper = mapper;
        }

        public List<ExportReportMonth> ExportReport(ExportReceipt exportReceipt)
        {
            List<ExportReportMonth> exportReportMonths = new List<ExportReportMonth>();

            var receipts11 = RepositoryWrapper.Receipts
                .FindAll()
                .Select(x => new { date = x.CreatedAt.ToString("dd/MM/yyyy"), id = x.ReceiptId })
                .ToList();
            List<Receipt> receipts = RepositoryWrapper.Receipts
                .FindAll()
                .ToList()
                .Where(x => x.CreatedAt.ToString("dd/MM/yyyy").Equals(exportReceipt.DateTimeData/*.ToUniversalTime()*/.ToString("dd/MM/yyyy")))
                .ToList();
            List<Class> Classes = RepositoryWrapper.Classes
                .FindAll()
                .ToList()
                .Where(x => receipts.Any(r => r.ClassId.Equals(x.ClassId)))
                .ToList();
            List<ClassSchedule> ClassSchedules = RepositoryWrapper.ClassSchedules
                .FindAll()
                .ToList()
                .Where(x => Classes.Any(c => c.ClassId.Equals(x.ClassId)))
                .ToList();
            List<Subject> subjects = RepositoryWrapper.Subjects
                .FindAll()
                .ToList()
                .Where(x => ClassSchedules.Any(z => z.SubjectId.Equals(x.SubjectId)))
                .ToList();

            List<Student> Students = RepositoryWrapper.Students
                .FindAll()
                .ToList()
                .Where(x => receipts.Any(z => z.StudentId.Equals(x.StudentId)))
                .ToList();
            List<Lecturer> Lecturers = RepositoryWrapper.Lecturers
                .FindAll()
                .ToList()
                .Where(x => ClassSchedules.Any(z => z.LecturerId.Equals(x.LecturerId)))
                .ToList();


            int i = 0;
            foreach (Receipt receipt in receipts)
            {
                string subjectName = "";
                List<Subject> subjectsTemp = subjects.Where(s => s.Classes
                                                            .Any(c =>
                                                                c.ClassId.Equals(receipt.ClassId))
                                                            )
                                                        .ToList();
                foreach (Subject subject in subjectsTemp)
                {
                    subjectName += subject.SubjectName + " ";
                }
                string classScheduleString = "";
                string workShiftString = "";
                foreach (var schedule in ClassSchedules
                    .Where(x => x.ClassId.Equals(receipt.ClassId))
                    .GroupBy(x =>
                    new
                    {
                        DayOfWeek = x.DayOfWeek
                    })
                    .Select(x => new
                    {
                        DayOfWeek = x.Key.DayOfWeek,
                        ClassSchedule = x.FirstOrDefault()
                    })
                    .ToList())
                {
                    if (classScheduleString == "")
                    {
                        classScheduleString += schedule.DayOfWeek.ToString();
                    }
                    else
                    {
                        classScheduleString += ", " + schedule.DayOfWeek.ToString();
                    }
                    if (workShiftString == "")
                    {
                        workShiftString += TimeSpan.FromTicks(schedule.ClassSchedule.TimeStart).ToString(@"hh\:mm") + "-" + TimeSpan.FromTicks(schedule.ClassSchedule.TimeEnd).ToString(@"hh\:mm");
                    }
                    else
                    {
                        workShiftString += ", " + TimeSpan.FromTicks(schedule.ClassSchedule.TimeStart).ToString(@"hh\:mm") + "-" + TimeSpan.FromTicks(schedule.ClassSchedule.TimeEnd).ToString(@"hh\:mm");
                    }
                }
                Student student = Students.FirstOrDefault(x => x.StudentId.Equals(receipt.StudentId));
                string fullNameStudent = student != null ? student.StudentFirstName + " " + student.StudentLastName : " ";

                string fullNameLecture = "";
                foreach (var schedule in ClassSchedules
                    .Where(x => 
                            x.ClassId.Equals(receipt.ClassId) 
                            && subjectsTemp.Any(z => z.SubjectId.Equals(x.SubjectId))
                        )
                    .GroupBy(x =>
                    new
                    {
                        LecturerId = x.LecturerId
                    })
                    .Select(x => new
                    {
                        LecturerId = x.Key.LecturerId,
                        ClassSchedule = x.FirstOrDefault()
                    })
                    .ToList())
                {
                    Lecturer lecturer = Lecturers.FirstOrDefault(x => x.LecturerId.Equals(schedule.LecturerId));
                    if (fullNameLecture == "")
                    {
                        fullNameLecture += lecturer != null ? lecturer.LecturerFistName + " " + lecturer.LecturerLastName: " ";
                    }
                    else
                    {
                        fullNameLecture += lecturer != null ? ", " + lecturer.LecturerFistName + " " + lecturer.LecturerLastName : " ";
                    }
                }
                    
                ExportReportMonth reportMonth = new ExportReportMonth
                {
                    STT = i + 1,
                    Subject = subjectName,
                    ScheduleClass = classScheduleString,
                    WorkShift = workShiftString,
                    NoBook = receipt.NoBook,
                    TuitionFee = receipt.Amount,
                    Description = null,
                    Description2 = null,
                    Column1 = "Thu học phí môn: " + subjectName,
                    NameProductService = "Thu học phí môn: " + subjectName,
                    FullNameStudent = fullNameStudent,
                    FullNameLecture = fullNameLecture,
                    TaxCode = null,
                    OneIsTrue = null,
                    SubjectNameAndLectureName = subjectName + " " + fullNameLecture,
                    SalaryCode = null,
                    SubjectNameAndSchedule = subjectName + " " + classScheduleString,
                    LectureV2 = null
                };
                exportReportMonths.Add(reportMonth);
                i++;
            }
            return exportReportMonths;
        }

        public List<RegistrationStudy> GetRegister(string accountId)
        {
            Student student = RepositoryWrapper.Students.FindByCondition(x => x.AccountId.Equals(accountId)).FirstOrDefault();
            if (student is null)
            {
                return null;
            }
            return RepositoryWrapper
                .RegistrationStudies
                .FindByCondition(x => x.StudentId.Equals(student.StudentId))
                .ToList();
        }

        public List<ResponseEnrollmentClass> GetClassWithSubjectGroupId(SubjectGroup subjectGroup)
        {
            List<SubjectGroup> subjectGroups = RepositoryWrapper
                .SubjectGroups
                .FindAll()
                .Include(x => x.Subjects)
                .ToList();
            var classSchedulessss = GetClassSchedules();

            var classSchedules = classSchedulessss
                .GroupBy(x =>
                    new
                    {
                        SubjectId = x.SubjectId,
                        ClassId = x.ClassId
                    })
                .Select(x => new
                {
                    ClassId = x.Key.ClassId,
                    SubjectId = x.Key.SubjectId,
                    ClassSchedule = x.FirstOrDefault()
                })
                .ToList();

            List<Class> classes = RepositoryWrapper
                .Classes
                .FindAll()
                .ToList()
                .Where(x => classSchedules
                    .Any(s => s.ClassId.Equals(x.ClassId)) &&
                    x.Admission == Class.STATUS_OPEN.OPEN
                    )
                .ToList();
            List<ResponseEnrollmentClass> reponses = new List<ResponseEnrollmentClass>();
            //return reponses;

            //List<RegistrationStudy> RegistrationStudy = RepositoryWrapper.RegistrationStudies.FindAll().Where(x => classes.Any(z => z.ClassId.Equals(x.ClassId))).ToList();
            
            foreach (Class @class in classes)
            {
                ClassSchedule classSchedule = classSchedules.FirstOrDefault(x => x.ClassId.Equals(@class.ClassId)).ClassSchedule;
                //List<ClassSchedule> classSchedules = classSchedules.Where(x => x.ClassId.Equals(@class.ClassId)).ClassSchedule;
                Course course = RepositoryWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
                SchoolYear schoolYear = RepositoryWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
                Lecturer lecturer = classSchedule != null ? RepositoryWrapper.Lecturers.FirstOrDefault(x => x.LecturerId.Equals(classSchedule.LecturerId)) : null;
                //SubjectGroup subjectGroupTemp = subjectGroup.FirstOrDefault(x =>
                //    x.Subjects.Any(z =>
                //        z.SubjectId.Equals(classSchedule.SubjectId)
                //    ));
                if (!subjectGroup.Subjects.Any(z => z.SubjectId.Equals(classSchedule.SubjectId)))
                {
                    continue;
                }
                //@class.QuantityRegisted = RegistrationStudy.Where(x => x.ClassId == @class.ClassId).ToList().Count;
                @class.QuantityRegisted = RepositoryWrapper.RegistrationStudies.FindByCondition(x => x.ClassId == @class.ClassId).ToList().Count;
                ResponseEnrollmentClass response = new ResponseEnrollmentClass
                {
                    Class = @class,
                    Course = course,
                    SchoolYear = schoolYear,
                    Lecturer = lecturer,
                    DayOfWeek = classSchedule != null ? classSchedule.DayOfWeek : null,
                    TimeStart = classSchedule != null ? classSchedule?.DateStart : null,
                    ClassSchedule = classSchedule
                };
                reponses.Add(response);
            }
            return reponses;
        }

        public List<ClassSchedule> GetClassSchedules()
        {
            return RepositoryWrapper
                .ClassSchedules
                .FindAll()
                .Include(x => x.Subject)
                .ToList();
        }

        public List<Class> GetAllClassOpen()
        {
            List<ClassSchedule> classSchedulesTemp = GetClassSchedules();
            var classSchedules = classSchedulesTemp
                .GroupBy(x =>
                    new
                    {
                        SubjectId = x.SubjectId,
                        ClassId = x.ClassId
                    })
                .Select(x => new
                {
                    SubjectId = x.Key.SubjectId,
                    ClassId = x.Key.ClassId,
                    ClassSchedule = x.FirstOrDefault()
                })
                .ToList();

            List<Class> classes = RepositoryWrapper
                .Classes
                .FindAll()
                .ToList()
                .Where(x => classSchedules
                    .Any(s => s.ClassId.Equals(x.ClassId)) &&
                    x.Admission == Class.STATUS_OPEN.OPEN
                    )
                .ToList();
            return classes;
        }

        public List<ResponseEnrollmentClass> GetClassAllClassOpen()
        {
            List<ClassSchedule> classSchedulesTemp = GetClassSchedules();
            var classSchedules = classSchedulesTemp
                .GroupBy(x =>
                    new
                    {
                        SubjectId = x.SubjectId,
                        ClassId = x.ClassId
                    })
                .Select(x => new
                {
                    SubjectId = x.Key.SubjectId,
                    ClassId = x.Key.ClassId,
                    ClassSchedule = x.FirstOrDefault()
                })
                .ToList();

            List<Class> classes = RepositoryWrapper
                .Classes
                .FindAll()
                .ToList()
                .Where(x => classSchedules
                    .Any(s => s.ClassId.Equals(x.ClassId)) &&
                    x.Admission == Class.STATUS_OPEN.OPEN
                    )
                .ToList();


            List<SubjectGroup> subjectGroups = RepositoryWrapper
                .SubjectGroups
                .FindAll()
                .Include(x => x.Subjects)
                .ToList();
            subjectGroups = subjectGroups.FilterClass(classSchedulesTemp);

            List<ResponseEnrollmentClass> reponses = new List<ResponseEnrollmentClass>();
            //List<RegistrationStudy> RegistrationStudy = RepositoryWrapper.RegistrationStudies.FindAll().ToList().Where(x => classes.Any(z => z.ClassId.Equals(x.ClassId))).ToList();
            
            foreach (Class @class in classes)
            {
                ClassSchedule classSchedule = classSchedules.FirstOrDefault(x => x.ClassId.Equals(@class.ClassId)).ClassSchedule;
                //List<ClassSchedule> classSchedules = classSchedules.Where(x => x.ClassId.Equals(@class.ClassId)).ClassSchedule;
                Course course = RepositoryWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(@class.CourseID));
                SchoolYear schoolYear = RepositoryWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(@class.SchoolYearID));
                Lecturer lecturer = classSchedule != null ? RepositoryWrapper.Lecturers.FirstOrDefault(x => x.LecturerId.Equals(classSchedule.LecturerId)) : null;
                //SubjectGroup subjectGroupTemp = subjectGroups.FirstOrDefault(x => x.Subjects.Any(z => z.SubjectId.Equals(classSchedule.SubjectId)));
                SubjectGroup subjectGroupTemp = subjectGroups
                    .FirstOrDefault(
                        x => x.Subjects.Any(
                            z => z.SubjectId.Equals(classSchedule.SubjectId)
                            )
                        );
                if (subjectGroupTemp is null)
                {
                    continue;
                }
                //@class.QuantityRegisted = RegistrationStudy.Where(x => x.ClassId == @class.ClassId).ToList().Count;
                @class.QuantityRegisted = RepositoryWrapper.RegistrationStudies.FindByCondition(x => x.ClassId == @class.ClassId).ToList().Count;
                ResponseEnrollmentClass response = new ResponseEnrollmentClass
                {
                    Class = @class,
                    Course = course,
                    SchoolYear = schoolYear,
                    Lecturer = lecturer,
                    DayOfWeek = classSchedule != null ? classSchedule.DayOfWeek : null,
                    TimeStart = classSchedule != null ? classSchedule?.DateStart : null,
                    ClassSchedule = classSchedule,
                    SubjectGroupId = subjectGroupTemp != null ? subjectGroupTemp.SubjectGroupId : null
                };
                reponses.Add(response);
            }
            return reponses;
        }

        public List<SubjectGroup> GetAll()
        {
            return RepositoryWrapper
                .SubjectGroups
                .FindAll()
                .Include(x => x.Subjects)
                .ToList();
        }

        public SubjectGroup GetById(string subjectGroupId)
        {
            return RepositoryWrapper
                .SubjectGroups
                .FindByCondition(x => x.SubjectGroupId.Equals(subjectGroupId))
                .Include(x => x.Subjects)
                .FirstOrDefault();
        }

        public SubjectGroup Store(StoreSubjectGroupRequest request)
        {
            SubjectGroup subjectGroup = Mapper.Map<SubjectGroup>(request);
            RepositoryWrapper.SubjectGroups.Add(subjectGroup);
            RepositoryWrapper.SaveChanges();
            return subjectGroup;
        }

        public SubjectGroup Update(UpdateSubjectGroupRequest request, SubjectGroup subjectGroup)
        {
            subjectGroup = request.MergeData<SubjectGroup>(subjectGroup);
            RepositoryWrapper.SubjectGroups.UpdateMaria(subjectGroup);
            RepositoryWrapper.SaveChanges();
            return subjectGroup;
        }

        public void Delete(SubjectGroup subjectGroup)
        {
            RepositoryWrapper.SubjectGroups.RemoveMaria(subjectGroup);
            RepositoryWrapper.SaveChanges();
        }

        public ResponseTable ShowAll(SearchSubjectGroupRequest request)
        {
            var querySearch = RepositoryWrapper.SubjectGroups.FindAll();
            #region Sơn
            //if (request.Search != null)
            //{
            //    querySearch = querySearch.Where(s => s.SubjectGroupName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            //}
            #endregion
            #region Fix for Document
            List<SubjectGroup> subjectGroups = querySearch.ToList().Where(m => String.IsNullOrEmpty(request.Search) ||
                                                (m.SubjectGroupName != null && m.SubjectGroupName.ToLower().RemoveUnicode().Contains(request.Search.ToLower().RemoveUnicode()))
                                                )
                                                .ToList();
            #endregion
            if (!string.IsNullOrEmpty(request.SortField) || !string.IsNullOrEmpty(request.SortOrder))
            {
                var query = request.SortField + " " + request.SortOrder;
                subjectGroups = subjectGroups.AsQueryable().OrderBy(query).ToList();
            }
            return new ResponseTable
            {
                Data = request.Page != 0 ? subjectGroups.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList() : subjectGroups.ToList(),
                Info = new Info
                {
                    Page = request.Page != 0 ? request.Page : 1,
                    Limit = request.Page != 0 ? request.Limit : subjectGroups.Count(),
                    TotalRecord = subjectGroups.Count(),
                }
            };
        }

        public string GetSubjectGroupName(string subjectGroupId)
        {
            return RepositoryWrapper.SubjectGroups.FindByCondition(s => s.SubjectGroupId.Equals(subjectGroupId)).Select(s => s.SubjectGroupName).FirstOrDefault();
        }
    }
}
