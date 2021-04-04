using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Classes.Entities;
using Project.Modules.ClassSchedules.Entities;
using Project.Modules.ClassSchedules.Requests;
using Project.Modules.Courses.Entities;
using Project.Modules.Lecturers.Entities;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.Slots.Entities;
using Project.Modules.Students.Entities;
using Project.Modules.Subjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography.X509Certificates;

namespace Project.Modules.ClassSchedules.Services
{
    public interface IClassScheduleService
    {
        public Student DetailStudent(string userId);
        public Lecturer DetailLecturer(string userId);
        ResponseTable ListSchedule(ShowClassScheduleRequest request);
        List<ClassSchedule> ListSchedule(string subjectId, string classId);
        (bool, ClassScheduleResponse) AddSchedule(ClassSchedule classSchedule);
        List<ClassScheduleCalendar> ShowCalendar(ShowCalendarRequest request);
        ClassSchedule ShowDetailCalendar(string scheduleId);
        (ClassSchedule, string) EditCalendar(ClassSchedule classSchedule, EditScheduleRequest request);
        void DeleteCalendar(List<ClassSchedule> classSchedule);
        (List<ClassScheduleResponse>, string) CopyClass(CopyClassRequest request);
    }
    public class ClassScheduleService : IClassScheduleService
    {
        private readonly IRepositoryMariaWrapper repositoryWrapper;
        public ClassScheduleService(IRepositoryMariaWrapper RepositoryWrapper)
        {
            repositoryWrapper = RepositoryWrapper;
        }

        public ResponseTable ListSchedule(ShowClassScheduleRequest request)
        {
            List<ClassSchedule> classSchedules;
            IQueryable<ClassSchedule> classSchedulesQuery = repositoryWrapper.ClassSchedules
                    .FindByCondition(x => x.ClassId != null && (string.IsNullOrEmpty(request.Class) || request.Class.Equals(x.ClassId)))
                    .Include(x => x.Class)
                    .Include(x => x.Subject)
                    .Include(x => x.Lecturer)
                    .Where(x => string.IsNullOrEmpty(request.Search) ||
                                x.Lecturer.LecturerCode.Contains(request.Search) || 
                                x.Lecturer.LecturerFistName.Contains(request.Search) ||
                                x.Lecturer.LecturerLastName.Contains(request.Search) ||
                                (x.Lecturer.LecturerFistName + " " + x.Lecturer.LecturerLastName).Contains(request.Search) ||
                                x.Subject.SubjectName.Contains(request.Search));

            int countData = classSchedulesQuery.Count();
            if (request.Limit > 0 && request.Page > 0)
            {
                classSchedules = classSchedulesQuery
                    .Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList();
            }   
            else
            {
                classSchedules = classSchedulesQuery
                    .ToList();
            }
            if (!String.IsNullOrEmpty(request.SortField) || !String.IsNullOrEmpty(request.SortOrder))
            {
                var query = request.SortField + " " + request.SortOrder;
                if (!classSchedules.FieldExists<ClassSchedule>(request.SortField))
                {
                    Console.WriteLine("GuisaitenCotClassSchedule");
                }
                else
                {
                    classSchedules = classSchedules.AsQueryable().OrderBy(query).ToList();
                }
            }

            return new ResponseTable
            {
                Data = classSchedules.Select(x => new ClassScheduleResponse(x)).ToList(),
                Info = new Info
                {
                    Limit = request.Limit,
                    Page = request.Page,
                    TotalRecord = countData
                }
            };
        }

        public List<ClassSchedule> ListSchedule(string subjectId, string classId)
        {
            return repositoryWrapper.ClassSchedules.FindByCondition(x => x.ClassId.Equals(classId) && x.SubjectId.Equals(subjectId)).ToList();
        }

        private Lecturer LecturerExists(string lecturerId)
        {
            return repositoryWrapper.Lecturers.FirstOrDefault(x => x.LecturerId.Equals(lecturerId));
        }

        private Subject SubjectExists(string subjectId)
        {
            return repositoryWrapper.Subjects.FirstOrDefault(x => x.SubjectId.Equals(subjectId));
        }

        private Class ClassExists(string classId)
        {
            return repositoryWrapper.Classes.FirstOrDefault(x => x.ClassId.Equals(classId));
        }

        public Student DetailStudent(string userId)
        {
            return repositoryWrapper.Students.FirstOrDefault(x => x.AccountId.Equals(userId));
        }

        public Lecturer DetailLecturer(string userId)
        {
            return repositoryWrapper.Lecturers.FirstOrDefault(x => x.AccountId.Equals(userId));
        }

