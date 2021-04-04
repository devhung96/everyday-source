using AutoMapper;
using MySql.Data.MySqlClient;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.App.Services;
using Project.Modules.Courses.Entities;
using Project.Modules.Courses.Requests;
using Project.Modules.SchoolYears.Extension;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Project.Modules.Courses.Services
{
    public interface ICourseService : IService<Course>
    {
        string GetCourseName(string courseId);
        Course Store(StoreCourseRequest request);
        bool CheckCourseCodeIsExistsExceptCourse(UpdateCourseRequest request, Course exceptCourse);
        Course Update(UpdateCourseRequest request, Course course);
        List<Course> GetAll();
        ResponseTable ShowAll(SearchCourseRequest request);

    }
    public class CourseService : ICourseService
    {
        private readonly IRepositoryMariaWrapper _repositoryWrapper;
        private readonly IMapper Mapper;

        public CourseService(IRepositoryMariaWrapper repositoryWrapper, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            Mapper = mapper;
        }

        public bool CheckCourseCodeIsExistsExceptCourse(UpdateCourseRequest request, Course exceptCourse)
        {
            if (!request.CourseCode.Equals(exceptCourse.CourseCode))
            {
                return _repositoryWrapper.Courses.Any(c => c.CourseCode.Equals(request.CourseCode));
            }
            return false;
        }

        public void Delete(Course course)
        {
            _repositoryWrapper.Courses.RemoveMaria(course);
            _repositoryWrapper.SaveChanges();                    
        }

        public List<Course> GetAll()
        {
            return _repositoryWrapper.Courses.FindAll().ToList();
        }

        public Course GetById(string id)
        {
            return _repositoryWrapper.Courses.GetById(id);
        }

        public string GetCourseName(string courseId)
        {
            return _repositoryWrapper.Courses.FindByCondition(s => s.CourseId.Equals(courseId)).Select(s => s.CourseName).FirstOrDefault();
        }

        public ResponseTable ShowAll(SearchCourseRequest request)
        {
            var querySearch = _repositoryWrapper.Courses.FindAll();
            if (!string.IsNullOrEmpty(request.Search))
            {
                querySearch = querySearch.Where(c => c.CourseName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) || c.CourseCode.Contains(request.Search,StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(request.SortField) || !string.IsNullOrEmpty(request.SortOrder))
            {
                var query = request.SortField + " " + request.SortOrder;
                querySearch = querySearch.OrderBy(query);
            }
            return new ResponseTable
            {
                Data = request.Page != 0 ? querySearch.Skip((request.Page - 1) * request.Limit).Take(request.Limit).ToList() : querySearch.ToList(),
                Info = new Info
                {
                    Page = request.Page != 0 ? request.Page : 1,
                    Limit = request.Page != 0 ? request.Limit : querySearch.Count(),
                    TotalRecord = querySearch.Count(),
                }
            };
        }

        public Course CreateDefault()
        {
            Course course = new Course
            {
                CourseId = "2c88f7c7-345f-4cf3-b013-08f6181a92ab",
                CourseCode = "DF102020",
                CourseName = "Default"
            };
            _repositoryWrapper.Courses.Add(course);
            _repositoryWrapper.SaveChanges();
            return course;
        }

        public Course Store(StoreCourseRequest request)
        {
            Course course = Mapper.Map<Course>(request);
            _repositoryWrapper.Courses.Add(course);
            _repositoryWrapper.SaveChanges();
            return course;
        }

        public Course Update(UpdateCourseRequest request, Course course)
        {
            course = request.MergeData<Course>(course);
            _repositoryWrapper.Courses.UpdateMaria(course);
            _repositoryWrapper.SaveChanges();
            return course;
        }
    }
}
