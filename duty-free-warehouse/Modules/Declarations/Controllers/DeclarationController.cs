using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Declarations.Requests;
using Project.Modules.Declarations.Services;
using Project.Modules.Declarations.Validatations;
using System;

namespace Project.Modules.Declarations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeclarationController : BaseController
    {
        private readonly IDeClarationServices clarationServices;
        public DeclarationController(IDeClarationServices clarationServices)
        {
            this.clarationServices = clarationServices;
        }
        [HttpPost("store")]
        public IActionResult Store(DeclarationStore request)
        {
            int userID;
            int.TryParse(User.FindFirst("UserID")?.Value, out userID);
            (object data, string message) result = clarationServices.Store(request, userID);
            if (!String.IsNullOrEmpty(result.message))
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data,"Tạo tờ khai thành công !");
        }
        [HttpGet("showall")]
        public IActionResult ShowAll()
        {
            object result = clarationServices.ShowAll();
            return ResponseOk(result,"Danh sách tờ khai");
        }
        [HttpGet("showNumber/{DeclarationNumber}")]
        public IActionResult ShowNumber(string DeclarationNumber)
        {
            (object data, string message) result = clarationServices.ShowID(DeclarationNumber);
            if (!String.IsNullOrEmpty(result.message))
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, "Thành công");
        }
        [HttpPost("filter")]
        public IActionResult FilterDeclaration([FromBody] SortDeclaration request)
        {
            var result = clarationServices.Filter(request);
            return ResponseOk(result);
        }
        [HttpPost("export-report")]
        public IActionResult ExportReportN2([FromBody] FilterExportDeclaration filterExport)
        {
            int userID;
            int.TryParse(User.FindFirst("UserID")?.Value, out userID);
            var result = clarationServices.ExportFilter(filterExport, userID);
            return ResponseOk(result);
        }

        [HttpPost("export-x2")]
        public IActionResult ExportReportX2([FromBody] FilterExportDeclaration filterExport)
        {
            var result = clarationServices.ExportDeclarationTypeX2(filterExport,2);
            return ResponseOk(result);
        }

        [HttpPost("export-x4")]
        public IActionResult ExportReportX4([FromBody] FilterExportDeclaration filterExport)
        {
            var result = clarationServices.ExportDeclarationTypeX4(filterExport,1);
            return ResponseOk(result);
        }

        [HttpPost("export-x5")]
        public IActionResult ExportReportX5([FromBody] FilterExportDeclaration filterExport)
        {
            var result = clarationServices.ExportDeclarationTypeX5(filterExport);
            return ResponseOk(result);
        }

        [HttpPost("report")]
        public IActionResult Report([FromBody] Report request)
        {
            var result = clarationServices.Report(request);
            return ResponseOk(result);
        }
        [HttpPost("report-import")]
        public IActionResult ReportImport([FromBody] Report request)
        {
            var result = clarationServices.ListReportImport(request);
            return ResponseOk(result);
        }
        [HttpPost("track-liquidity")]
        public IActionResult TrackLiquidity([FromBody] TrackLiquidityRequest request)
        {
            var result = clarationServices.TrackLiquidity(request); 
            return ResponseOk(result);
        }
        [HttpPost("confirm-settlement")]
        public IActionResult ConfirmSettlement([FromBody] ConfirmDispatch request)
        {
            var result = clarationServices.ConfirmSettlement(request.DeclaNumber,request.DispatchNumber);
            if (!result.flag)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.message);    
        }
        [HttpPost("dispatch-extend")]
        public IActionResult DispatchExtend([FromBody] DispatchExtend request)
        {
            var result = clarationServices.DispatchExtend(request.DeclaNumber, request.DispatchNumber, request.DeClaRenewalDateData, request.DispatchDateData);
            if (!result.flag)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.message);
        }
        [HttpPost("warehouseReceipt")]
        public IActionResult WareHouseReceipt([FromBody] WarehouseReceiptRequest request)
        {
            var result = clarationServices.WarehouseReceipt(request);
            return ResponseOk(result, "Success");
        }
        [HttpPost("deliveryReceip")]
        public IActionResult DeliveryReceipt([FromBody] WarehouseReceiptRequest request) 
        {
            var result = clarationServices.DeliveryReceipt(request);
            return ResponseOk(result, "Success");
        }
        [HttpPost("extensionDispatchReport")]
        public IActionResult ExtensionDispatchReport([FromBody] WarehouseReceiptRequest request)
        {
            var result = clarationServices.ExtensionDispatchReport(request);
            return ResponseOk(result, "Success");
        }
    }
}