using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.SettlementReports.Entities
{
    public class SettlementReport
    {
        public string nameProduct { get; set; } // tên sản phẩm
        public string ProductNumber { get; set; } // mã hs
        public string codeProduct { get; set; }// mã sản phẩm
        public string unit { get; set; }// đơn vị
        public int tonDauKy { get; set; }
        public int nhapTrongKy { get; set; }
        public int xuatTrongKy { get; set; }
        public int tonCuoiKy { get; set; }
        public string TaxCode { get; set; } // mã số thuế
        public DateTime NamBC { get; set; }// năm báo cáo

    }
}
