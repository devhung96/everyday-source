using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Requests;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api/management-account-admin")]
    [ApiController]
    public class ManagementAccountAdminController : BaseController
    {

        private readonly IManagementAccountAdminService _managementAccountAdminService;
        private readonly IMapper _mapper;
        private readonly IForgotPasswordService forgotPasswordService;


        public ManagementAccountAdminController(IManagementAccountAdminService managementAccountAdminService, IMapper mapper, IForgotPasswordService forgotPasswordService)
        {
            _managementAccountAdminService = managementAccountAdminService;
            _mapper = mapper;
            this.forgotPasswordService = forgotPasswordService;
        }

        [HttpPost("show-all")]
        public IActionResult ShowAll([FromBody] RequestTable requestTable)
        {
            (List<UserSuper> data, string message) = _managementAccountAdminService.ShowAllUserAdmin();

            #region Paganation
            data = data.Where(x =>
                string.IsNullOrEmpty(requestTable.Search) ||
                (!string.IsNullOrEmpty(requestTable.Search) &&
                    (
                        x.Email.ToLower().Contains(requestTable.Search.ToLower()) ||
                        x.FullName.ToLower().Contains(requestTable.Search.ToLower()) 
                ))).ToList();

            ResponseTable responseTable = new ResponseTable()
            {
                Info = new Info()
                {
                    Page = requestTable.Page,
                    TotalRecord = data.Count,
                    Results = requestTable.Results
                }
            };
            if(requestTable.Page == -1)
            {
                responseTable.DateResult = data.ToList();
            }
            else
            {
                responseTable.DateResult = data
                                              .Skip((requestTable.Page - 1) * requestTable.Results)
                                              .Take(requestTable.Results)
                                              .ToList();
            }
           
            #endregion
            return Ok(responseTable);

        }


        [HttpGet("{userId}")]
        public IActionResult Show(string userId )
        {
            //
            return Ok();
        }
                     
        [HttpPost]
        public IActionResult CreateUserAdmin([FromBody] CreateUserAdminRequest request)
        {
            UserSuper user =_mapper.Map<UserSuper>(request);
            (UserSuper result, string message)  = _managementAccountAdminService.CreateUserAdmin(user);

            if(result is null)
            {
                return ResponseBadRequest(message);
            }    
            return Ok(result);
        }


        [HttpPut("{userId}")]
        public IActionResult UpdateUserAdmin([FromBody] UpdateUserAdminRequest request , string userId)
        {
            var (user, message) = _managementAccountAdminService.EditAdmin(request, userId);
            if(user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user,message);
        }

        [HttpPut("reset-password/{userSuperId}")]
        public IActionResult Reset (string userSuperId)
        {
            (UserSuper user, string message)  = _managementAccountAdminService.ResetAdmin(userSuperId);

            if(user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user.FullName, message);

        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteUserAdmin(string userId)
        {
            var (user, message) = _managementAccountAdminService.DeleteAdmin(userId);
            if(user is null)
            {
                return ResponseBadRequest(message);
            }    
            return ResponseOk(user,message);
        }

        [HttpPost("forgot-password")]
        public IActionResult Forgot([FromBody] RequestForgotAdmin request)
        {
              (object user, string message) = forgotPasswordService.CheckEmailAdmin(request);
            if(user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);
        }
        [HttpPut("update-password/{key}")]
        public IActionResult UpdatePassword(string key, [FromBody] ForgotPasswordRequest request)
        {
            (object user, string message)  = forgotPasswordService.UpdatePasswordAdmin(request, key);
            if( user is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(user, message);
        }
    }
}