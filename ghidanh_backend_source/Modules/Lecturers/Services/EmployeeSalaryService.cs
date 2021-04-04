using Microsoft.EntityFrameworkCore;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Classes.Entities;
using Project.Modules.Lecturers.Entities;
using Project.Modules.Lecturers.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
namespace Project.Modules.Lecturers.Services
{

    public interface IEmployeeSalaryService
    {
        public ResponseTable ShowAll(ShowAllEmployeeSalary request);
    }
    public class EmployeeSalaryService : IEmployeeSalaryService
    {
        private readonly IRepositoryMongoWrapper _repositoryMongoWrapper;
        private readonly IRepositoryMariaWrapper _repositoryMariaWrapper;
        public EmployeeSalaryService(IRepositoryMongoWrapper repositoryMongoWrapper, IRepositoryMariaWrapper repositoryMariaWrapper)
        {
            _repositoryMongoWrapper = repositoryMongoWrapper;
            _repositoryMariaWrapper = repositoryMariaWrapper;
        }


        public ResponseTable ShowAll(ShowAllEmployeeSalary request)
        {
            // Get class in CourseId
            List<Class> classes = _repositoryMariaWrapper.Classes.FindByCondition(x => x.CourseID == request.CourseId).ToList();

            var classSchedules = _repositoryMariaWrapper.ClassSchedules.FindByCondition(x => classes.Select(y => y.ClassId).Contains(x.ClassId)).Include(x => x.Lecturer).ToList().GroupBy(x => x.LecturerId).Select(x => new { LecturerId = x.Key, ClassSchedule = x.FirstOrDefault() }).ToList();

            List<ReponseEmployeeSalaryResponse> reponseEmployeeSalaryResponses = new List<ReponseEmployeeSalaryResponse>();

            foreach (var item in classSchedules)
            {
                if (item.ClassSchedule.Lecturer is null) continue;

                ReponseEmployeeSalaryResponse tmp = new ReponseEmployeeSalaryResponse
                {
                    LecturerId = item.LecturerId,
                    LecturerCode = item.ClassSchedule.Lecturer.LecturerCode,
                    LecturerFistName = item.ClassSchedule.Lecturer.LecturerFistName,
                    LecturerLastName = item.ClassSchedule.Lecturer.LecturerLastName,
                    LecturerBirthday = item.ClassSchedule.Lecturer.LecturerBirthday,
                    LecturerEmail = item.ClassSchedule.Lecturer.LecturerEmail,
                    ExtraSubject = item.ClassSchedule.Lecturer.ExtraSubject,
                    LecturerGender = item.ClassSchedule.Lecturer.LecturerGender,
                    LecturerPhone = item.ClassSchedule.Lecturer.LecturerPhone,
                   
                };
                List<string> extraSubjectIds = _repositoryMariaWrapper.LecturerSubjects.FindByCondition(x => x.LecturerId == item.LecturerId).Select(m => m.SubjectId).ToList();
                tmp.ExtraSubject = _repositoryMariaWrapper.Subjects.FindByCondition(m => extraSubjectIds.Contains(m.SubjectId)).ToList();
                tmp.LecturerName = tmp.LecturerFistName + " " + tmp.LecturerLastName;

                var employeeSalaries = _repositoryMariaWrapper.EmployeeSalaries.FirstOrDefault(x => x.LectureId == tmp.LecturerId && x.CourseId == request.CourseId);
                if(employeeSalaries != null)
                {
                    tmp.EmployeeSalaryConfirm = employeeSalaries.EmployeeSalaryConfirm;
                    tmp.EmployeeSalaryTotal = employeeSalaries.EmployeeSalaryTotal;

                }


                reponseEmployeeSalaryResponses.Add(tmp);
            }
            reponseEmployeeSalaryResponses = reponseEmployeeSalaryResponses.Where(x => String.IsNullOrEmpty(request.Search) || x.LecturerCode.Equals(request.Search) || x.LecturerCode.Equals(request.Search) || x.LecturerEmail.Equals(request.Search) || x.EmployeeSalaryConfirmText.Equals(request.Search)).ToList();

            var query = request.SortField + " " + request.SortOrder;
            ResponseTable result = new ResponseTable
            {
                Info = new Info
                {
                    Page = request.Page,
                    Limit = request.Limit,
                    TotalRecord = reponseEmployeeSalaryResponses.Count,
                    EmployeeSalaryTotalAll = reponseEmployeeSalaryResponses.Where(x=> x.EmployeeSalaryTotal.HasValue).Sum(x => x.EmployeeSalaryTotal.Value)
                }
            };


            // page == 0 show all
            if (request.Page == 0)
            {
                result.Data = reponseEmployeeSalaryResponses.AsQueryable().OrderBy(query).ToList();
                return result;
            }


            // panigation
            result.Data = reponseEmployeeSalaryResponses.Skip((request.Page - 1) * request.Limit).Take(request.Limit).AsQueryable().OrderBy(query).ToList();
            return result;
        }




    }

    public class ReponseEmployeeSalaryResponse : Lecturer
    {
        public EmployeeSalaryConfirm EmployeeSalaryConfirm { get; set; } 
        public string EmployeeSalaryConfirmText
        {
            get
            {
                if (EmployeeSalaryConfirm == EmployeeSalaryConfirm.Confirm)
                {
                    return "Đã chốt lương";
                }
                return "Chưa chốt lương";
            }
        }

        public double? EmployeeSalaryTotal { get; set; } = null;



        public string LecturerName { get; set; }
        

    }
}
