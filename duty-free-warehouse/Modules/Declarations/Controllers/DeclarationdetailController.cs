using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Declarations.Requests;
using Project.Modules.Declarations.Services;
using Project.Modules.DeClarations.Entites;

namespace Project.Modules.Declarations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeclarationdetailController : BaseController
    {
        private readonly IDeClarationDetailServices deClarationDetailServices;
        public DeclarationdetailController(IDeClarationDetailServices deClarationDetailServices)
        {
            this.deClarationDetailServices = deClarationDetailServices;
        }
        [HttpPost("store-import")]
        public IActionResult StoreImport([FromBody] StoreDetail request)
        {
            (object data, string message) result = deClarationDetailServices.StoreImport(request);
            if (!String.IsNullOrEmpty(result.message))
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, "Tạo chi tiết tờ khai nhập thành công !");
        }
        [HttpPost("storeandsave-import")]
        public IActionResult StoreAndSaveImport([FromBody] StoreDetail request)
        {
            (object data, string message) result = deClarationDetailServices.StoreAndSaveInventoryImport(request);
            if (!String.IsNullOrEmpty(result.message))
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, "Nhập kho thành công !");
        }
        [HttpPost("store-export")]
        public IActionResult StoreExport([FromBody] StoreExportDetail request)
        {
            (List<DeClarationDetail> data, string message) result = deClarationDetailServices.StoreExport(request);
            if (!String.IsNullOrEmpty(result.message))
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, "Tạo chi tiết tờ khai xuất thành công !");
        }
        [HttpPost("storeandsave-export")]
        public IActionResult StoreAndSaveExport([FromBody] StoreExportDetail request)
        {
            (object data, string message) result = deClarationDetailServices.StoreAndSaveInventoryExport(request);
            if (!String.IsNullOrEmpty(result.message))
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data);
        }
        [HttpGet("showProductImport/{DeclarationNumberImport}")]
        public IActionResult ShowProductImport(string DeclarationNumberImport)
        {
            (object data, string message) result = deClarationDetailServices.ShowProductFollowImport(DeclarationNumberImport);
            if (!String.IsNullOrEmpty(result.message))
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, "Xuất kho thành công !");
        }
        [HttpGet("showProductInventory/{DeclarationNumber}/{ProductCode}")]
        public IActionResult ShowProductInventory(string DeclarationNumber,string ProductCode)
        {
            var result = deClarationDetailServices.ShowProductInInventory(DeclarationNumber, ProductCode);
            return ResponseOk(result, "Thành công !");
        }
    }
}