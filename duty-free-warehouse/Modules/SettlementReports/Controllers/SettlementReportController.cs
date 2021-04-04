using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Project.App.Controllers;
using Project.App.Requests;
using Project.Modules.DeClarations.Entites;
using Project.Modules.SettlementReports.Entities;
using Project.Modules.SettlementReports.Requests;
using Project.Modules.SettlementReports.Services;

namespace Project.Modules.SettlementReports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettlementReportController : BaseController
    {
        private readonly ISettlementReportService _ISettlementReport;
        public SettlementReportController(ISettlementReportService ISettlementReport)
        {
            _ISettlementReport = ISettlementReport;
        }
        [HttpPost]
        public IActionResult showAll(InputFiterRequest inputFiter)
        {
            (DataPaginationResponse data, string message) = _ISettlementReport.showAll(inputFiter);
            return ResponseOk(data, message);
        }

    }
}