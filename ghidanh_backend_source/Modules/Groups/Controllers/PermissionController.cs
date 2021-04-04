using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Groups.Sevices;

namespace Project.Modules.Groups.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : BaseController
    {
        private readonly IGroupService groupService;
        public PermissionController(IGroupService groupService)
        {
            this.groupService = groupService;
        }
        [HttpGet("showAll")]
        public IActionResult GetAll()
        {
            return ResponseOk(groupService.GetAllPermissions());
        }
        [HttpGet("module")]
        public IActionResult ShowAllModule ()
        {
            return ResponseOk(groupService.GetAllModule());
        }
    }
}
