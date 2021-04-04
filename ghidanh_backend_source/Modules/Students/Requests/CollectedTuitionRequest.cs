using Project.Modules.Students.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Students.Requests
{
    public class CollectedTuitionRequest
    {
        [Required]
        public string StudentId { get; set; }
        [Required]
        public string ClassId { get; set; }
        [Required]
        public EnumTuition RegistrationTuition { get; set; }
        public double TypeAmount { get; set; } // số tiền theo loại
        public string MoneyTypeId { get; set; }// loại thu
        public double SurchargeAmount { get; set; } // tiền phụ thu
        public double DiscountAmount { get; set; } // tiền giảm giá
        public string DiscountNote { get; set; } // nội dung giảm giá
        public double Amount { get; set; } // Tổng thu
    }
}
