using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.SettlementReports.Entities
{
    public class Report
    {
        public List<SettlementReport> SettlementReports { get; set; }
        public int TongTonDauKy { get; set; }
        public int TongNhapTrongKy { get; set; }
        public int TongXuatTrongKy { get; set; }
        public int TongTonCuoiKy { get; set; }
    }
}
