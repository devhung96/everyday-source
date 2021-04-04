using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Project.App.Databases;
using Project.Modules.SchoolYears.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Semesters.Validation
{
    public class BaseParamsSemester
    {
        public string SemesterName { get; set; }
        public string SchoolYearId { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
    }
    public class CheckInsertUpdateSemester : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("RequestFail");
            }
            BaseParamsSemester baseParams = JsonConvert.DeserializeObject<BaseParamsSemester>(JsonConvert.SerializeObject(value));
            if (baseParams.SchoolYearId is null)
            {
                return new ValidationResult("SchoolYearidRequired");
            }
            if (baseParams.SemesterName is null)
            {
                return new ValidationResult("SemesterNameRequired");
            }
            //DateTime
            DateTime TimeStart;
            DateTime TimeEnd; 
            if (!DateTime.TryParseExact(baseParams.TimeStart, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out TimeStart))
            {
                return new ValidationResult("TimeStartNotValid");
            }
            if (!DateTime.TryParseExact(baseParams.TimeEnd, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out TimeEnd))
            {
                return new ValidationResult("TimeStartNotValid");
            }

            //DateTime TimeStart = DateTime.Parse(dateTime);
            //DateTime TimeEnd = DateTime.Parse(dateTime2);
            if (TimeEnd < TimeStart)
            {
                return new ValidationResult("TimeEndNotValid");
            }

            string SchoolYearId = baseParams.SchoolYearId.ToString();
            MariaDBContext mariaDBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            SchoolYear schoolYears = mariaDBContext.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(SchoolYearId));
            if (schoolYears is null)
            {
                return new ValidationResult("SchoolYearNotFound");
            }

            //check nien khoa
            if (schoolYears.TimeStart > TimeStart.Year && schoolYears.TimeEnd < TimeStart.Year)
            {
                return new ValidationResult("TimeStartInValid");
            }
            if (schoolYears.TimeStart > TimeEnd.Year && schoolYears.TimeEnd < TimeEnd.Year)
            {
                return new ValidationResult("TimeStartInValid");
            }

            
            return ValidationResult.Success;

        }
    }
}
