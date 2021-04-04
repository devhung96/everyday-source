using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Lecturers.Requests
{
    public class ShowAllEmployeeSalary : RequestTable
    {
        [Required(ErrorMessage = "CourseIdIsRequired")]
        public string CourseId { get; set; }
    }
}
