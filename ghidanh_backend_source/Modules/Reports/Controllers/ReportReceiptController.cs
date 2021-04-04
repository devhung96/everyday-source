using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Reports.Models;
using Project.Modules.Reports.Requests;
using Project.Modules.Reports.Services;
using Project.Modules.SubjectGroups.Responses;
using Project.Modules.SubjectGroups.Services;

namespace Project.Modules.Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportReceiptController : BaseController
    {
        private readonly IReceiptDetailService _receiptDetailService;
       
        private readonly IReportReceiptService reportReceiptService;

        private readonly ISubjectGroupService SubjectGroupService;
        public ReportReceiptController(IReportReceiptService reportReceiptService, IReceiptDetailService receiptDetailService, ISubjectGroupService subjectGroupService)
        {
            this.reportReceiptService = reportReceiptService;
            _receiptDetailService = receiptDetailService;
            SubjectGroupService = subjectGroupService;
        }
        [HttpPost("exportReceipt")]
        public IActionResult ExportReceipt([FromBody] ExportReceipt request)
        {
            DateTime dateTimeData;
            DateTime.TryParseExact(request.DateTime, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dateTimeData);
            request.DateTimeData = dateTimeData;
            // An response
            var reportReceipt = reportReceiptService.ReportReceipt(request.DateTimeData);
            // a Hùng reponse
            var reportReceiptDetail = _receiptDetailService.GetReceiptDetail(request.DateTimeData.ToUniversalTime()); 
            // a Hoàn Anh response
            List<ExportReportMonth> reportMonths = SubjectGroupService.ExportReport(request);

            Report report = new Report()
            {
                ReportReceipt = reportReceipt,
                ReportReceiptDetail = reportReceiptDetail,
                ExportReportMonths = reportMonths
            };
            return ResponseOk(report,"Success");
        }
    }
}