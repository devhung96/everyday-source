//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Project.App.Controllers;
//using Project.App.Helpers;
//using Project.App.Middleware;
//using Project.Modules.PermissonUsers.Requests;
//using Project.Modules.PermissonUsers.Services;

//namespace Project.Modules.PermissonUsers.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    //[MiddlewareFilter(typeof(CheckTokenMiddleware))]
//    //[Authorize(Roles = "ADMINISTRATOR_SYSTEM")]
//    public class PermissionUserController : BaseController
//    {
//        private readonly IPerUserServices perUserServices;
//        public PermissionUserController(IPerUserServices _perUserServices)
//        {
//            perUserServices = _perUserServices;
//        }
//        [HttpPost("AddPermissionUser")]
//        public IActionResult AddPermissionUser([FromBody]AddPerUserRequest data)
//        {
//            if (perUserServices.TextPermission(data))
//                return ResponseBadRequest("Người dùng đã tồn tại quyền");
//            PermissionUser peruser =perUserServices.Save(data);
//            return ResponseOk(new {peruser},"Add permission of user success.");
//        }
//        [HttpDelete("DeletePermission/{PermissionUserID}")]
//        public IActionResult DeletePermission(int PermissionUserID)
//        {
//            PermissionUser peruser = perUserServices.Delete(PermissionUserID);
//            return ResponseOk(new { peruser }, "Delete permission of user success.");
//        }
//        [HttpGet("PermisionOfUser/{UserID}")]
//        public IActionResult GetListOfUser(int UserID)
//        {
//            List<PermissionUser> ListOfUser = perUserServices.List(UserID);
//            return ResponseOk(new { ListOfUser });
//        }
//        [HttpGet]
//        public IActionResult GetData()
//        {
//           // int userID = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            List<PermissionUser> ListData = perUserServices.ListData();
//            return ResponseOk(new { ListData });
//        }
//    }
//}