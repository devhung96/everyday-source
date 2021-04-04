using Project.Modules.Lecturers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Lecturers.Models
{
    public class ResponseEmployeeSalaryBill
    {
        public Lecturer Lecturer { get; set; }
        public double BaseSalary { get; set; }
        public string SalaryBillNumber { get; set; }
        public EmployeeSalary EmployeeSalary { get; set; }
    }
}
