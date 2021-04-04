using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Middleware;
using Project.Modules.Users.Request;
using Project.Modules.Users.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [MiddlewareFilter(typeof(CheckTokenMiddleware))]
    public class DepartmentController : BaseController
    {
        private readonly IDepartmentService departmentService;
        public DepartmentController(IDepartmentService _departmentService)
        {
            departmentService = _departmentService;
        }

        [HttpPost("add")]
        public IActionResult AddGroup([FromBody] AddDepartmentRequest addGroupRequest)
        {
            (object data, string message) result = departmentService.CreateDepartment(addGroupRequest);
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }


        [HttpGet("users")]
        public IActionResult GetUsersGroup()
        {
            (object data, string message) result = departmentService.GetUserDepartment();
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }


        [HttpPost("edit")]
        public IActionResult EditDepartment([FromQuery] string departmentCode,[FromBody] AddDepartmentRequest addGroupRequest)
        {
            (object data, string message) result = departmentService.UpdateDepartment(departmentCode,addGroupRequest);
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }

        [HttpDelete("delete")]
        public IActionResult DeleteDepartment([FromQuery] string departmentCode)
        {
            (object data, string message) result = departmentService.RemoveDepartment(departmentCode);
            if (result.data == null)
                return ResponseBadRequest(result.message);
            return ResponseOk(result.data, result.message);
        }
    }
}