using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Seals.Request;
using Project.Modules.Seals.Requests;
using Project.Modules.Seals.Services;
using System;

namespace Project.Modules.Seals.Controllers
{
    [Route("api/report")]
    [ApiController]
    public class ReportExportController : BaseController
    {
        public readonly IReportExportService reportExportService;
        public ReportExportController(IReportExportService ReportExportService)
        {
            reportExportService = ReportExportService;
        }
        [HttpPost("export-product")]
        public IActionResult ExportProduct([FromBody]ReportExportRequest request)
        {
            return Ok(reportExportService.ShowData(request));      
        }

        [HttpPost("WarehouseToAircraftX11")]
        public IActionResult ReportWarehouseToAircraftX12([FromBody]ReportX11X12X5Request request)
        {
            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            if (!string.IsNullOrEmpty(request.DateFrom) && !string.IsNullOrEmpty(request.DateTo))
            {
                dateFrom = DateTime.ParseExact(request.DateFrom, "dd/MM/yyyy", null);
                dateTo = DateTime.ParseExact(request.DateTo, "dd/MM/yyyy", null);
            }
            return ResponseOk(reportExportService.ReportX11(dateFrom, dateTo));
        }

        [HttpPost("AircraftToWarehouseX12")]
        public IActionResult ReportAircraftToWarehouseX12([FromBody] ReportX11X12X5Request request)
        {
            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            if(!string.IsNullOrEmpty(request.DateFrom) && !string.IsNullOrEmpty(request.DateTo))
            {
                dateFrom = DateTime.ParseExact(request.DateFrom, "dd/MM/yyyy", null);
                dateTo = DateTime.ParseExact(request.DateTo, "dd/MM/yyyy", null);
            }
                
            return ResponseOk(reportExportService.ReportX12(dateFrom, dateTo));
        }

        [HttpPost("SellInflight")]
        public IActionResult ReportSellInfligt([FromBody] ReportX11X12X5Request request)
        {
            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            if (!string.IsNullOrEmpty(request.DateFrom) && !string.IsNullOrEmpty(request.DateTo))
            {
                dateFrom = DateTime.ParseExact(request.DateFrom, "dd/MM/yyyy", null);
                dateTo = DateTime.ParseExact(request.DateTo, "dd/MM/yyyy", null);
            }

            return ResponseOk(reportExportService.ReportX5(dateFrom, dateTo));
        }
    }
}