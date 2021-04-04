using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.App.Services;
using Project.Modules.Classes.Entities;
using Project.Modules.Courses.Services;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.SubjectGroups.Services;
using Project.Modules.Subjects.Entities;
using Project.Modules.Subjects.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Project.Modules.Subjects.Services
{
    public interface ISubjectService : IService<Subject>
    {
        List<Subject> GetAll();
        ResponseTable ShowAll(SearchSubjectRequest request);
        Subject Store(StoreSubjectRequest request);
        Subject Update(UpdateSubjectRequest request, Subject subject);
        bool CheckSubjectCodeIsExistsExceptSubject(UpdateSubjectRequest request, Subject exceptSubject);
        bool CheckSubjectIsContainedInSubjectGroup(string subjectGroupId);
        bool CheckSubjectIsContainInCourse(string courseId);
        bool CheckCourseSubjectIsContainInCourse(string courseId);
        bool CheckCourseIdInClass(string courseId);
    }
    public class SubjectService : ISubjectService
    {
        private readonly IRepositoryMariaWrapper RepositoryMaria;
        private readonly ICourseService CourseService;
        private readonly ISubjectGroupService SubjectGroupService;
        private readonly IMapper Mapper;

        public SubjectService(IRepositoryMariaWrapper repositoryWrapper, ICourseService courseService, ISubjectGroupService subjectGroupService, IMapper mapper)
        {
            RepositoryMaria = repositoryWrapper;
            CourseService = courseService;
            SubjectGroupService = subjectGroupService;
            Mapper = mapper;
        }

        public Subject GetById(string subjectId)
        {
            return RepositoryMaria
                .Subjects
                //.FindByCondition(x => x.SubjectId.Equals(subjectId))
                //.ToList()
                //.Select(x =>
                //{
                //    x.Classes = RepositoryMaria
                //    .Classes
                //    .FindByCondition(y => y.CourseID.Equals(x.CourseId))
                //    .Where(x => x.Admission == Classes.Entities.Class.STATUS_OPEN.OPEN)
                //    .ToList();
                //    return x;
                //}
                //)
                //.FirstOrDefault();
                .GetById(subjectId);
        }

        public ResponseTable ShowAll(SearchSubjectRequest request)
        {
            var querySearch = RepositoryMaria.Subjects.FindAll();
            if (!string.IsNullOrEmpty(request.SubjectGroupId))
            {
                querySearch = querySearch.Where(s => s.SubjectGroupId.Equals(request.SubjectGroupId));
            }
            if (!string.IsNullOrEmpty(request.CourceId))
            {
                querySearch = querySearch.Where(s => s.CourseId.Equals(request.CourceId));
            }

            //if (!string.IsNullOrEmpty(request.Search))
            //{
            //    querySearch = querySearch.Where(s => 
            //                s.SubjectName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) 
            //                || 
            //                s.SubjectCode.Contains(request.Search, StringComparison.OrdinalIgnoreCase));
            //}
            List<Subject> subjects = querySearch.ToList();
            #region Search
            subjects = subjects.Where(m => (String.IsNullOrEmpty(request.Search) ||
                                                    (m.SubjectName != null && m.SubjectName.ToLower().RemoveUnicode().Contains(request.Search.ToLower().RemoveUnicode()))
                                                    ||
                                                    (m.SubjectCode != null && m.SubjectCode.ToLower().RemoveUnicode().Contains(request.Search.ToLower().RemoveUnicode()))
                                            )
                                            &&
                                           (String.IsNullOrEmpty(request.CourceId) ||
                                                    (m.CourseId != null && m.CourseId.Contains(request.CourceId))
                                           )
                                           &&
                                           (String.IsNullOrEmpty(request.SubjectGroupId) ||
                                                    (m.SubjectGroupId != null && m.SubjectGroupId.Contains(request.SubjectGroupId)))
                                           )
                                           .ToList();
            #endregion
            foreach (Subject subject in subjects)
            {
                subject.SubjectGroupName = SubjectGroupService.GetSubjectGroupName(subject.SubjectGroupId);
                subject.CourseName = CourseService.GetCourseName(subject.CourseId);
            }
            if (!string.IsNullOrEmpty(request.SortField) || !string.IsNullOrEmpty(request.SortOrder))
            {
                //var query = request.SortField + " " + request.SortOrder;
                //querySearch = subjects.AsQueryable().OrderBy(request.SortField + " " + request.SortOrder);
                subjects = subjects.AsQueryable().OrderBy(request.SortField + " " + request.SortOrder).ToList();
            }

            return new ResponseTable
            {
                Data = request.Page != 0 ? subjects.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList() : subjects.ToList(),
                Info = new Info
                {
                    Page = request.Page != 0 ? request.Page : 1,
                    Limit = request.Page != 0 ? request.Limit : subjects.Count(),
                    TotalRecord = subjects.Count(),
                }
            };
        }

        public Subject Store(StoreSubjectRequest request)
        {
            Subject subject = Mapper.Map<Subject>(request);
            RepositoryMaria.Subjects.Add(subject);
            RepositoryMaria.SaveChanges();
            return subject;
        }

        public bool CheckSubjectCodeIsExistsExceptSubject(UpdateSubjectRequest request, Subject exceptSubject)
        {
            if (!request.SubjectCode.Equals(exceptSubject.SubjectCode))
            {
                return RepositoryMaria.Subjects
                .Any(x => x.SubjectCode.Equals(request.SubjectCode));
            }
            return false;
        }

        public Subject Update(UpdateSubjectRequest request, Subject subject)
        {
            subject = request.MergeData<Subject>(subject);
            RepositoryMaria.Subjects.UpdateMaria(subject);
            RepositoryMaria.SaveChanges();
            return subject;
        }

        public void Delete(Subject subject)
        {
            RepositoryMaria.Subjects.RemoveMaria(subject);
            RepositoryMaria.SaveChanges();
        }

        public bool CheckSubjectIsContainedInSubjectGroup(string subjectGroupId)
        {
            return RepositoryMaria.Subjects.Any(s => s.SubjectGroupId.Equals(subjectGroupId));
        }

        public bool CheckSubjectIsContainInCourse(string courseId)
        {
            return RepositoryMaria.Subjects.Any(s => s.CourseId.Equals(courseId));
        }

        public bool CheckCourseIdInClass(string courseId)
        {
            return RepositoryMaria.Classes.FindAll().ToList().Any(x => x.CourseID == courseId);
        }
        public bool CheckCourseSubjectIsContainInCourse(string courseId)
        {
            return RepositoryMaria.CourseSubjects.FindAll().ToList().Any(s => s.CourseId.Equals(courseId));
        }

        public List<Subject> GetAll()
        {
            return RepositoryMaria
                .Subjects
                .FindAll()
                //.ToList()
                //.Select(x =>
                //{
                //    x.Classes = RepositoryMaria
                //    .Classes
                //    .FindByCondition(y => y.CourseID.Equals(x.CourseId))
                //    .Where(x => x.Admission == Classes.Entities.Class.STATUS_OPEN.OPEN)
                //    .ToList();
                //    return x;
                //}
                //)
                .ToList();
        }
    }
}
