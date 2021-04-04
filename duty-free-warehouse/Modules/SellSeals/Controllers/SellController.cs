using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;

namespace Project.Modules.SellSeals.Controllers
{
    [Route("api/invoice")]
    [ApiController]
    public class SellController : BaseController
    {
        private readonly ISellSealSerivce sellSealSerivce;
        public SellController(ISellSealSerivce _sellSealSerivce)
        {
            sellSealSerivce = _sellSealSerivce;
        }

        [HttpGet]
        public IActionResult CreateNewUser([FromQuery] string sealNo)
        {
            var result = sellSealSerivce.GetInvoicesWithSealNo(sealNo);
            if (result.data is null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

        [HttpDelete("delete")]
        public IActionResult RemoveInvoice([FromQuery] long invoiceId)
        {
            var result = sellSealSerivce.RemoveInvoice(invoiceId);
            if (result.data is null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

        [HttpPut("update")]
        public IActionResult UpdateInvoice([FromQuery] long invoiceId, [FromBody] Invoice invoice)
        {
            var result = sellSealSerivce.UpdateInvoice(invoiceId,invoice);
            if (result.data is null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }


        [HttpPost("add")]
        public IActionResult AddNewInvoice([FromQuery] string sealNo,[FromBody] Invoice invoice)
        {
            var result = sellSealSerivce.AddNewInvoice(sealNo, invoice);
            if (result.data is null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

    }
}