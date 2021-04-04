using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Classes.Requests
{
    public class GetClassRegisterStudent
    {
        [Required]
        public string StudentID { get; set; }
        public string CourseID { get; set; }
        public string SchoolYearID { get; set; }
    }
}
