using MongoDB.Driver;
using Project.App.DesignPatterns.Repositories;
using Project.Modules.Classes.Entities;
using Project.Modules.Lecturers.Entities;
using Project.Modules.Lecturers.Models;
using Project.Modules.Lecturers.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Lecturers.Services
{
    public interface ILectureSalaryService
    {
        (object data, string messasge) GetEmployeeSalaryBill(string courseId, string lectureId);
        (object data, string message) StoreEmployeeSalary(StoreSalary request);
        (object data, string message) ConfirmSalaryForCourse(string courseId);
    }
    public class LectureSalaryService : ILectureSalaryService
    {
        private readonly IRepositoryMariaWrapper repositoryMariaWrapper;
        public LectureSalaryService(IRepositoryMariaWrapper repositoryMariaWrapper)
        {
            this.repositoryMariaWrapper = repositoryMariaWrapper;
        }
        public (object data, string messasge) GetEmployeeSalaryBill(string courseId, string lectureId)
        {
            ResponseEmployeeSalaryBill responseEmployeeSalaryBill = new ResponseEmployeeSalaryBill();
            EmployeeSalary employeeSalary = repositoryMariaWrapper.EmployeeSalaries.FirstOrDefault(x => x.CourseId.Equals(courseId) && x.LectureId.Equals(lectureId));
            if (employeeSalary != null)
            {
                responseEmployeeSalaryBill.Lecturer = repositoryMariaWrapper.Lecturers.FirstOrDefault(x => x.LecturerId.Equals(lectureId));
                responseEmployeeSalaryBill.SalaryBillNumber = null;
                responseEmployeeSalaryBill.BaseSalary = employeeSalary.EmployeeSalaryBasic;
                responseEmployeeSalaryBill.EmployeeSalary = employeeSalary;
            }
            else
            {
                List<string> listClassId = repositoryMariaWrapper.Classes.FindByCondition(x => x.CourseID.Equals(courseId)).Select(x => x.ClassId).ToList();
                if (listClassId.Count < 1)
                {
                    return (null, "NoClassesExistInThisCourse");
                }
                List<string> classIds = repositoryMariaWrapper.ClassSchedules.FindByCondition(x => listClassId.Contains(x.ClassId) && x.LecturerId.Equals(lectureId)).Select(x => x.ClassId).Distinct().ToList();
                if (classIds.Count < 1)
                {
                    return (null, "ThisTeacherIsNotInChargeOfAnyClass");
                }
                responseEmployeeSalaryBill.Lecturer = repositoryMariaWrapper.Lecturers.FirstOrDefault(x => x.LecturerId.Equals(lectureId));
                responseEmployeeSalaryBill.SalaryBillNumber = null;
                responseEmployeeSalaryBill.BaseSalary = repositoryMariaWrapper.Receipts.FindByCondition(x => classIds.Contains(x.ClassId)).Sum(x => x.TypeAmount);
                responseEmployeeSalaryBill.EmployeeSalary = repositoryMariaWrapper.EmployeeSalaries.FirstOrDefault(x => x.LectureId.Equals(lectureId) && x.CourseId.Equals(courseId));
            }
            return (responseEmployeeSalaryBill, "Success");
        }
        public (object data, string message) StoreEmployeeSalary(StoreSalary request)
        {
            EmployeeSalary employeeSalary = repositoryMariaWrapper.EmployeeSalaries.FirstOrDefault(x => x.EmployeeSalaryId.Equals(request.EmployeeSalaryId));
            if (employeeSalary is null)
            {
                employeeSalary = new EmployeeSalary()
                {
                    CourseId = request.CourseId,
                    EmployeeSalaryBasic = request.BaseSalary.Value,
                    EmployeeSalaryNote = request.Note,
                    EmployeeSalarySubsidiesOther = request.TotalSalarySubsidiesOther.Value,
                    EmployeeSalarySubsidiesPercent = request.Percent.Value,
                    EmployeeSalaryReal = request.TotalSalary.Value,
                    EmployeeSalaryTotal = request.TotalSalary.Value + request.TotalSalarySubsidiesOther.Value,
                    LectureId = request.LectureId,
                    EmployeeSalarySubsidiesOtherName = request.TotalSalarySubsidiesOtherName,
                    EmployeeSalaryNumber = 1,
                    CheckUpdate = true
                };
                repositoryMariaWrapper.EmployeeSalaries.Add(employeeSalary);
            }
            else
            {
                employeeSalary.EmployeeSalaryBasic = request.BaseSalary.Value;
                employeeSalary.EmployeeSalaryNote = request.Note;
                employeeSalary.EmployeeSalarySubsidiesOther = request.TotalSalarySubsidiesOther.Value;
                employeeSalary.EmployeeSalarySubsidiesPercent = request.Percent.Value;
                employeeSalary.EmployeeSalaryTotal = request.TotalSalary.Value + request.TotalSalarySubsidiesOther.Value;
                employeeSalary.EmployeeSalarySubsidiesOtherName = request.TotalSalarySubsidiesOtherName;
                employeeSalary.EmployeeSalaryReal = request.TotalSalary.Value;
                employeeSalary.CheckUpdate = false;
            }
            repositoryMariaWrapper.SaveChanges();
            return (employeeSalary, "Success");
        }
        public (object data, string message) ConfirmSalaryForCourse(string courseId)
        {
            List<EmployeeSalary> employeeSalaries = repositoryMariaWrapper.EmployeeSalaries.FindByCondition(x => x.CourseId.Equals(courseId)).ToList();
            if (employeeSalaries.Count < 1)
            {
                return (null, "EmployeeSalaryBelongToThisCourseNotFound");
            }
            employeeSalaries.ForEach(x => x.EmployeeSalaryConfirm = EmployeeSalaryConfirm.Confirm);
            employeeSalaries.ForEach(x => x.EmployeeSalaryUpdatedAt = DateTime.UtcNow);
            repositoryMariaWrapper.SaveChanges();
            return (employeeSalaries, "Success");
        }
    }
}
