using Newtonsoft.Json;
using Project.App.Databases;
using Project.Modules.SchoolYears.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.SchoolYears.Validation
{
    public class SchoolYearTime
    {
        public int? TimeStart { get; set; }
        public int? TimeEnd { get; set; }

    }
    public class YearValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("ObjectRequired");
            }
            SchoolYearTime time = JsonConvert.DeserializeObject<SchoolYearTime>(JsonConvert.SerializeObject(value));
            //SchoolYearTime time = (SchoolYearTime)value;
            if (time.TimeStart is null || time.TimeEnd is null)
            {
                return new ValidationResult("FieldRequired");
            }
            if (time.TimeEnd < time.TimeStart)
            {
                return new ValidationResult("TimeInvalid");
            }
            if (time.TimeEnd < 0 || time.TimeStart < 0)
            {
                return new ValidationResult("TimeNotLessThan0");
            }
            MariaDBContext mariaDBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            List<SchoolYear> schoolYears = mariaDBContext.SchoolYears.ToList();
            Entities.SchoolYear schoolYear = Enumerable.FirstOrDefault<Entities.SchoolYear>(schoolYears, (Func<Entities.SchoolYear, bool>)(x => (bool)(x.TimeStart == time.TimeStart && x.TimeEnd == time.TimeEnd)));
            if (schoolYear != null)
            {
                return new ValidationResult("SchoolYearIsExists");
            }
            //if (schoolYears.Any(x => x.TimeStart < time.TimeStart) && schoolYears.Any(x => x.TimeEnd > time.TimeStart))
            //{
            //    return new ValidationResult("TimeStartInValid");
            //}
            //if (schoolYears.Any(x => x.TimeStart < time.TimeEnd) && schoolYears.Any(x => x.TimeEnd > time.TimeEnd))
            //{
            //    return new ValidationResult("TimeEndInValid");
            //}

            return ValidationResult.Success;
        }
    }
}
