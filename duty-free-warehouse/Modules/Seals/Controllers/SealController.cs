using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Project.App.Controllers;
using Project.Modules.Seals.Request;
using Project.Modules.Seals.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SealController : BaseController
    {
        private readonly ISealImportService sealImportService;
        public SealController(ISealImportService SealImportService)
        {
            sealImportService = SealImportService;
        }
        [HttpPost("importCart")]
        public IActionResult ImportCart([FromBody] List<SealImportRequest> request)
        {
            (object data, string message) = sealImportService.Import(request);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(request);
        }

        [HttpPost("changeQuantityExport/{sealNumber}/products/{productCode}")]
        public IActionResult ChangeQuantityExport([FromBody]JObject jObject, string sealNumber, string productCode)
        {
            JToken jToken = jObject["quantity"];
            if(jToken is null)
            {
                return ResponseBadRequest("Lỗi Thiếu Trường: Số Lượng Sản Phẩm");
            }
            int.TryParse(jToken.ToString(), out int quantity);
            if(quantity < 0)
            {
                return ResponseBadRequest("Số Lượng Phải Là Số Và Lớn Hơn 0");
            }
            (int check, string message) = sealImportService.ChangeQuantityExport(quantity, sealNumber, productCode);
            if(check == 0)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(null, message);
        }

        [HttpGet("{sealNumber}")]
        public IActionResult DetailSeal(string sealNumber)
        {
            return ResponseOk(sealImportService.DetailSeal(sealNumber));
        }
    }
}