        public List<ClassScheduleCalendar> ShowCalendar(ShowCalendarRequest request)
        {
            List<ClassSchedule> classSchedules;

            int countDay = (request.DateTo - request.DateFrom).Days + 1;

            if(!string.IsNullOrEmpty(request.LecturerId))
            {
                classSchedules = ShowCalendarLecturer(request.LecturerId);
            }    
            else if (!string.IsNullOrEmpty(request.ClassId))
            {
                classSchedules = ShowCalendarByClassId(request.ClassId);
            }
            else
            {
                classSchedules = ShowCalendarStudent(request.StudentId);
            }

            List<ClassScheduleCalendar> classScheduleCalendars = new List<ClassScheduleCalendar>();
            for (int i = 0; i < countDay; i++)
            {
                DateTime dateCurrent = request.DateFrom.AddDays(i);
                classScheduleCalendars.Add(new ClassScheduleCalendar
                {
                    Date = dateCurrent.ToString("yyyy-MM-dd"),
                    DayOfWeek = dateCurrent.DayOfWeek.ToString(),
                    Data = classSchedules
                        .Where(x =>
                            x.DateStart <= dateCurrent &&
                            x.DateEnd >= dateCurrent &&
                            x.DayOfWeek.ToLower().Contains(dateCurrent.DayOfWeek.ToString().Substring(0, 3).ToLower()))
                        .Select(x => new ClassScheduleCalendarData(x)).ToList()
                });
            }
            return classScheduleCalendars;
        }

        private List<ClassSchedule> ShowCalendarStudent(string userId)
        {
            List<string> classIds = repositoryWrapper.RegistrationStudies
                .FindByCondition(x => x.StudentId.Equals(userId))
                .Select(x => x.ClassId)
                .ToList();

            return repositoryWrapper.ClassSchedules
                .FindByCondition(x => classIds.Contains(x.ClassId))
                .Include(x => x.Lecturer)
                .Include(x => x.Class)
                .Include(x => x.Subject)
                .ToList();
        }

        private List<ClassSchedule> ShowCalendarByClassId(string classId)
        {
            return repositoryWrapper.ClassSchedules
                .FindByCondition(x => x.ClassId.Equals(classId))
                .Include(x => x.Lecturer)
                .Include(x => x.Class)
                .Include(x => x.Subject)
                .ToList();
        }

        private List<ClassSchedule> ShowCalendarLecturer(string userId)
        {
            return repositoryWrapper.ClassSchedules
                .FindByCondition(x => x.LecturerId.Equals(userId))
                .Include(x => x.Lecturer)
                .Include(x => x.Class)
                .Include(x => x.Subject)
                .ToList();
        }

        public ClassSchedule ShowDetailCalendar(string scheduleId)
        {
            return repositoryWrapper.ClassSchedules
                .FindByCondition(x => x.ClassScheduleId.Equals(scheduleId))
                .Include(x => x.Class)
                .Include(x => x.Subject)
                .Include(x => x.Lecturer)
                .FirstOrDefault();
        }

