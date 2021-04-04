using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Receipts.Entities;
using Project.Modules.Receipts.Requests;
using Project.Modules.Receipts.Services;
using Project.Modules.Reports.Models;

namespace Project.Modules.Receipts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptController : BaseController
    {
        private readonly IReceiptService receiptService;
        public ReceiptController(IReceiptService receipt)
        {
            receiptService = receipt;
        }
        [HttpPost("invoice")]
        public IActionResult Invoice([FromBody] ReceiptRequest request)
        {
            (object receipt, string message) = receiptService.Export(request);
            return receipt == null ? ResponseBadRequest(message) : ResponseOk(receipt, message);
        }
        [HttpGet("{amount}")]
        public IActionResult Test(string amount )
        {
            
            return ResponseOk(receiptService.NumberToText(amount.Trim()));
        }
        [HttpPost("export")]
        public IActionResult Export([FromBody] ReceiptExportRequest request)
        {
            return ResponseOk(receiptService.ReceiptExport(request));
        }
        [HttpGet("hihi")]
        public IActionResult actionResult()
        {
            var aaa = ConvertNumberToString.ChuyenSoSangChuoi2(1000000000520);
            return Ok(aaa);
        }
    }
}
