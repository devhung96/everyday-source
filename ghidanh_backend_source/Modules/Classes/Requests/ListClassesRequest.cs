using Project.App.Helpers;
using Project.Modules.Classes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Classes.Requests
{
    [ValidationListClasses]
    public class ListClassesRequest : RequestTable
    {
        [Required]
        public int Type { get; set; }
        public string ClassCode { get; set; }
        public string ClassName { get; set; }
        public string CourseID { get; set; }
        public string SchoolYearID { get; set; }
    }
}