        private bool CheckDuplicate(ClassSchedule classSchedule)
        {
            List<ClassSchedule> schedules = repositoryWrapper.ClassSchedules
                .FindByCondition(x => x.LecturerId.Equals(classSchedule.LecturerId) || x.ClassId.Equals(classSchedule.ClassId))
                .ToList();

            long timeStart = classSchedule.TimeStart;
            long timeEnd = classSchedule.TimeEnd;
            DateTime dayStart = classSchedule.DateStart;
            DateTime dayEnd = classSchedule.DateEnd;

            ClassSchedule scheduleCheckDay = schedules.FirstOrDefault(
                x => (x.LecturerId.Equals(classSchedule.LecturerId) || x.ClassId.Equals(classSchedule.ClassId)) && (
                (dayStart >= x.DateStart && dayStart <= x.DateEnd) ||
                (dayEnd >= x.DateStart && dayEnd <= x.DateEnd) ||
                (dayStart <= x.DateStart && dayEnd >= x.DateEnd)));

            if (scheduleCheckDay != null)
            {
                List<ClassSchedule> scheduleChecks = schedules.Where(x =>
                    (x.LecturerId.Equals(classSchedule.LecturerId) || x.ClassId.Equals(classSchedule.ClassId)) &&
                    (
                        (dayStart >= x.DateStart && dayStart <= x.DateEnd) ||
                        (dayEnd >= x.DateStart && dayEnd <= x.DateEnd) ||
                        (dayStart <= x.DateStart && dayEnd >= x.DateEnd)
                    ) && (
                        (timeStart + 1 >= x.TimeStart && timeStart + 1 <= x.TimeEnd) ||
                        (timeEnd - 1 >= x.TimeStart && timeEnd - 1 <= x.TimeEnd) ||
                        (timeStart - 1 <= x.TimeStart && timeEnd + 1 >= x.TimeEnd)
                    )).ToList();

                foreach (var item in classSchedule.DayOfWeek.Split(","))
                {
                    if (scheduleChecks.FirstOrDefault(x => x.DayOfWeek.Contains(item)) != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public (bool, ClassScheduleResponse) AddSchedule(ClassSchedule classSchedule)
        {
            Lecturer lecturer = LecturerExists(classSchedule.LecturerId);
            if (lecturer is null)
            {
                return (false, new ClassScheduleResponse(classSchedule, "LecturerIdInvalid"));
            }

            Subject subject = SubjectExists(classSchedule.SubjectId);
            if (subject is null)
            {
                return (false, new ClassScheduleResponse(classSchedule, "SubjectIdInvalid"));
            }

            Class @class = ClassExists(classSchedule.ClassId);
            if (@class is null)
            {
                return (false, new ClassScheduleResponse(classSchedule, "ClassIdInvalid"));
            }

            classSchedule.Class = @class;
            classSchedule.Subject = subject;
            classSchedule.Lecturer = lecturer;

            if (!CheckDuplicate(classSchedule))
            {
                return (false, new ClassScheduleResponse(classSchedule, "ScheduleTimeLeturerDuplicate"));
            }

            repositoryWrapper.ClassSchedules.Add(classSchedule);
            repositoryWrapper.SaveChanges();

            return (true, new ClassScheduleResponse(classSchedule, "Success"));
        }

        public (ClassSchedule, string) EditCalendar(ClassSchedule classSchedule, EditScheduleRequest request)
        {                
            if (!string.IsNullOrEmpty(request.LecturerId) && request.LecturerId != classSchedule.LecturerId)
            {
                if (LecturerExists(classSchedule.LecturerId) is null)
                {
                    return (null, "LecturerIdInvalid");
                }
                classSchedule.LecturerId = request.LecturerId;
            }

            if (!string.IsNullOrEmpty(request.SubjectId) && request.SubjectId != classSchedule.SubjectId)
            {
                if (SubjectExists(classSchedule.SubjectId) is null)
                {
                    return (null, "SubjectIdInvalid");
                }
                classSchedule.SubjectId = request.SubjectId;
            }
  
            if (!string.IsNullOrEmpty(request.ClassId) && request.ClassId != classSchedule.ClassId)
            {
                if (ClassExists(classSchedule.ClassId) is null)
                {
                    return (null, "ClassIdInvalid");
                }
                classSchedule.ClassId = request.ClassId;
            }    
                
            if(request.DateStart != null && request.DateEnd != null)
            {
                classSchedule.DateStart = request.DateStart.Value;
                classSchedule.DateEnd = request.DateEnd.Value;
            }

            if (!string.IsNullOrEmpty(request.ClassRoom))
            {
                classSchedule.ClassRoom = request.ClassRoom;
            }

            if (request.OnlineClassRoom != null)
            {
                classSchedule.ScheduleType = request.OnlineClassRoom.Value ? SCHEDULE_TYPE.ONLINE : SCHEDULE_TYPE.OFFLINE;
            }

            if (!string.IsNullOrEmpty(request.TimeStart) && !string.IsNullOrEmpty(request.TimeEnd))
            {
                classSchedule.TimeStart = TimeSpan.Parse(request.TimeStart).Ticks;
                classSchedule.TimeEnd = TimeSpan.Parse(request.TimeEnd).Ticks;
            }

            if(request.DayOfWeek.Count > 0)
            {
                classSchedule.DayOfWeek = string.Join(",", request.DayOfWeek).ToUpper();
            }

            if (!CheckDuplicate(classSchedule))
            {
                return (null, "ScheduleTimeLeturerDuplicate");
            }

            repositoryWrapper.ClassSchedules.UpdateMaria(classSchedule);
            repositoryWrapper.SaveChanges();
            return (classSchedule, "UpdateScheduleSuccess");
        }
        public void DeleteCalendar(List<ClassSchedule> classSchedules)
        {
            repositoryWrapper.ClassSchedules.RemoveRangeMaria(classSchedules);
            repositoryWrapper.SaveChanges();
        }

        public (List<ClassScheduleResponse>, string) CopyClass(CopyClassRequest request)
        {
            using IDbContextTransaction dbContextTransaction = repositoryWrapper.BeginTransaction();

            Course course = repositoryWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(request.CourseIdCopy));
            if(course is null)
            {
                return (new List<ClassScheduleResponse>(), "CourseNotFound");
            }

            Course newCourse = new Course
            {
                CourseCode = request.CourseCode,
                CourseName = request.CourseName
            };
            repositoryWrapper.Courses.Add(newCourse);
            repositoryWrapper.SaveChanges();

            List<Class> classes = repositoryWrapper.Classes.FindByCondition(x => x.CourseID.Equals(course.CourseId)).ToList();
            List<string> classIds = classes.Select(x => x.ClassId).ToList();
            Dictionary<string, string> mapClassId = new Dictionary<string, string>();
            foreach (Class item in classes)
            {
                Class newClass = new Class
                {
                    SchoolYearID = item.SchoolYearID,
                    ClassAmount = item.ClassAmount,
                    ClassQuantityStudent = item.ClassQuantityStudent,
                    ClassCode = item.ClassCode + "_1",
                    QuantityRegisted = item.QuantityRegisted,
                    ClassDescription = item.ClassDescription,
                    ClassAmountDescription = item.ClassAmountDescription,
                    Admission = item.Admission,
                    ClassImage = item.ClassImage,
                    ClassName = item.ClassName,
                    CourseID = newCourse.CourseId
                };
                repositoryWrapper.Classes.Add(newClass);
                mapClassId.Add(item.ClassId, newClass.ClassId);
            }

            repositoryWrapper.SaveChanges();

            var a = repositoryWrapper.ClassSchedules.FindByCondition(x => classIds.Contains(x.ClassId));
            List<ClassSchedule> classScheduleCopies = repositoryWrapper.ClassSchedules.FindByCondition(x => classIds.Contains(x.ClassId)).ToList();
            List<ClassSchedule> classScheduleCheck = classScheduleCopies;
            List<ClassSchedule> classScheduleInsert = new List<ClassSchedule>();
            List<ClassScheduleResponse> classScheduleErrors = new List<ClassScheduleResponse>();

            foreach (ClassSchedule item in classScheduleCopies)
            {
                ClassSchedule classSchedule = new ClassSchedule
                {
                    ClassId = mapClassId[item.ClassId],
                    ClassRoom = item.ClassRoom,
                    DateStart = request.DateStart.Value.Date,
                    DateEnd = request.DateEnd.Value.Date,
                    DayOfWeek = item.DayOfWeek,
                    LecturerId = item.LecturerId,
                    StepRepeat = item.StepRepeat,
                    SubjectId = item.SubjectId,
                    ScheduleType = item.ScheduleType,
                    TimeStart = item.TimeStart,
                    TimeEnd = item.TimeEnd
                };

                (bool check, ClassScheduleResponse classScheduleResponse) = AddSchedule(classSchedule);
                if(!check)
                {
                    classScheduleErrors.Add(classScheduleResponse);
                }
            }

            if (classScheduleErrors.Count > 0)
            {
                dbContextTransaction.Rollback();
            }
            else
            {
                dbContextTransaction.Commit();
            }

            return (classScheduleErrors, "");
        }
    }

    public class ClassScheduleError
    {
        public string LectureId { get; set; }
        public string LectureName { get; set; }
        public string Subject { get; set; }
        public string Class { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }

    public class ClassScheduleCalendar
    {
        public string Date { get; set; }
        public string DayOfWeek { get; set; }
        public List<ClassScheduleCalendarData> Data { get; set; } = new List<ClassScheduleCalendarData>();
    }

    public class ClassScheduleCalendarData
    {
        public string Class { get; set; }
        public string Subject { get; set; }
        public string ClassRoom { get; set; }
        public string LecturerFistName { get; set; }
        public string LecturerLastName { get; set; }
        public string DayOfWeek { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string Text { get; set; }

        public ClassScheduleCalendarData(ClassSchedule classSchedule)
        {
            Class = classSchedule.Class.ClassName;
            Subject = classSchedule.Subject.SubjectName;
            DateStart = classSchedule.DateStart;
            DateEnd = classSchedule.DateEnd;
            DayOfWeek = classSchedule.DayOfWeek;
            LecturerFistName = classSchedule.Lecturer.LecturerFistName;
            LecturerLastName = classSchedule.Lecturer.LecturerLastName;
            ClassRoom = classSchedule.ClassRoom;
            TimeStart = new TimeSpan(classSchedule.TimeStart).ToString(@"hh\:mm");
            TimeEnd = new TimeSpan(classSchedule.TimeEnd).ToString(@"hh\:mm");
            Text = $"{classSchedule.Subject.SubjectName} ({TimeStart} - {TimeEnd})";
        }
    }
}
