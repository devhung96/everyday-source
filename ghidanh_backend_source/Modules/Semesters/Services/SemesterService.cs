using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.WireProtocol.Messages;
using Project.App.DesignPatterns.Repositories;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.Semesters.Entities;
using Project.Modules.Semesters.Requests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Semesters.Services
{
    public interface ISemesterService
    {
        List<Semester> GetAll();
        Semester Insert(Semester semester);
        Semester GetById(string semesterId);
        void Delete(Semester semester);
        Semester Update(Semester semester, UpdateSemesterRequest updateSemester);
    }
    public class SemesterService : ISemesterService
    {
        public readonly IRepositoryMariaWrapper RepositoryMaria;
        public readonly IRepositoryMongoWrapper RepositoryMongoWrapper;
        public readonly IConfiguration Configuration; 
        public SemesterService(IRepositoryMariaWrapper repositoryMaria, IConfiguration configuration, IRepositoryMongoWrapper repositoryMongoWrapper)
        {
            RepositoryMaria = repositoryMaria;
            Configuration = configuration;
            RepositoryMongoWrapper = repositoryMongoWrapper;
        }

        public Semester Insert(Semester semester)
        {
            RepositoryMaria.Semesters.Add(semester);
            RepositoryMaria.SaveChanges();
            return semester;
        }

        public List<Semester> GetAll()
        {
            return RepositoryMaria.Semesters
                .FindAll()
                .ToList();
        }

        public Semester GetById(string semesterId)
        {
            return RepositoryMaria.Semesters
                //.FindAll()
                .FirstOrDefault(x => x.SemesterId.Equals(semesterId));
        }

        public void Delete(Semester semester)
        {
            RepositoryMaria.Semesters.RemoveMaria(semester);
            RepositoryMaria.SaveChanges();
        }

        public Semester Update(Semester semester, UpdateSemesterRequest updateSemester)
        {
            Semester semesterUpdate = new Semester
            {
                SemesterId = semester.SemesterId,
                SemesterName = updateSemester.SemesterName,
                TimeEnd = DateTime.ParseExact(updateSemester.TimeEnd, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                TimeStart = DateTime.ParseExact(updateSemester.TimeStart, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                SchoolYearId = updateSemester.SchoolYearId
            };
            semester = semesterUpdate.MergeData(semester);
            //if (!semester.CheckConditionSemester(true))
            //{
            //    return null;
            //}
            RepositoryMaria.Semesters.UpdateMaria(semester);
            RepositoryMaria.SaveChanges();
            return semester;
        }
    }
}
