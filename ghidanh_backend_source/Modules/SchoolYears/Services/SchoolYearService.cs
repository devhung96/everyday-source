using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.DesignPatterns.Repositories;
using Project.Modules.SchoolYears.Entities;
using Project.Modules.SchoolYears.Extension;
using Project.Modules.SchoolYears.Requests;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Project.Modules.SchoolYears.Services
{
    public interface ISchoolYearService
    {
        Entities.SchoolYear Insert(Entities.SchoolYear schoolYear);
        void Delete(Entities.SchoolYear schoolYears);
        List<Entities.SchoolYear> GetAll();
        Entities.SchoolYear GetById(string SchoolYearId);
        Entities.SchoolYear Update(Entities.SchoolYear schoolYears, UpdateSchoolYearRequest updateSchoolYearRequest);
    }
    public class SchoolYearService : ISchoolYearService
    {
        public readonly IRepositoryMariaWrapper RepositoryWrapper;
        public readonly IConfiguration Configuration;
        public SchoolYearService(IRepositoryMariaWrapper repositoryWrapper, IConfiguration configuration)
        {
            RepositoryWrapper = repositoryWrapper;
            Configuration = configuration;
        }

        public Entities.SchoolYear Insert(Entities.SchoolYear schoolYear)
        {
            RepositoryWrapper.SchoolYears.Add(schoolYear);
            RepositoryWrapper.SaveChanges();
            return schoolYear;
        }

        public void Delete(Entities.SchoolYear schoolYears)
        {
            RepositoryWrapper.SchoolYears.RemoveMaria(schoolYears);
            RepositoryWrapper.SaveChanges();
        }
        public List<Entities.SchoolYear> GetAll()
        {
            return RepositoryWrapper
                .SchoolYears
                .FindAll()
                .Include(x => x.Semesters)
                .ToList();
        }
        public Entities.SchoolYear GetById(string SchoolYearId)
        {
            return RepositoryWrapper
                .SchoolYears
                .FindAll()
                .Include(x => x.Semesters)
                .Where(x => x.SchoolYearId.Equals(SchoolYearId))
                .FirstOrDefault();
        }

        public Entities.SchoolYear Update(Entities.SchoolYear schoolYears, UpdateSchoolYearRequest updateSchoolYearRequest)
        {
            schoolYears = updateSchoolYearRequest.MergeData(schoolYears);
            //if (!schoolYears.CheckConditionSchoolYear(true))
            //{
            //    return null;
            //}
            RepositoryWrapper.SchoolYears.UpdateMaria(schoolYears);
            RepositoryWrapper.SaveChanges();
            return schoolYears;
        }
    }
}
