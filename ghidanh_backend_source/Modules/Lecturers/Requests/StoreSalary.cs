using Project.Modules.Lecturers.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Lecturers.Requests
{
    [ValidateStoreSalary]
    public class StoreSalary
    {
        public string EmployeeSalaryId { get; set; }
        [Required]
        public double? BaseSalary { get; set; } // Tiền cơ bản của 1 lớp (tổng tiền học sinh đóng trong lớp đó)
        [Required]
        public string CourseId { get; set; } // Khóa đào tạo
        [Required]
        public string LectureId { get; set; }
        [Required]
        public double? Percent { get; set; } // Phần trăm tiền dc nhận 
        [Required]
        public double? TotalSalary { get; set; } // Tổng tiền chưa cộng trợ cấp
        [Required]
        public double? TotalSalarySubsidiesOther { get; set; } // Tổng tiền trợ cấp khác
        public string Note { get; set; }
        public string TotalSalarySubsidiesOtherName { get; set; }
    }
}
