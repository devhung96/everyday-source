using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.Destroys.Entities;
using Project.Modules.Destroys.Requests;
using Project.Modules.Destroys.Services;

namespace Project.Modules.Destroys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DestroyController : BaseController
    {
        private readonly IDestroyService _destroyService;
        public DestroyController(IDestroyService destroyService)
        {
            _destroyService = destroyService;
        }


        [HttpGet]
        public IActionResult ShowAll([FromQuery] RequestTableShowAllDestroy requestTableShowAllDestroy)
        {
           (object data, string message) = _destroyService.ShowAll(requestTableShowAllDestroy);
            return Ok(data);
        }

        [HttpGet("confirm/{destroyId}")]
        public IActionResult ConfirmDestroy(int destroyId)
        {
            (Destroy data, string message) = _destroyService.Confirm(destroyId);
            if (data == null) return ResponseBadRequest(message);
            return ResponseOk(data, message);
        }


        [HttpGet("report-inventory-export")]
        public IActionResult BaoCaoTonKhoXuatFileExcel([FromQuery] RequestReportInventory  input)
        {
            (object data, string message) = _destroyService.BaoCaoTonKhoTheoNgay(input.search);
            return ResponseOk(data);
        }


        [HttpGet("report-inventory")]
        public IActionResult BaoCaoTonKho([FromQuery] RequestTable requestTable)
        {
            (object data, string message) = _destroyService.BaoCaoTonKhoTheoNgay(requestTable);
            return ResponseOk(data);
        }
        




    }
}