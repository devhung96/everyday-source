using Project.Modules.SchoolYears.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.SchoolYears.Requests
{
    //[YearValidation]
    public class UpdateSchoolYearRequest
    {
        [Required]
        public string SchoolYearName { get; set; }
        [Required]
        public int? TimeStart { get; set; }
        [Required]
        public int? TimeEnd { get; set; }
    }
}
