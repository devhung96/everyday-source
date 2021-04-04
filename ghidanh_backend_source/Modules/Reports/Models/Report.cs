using Project.Modules.Reports.Services;
using Project.Modules.SubjectGroups.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Reports.Models
{
    public class Report
    {
        public ReportReceipt ReportReceipt { get; set; }
        public List<ReceiptDetailSubjectReponse> ReportReceiptDetail { get; set; }
        public List<ExportReportMonth> ExportReportMonths { get; set; }
    }
}
