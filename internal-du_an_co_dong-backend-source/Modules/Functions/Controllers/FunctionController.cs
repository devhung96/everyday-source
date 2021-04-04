using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Functions.Requests;
using Project.Modules.Functions.Services;
using Project.Modules.Question.Validation;

namespace Project.Modules.Functions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionController : BaseController
    {
        private readonly IFunctionService functionService;
        public FunctionController(IFunctionService functionService)
        {
            this.functionService = functionService;
        }
        [HttpGet("show-all-permission-by-function")]
        public IActionResult ShowAllPermissionByFunction()
        {
            var result = functionService.ShowAllFunction();
            return ResponseOk(result, "Show all permission by funtion suscess.");
        }

      //  [DisplayNameAttribute(Modules = 8, Level = 2)]
        [HttpGet("show-all-permisson")]
        public IActionResult ShowAllPermisson()
        {
            var result = functionService.ShowAllPermisson();
            return ResponseOk(result, "Xuất danh sách quyền thành công.");
        }

    //    [DisplayNameAttribute(Modules = 8, Level = 2)]
        [HttpGet("show-all-permisson-by-group/{idGroup}")]
        public IActionResult ShowAllPermisson(int idGroup)
        {
            var result = functionService.ShowAllPermissonByGroup(idGroup);
            if (result is null)
            {
                return ResponseBadRequest("Vai trò không tồn tại.");
            }    
            return ResponseOk(result, "Xuất danh sách quyền theo vai trò thành công.");
        }


        //[HttpPost("add-permisson-group")]
        //public IActionResult AddPermissonToUser([FromBody] AddPermissonToUserRequest request)
        //{

        //}





    }
}