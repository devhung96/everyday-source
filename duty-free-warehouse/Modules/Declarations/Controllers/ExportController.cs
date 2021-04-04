using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Declarations.Requests;
using Project.Modules.Declarations.Services;

namespace Project.Modules.Declarations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : BaseController
    {
        private readonly IExportService service;
        public ExportController(IExportService exportService)
        {
            this.service = exportService;
        }
        [HttpPost("N1")]
        public IActionResult N1 ([FromBody] ExportRequest request)
        {
            (List<ExportReponse> inventoryN1s, string message) = service.ExportN1(request);
            return ResponseOk(inventoryN1s,message);
        } 
        [HttpPost("X1")]
        public IActionResult X1 ([FromBody] ExportRequest request)
        {
            (List<ExportReponse> inventoryN1s, string message) = service.ExportX1(request);
            return ResponseOk(inventoryN1s,message);
        }

    }
}
