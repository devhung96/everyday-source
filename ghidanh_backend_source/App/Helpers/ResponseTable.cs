using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Helpers
{
    public class ResponseTable
    {
        public object Data { get; set; } // Dữ liệu trả ra
        public Info Info { get; set; } // Thông tin của trang
    }

    public class Info
    {
        public int Limit { get; set; }  // Số item trong 1 trang
        public int Page { get; set; } // Số trang
        public int TotalRecord { get; set; } // Tổng số phần tử
        public double EmployeeSalaryTotalAll { get; set; } 
    }
}
