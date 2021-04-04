
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.Modules.Classes.Requests;
using Project.Modules.Classes.Services;
namespace Project.Modules.Classes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : BaseController
    {
        private readonly IClassService classService;
        public ClassController(IClassService classService)
        {
            this.classService = classService;
            
        }
        [HttpPost("store")]
        public IActionResult Store([FromForm] StoreClassRequest request)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            var result = classService.Store(request, host);
            return ResponseOk(result, "Success");
        }
        [HttpPut("update")]
        public IActionResult Update([FromForm] UpdateClassRequest request)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            var result = classService.Update(request,host);
            return ResponseOk(result, "Success");
        }
        [HttpDelete("{classID}")]
        public IActionResult Delete(string classID)
        {
            var result = classService.Delete(classID);
            if(result.data is null)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.data, result.message);
        }
        [HttpPost("showClasses")]
        public IActionResult ShowClasses([FromBody] ListClassesRequest request)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            var result = classService.ShowClasses(request, host);
            return ResponseOk(result, "Success");
        }
        [HttpGet("{classID}")]
        public IActionResult ShowClassByID(string classID)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            var result = classService.ShowClass(classID, host);
            if(result.data is null)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.data, result.message);
        }
        [HttpPost("showClassesOpen")]
        public IActionResult ShowClassesOpen([FromBody] ListClassesRequest request)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            var result = classService.ShowClassOpen(request, host);
            return ResponseOk(result, "Success");
        }
        [HttpPost("showClassbyRegisterStudent")]
        public IActionResult ShowClassbyRegisterStudent([FromBody] GetClassRegisterStudent request)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            var result = classService.ShowClassesForRegister(request, host);
            if(result.data is null)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.data, "Success");
        }
        [Authorize]
        [HttpPost("showClassByStudent")]
        public IActionResult ShowClassByStudent([FromBody] ListClassesRequest request)
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            string accountId = User.FindFirst("AccountId")?.Value;
            var result = classService.GetClassByStudent(request, accountId, host);
            if(result.data is null)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.data, result.message);
        }
        [HttpPost("enrollmentClass")]
        public IActionResult EnrollmentClass()
        {
            string host = Request.Scheme + "://" + Request.Host + "/";
            var result = classService.EnrollmentClass(host);
            if(result.data is null)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.data, result.message);
        }
        [HttpPut("updateSurcharge")]
        public IActionResult UpdateSurcharge([FromBody] UpdateSurchargeRequest request)
        {
            var result = classService.Update(request);
            return ResponseOk(result, "Success");
        }
        [HttpGet("showAllSurcharge/{classId}")]
        public IActionResult ShowAllSurcharge(string classId)
        {
            var result = classService.ShowAllSurcharge(classId);
            return ResponseOk(result, "Success");
        }
        [HttpDelete("deleteSurcharge/{surchargeId}")]
        public IActionResult DeleteSurcharge(string surchargeId)
        {
            var result = classService.DeleteSurcharge(surchargeId);
            if(result.data is null)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.data, "Success");
        }
    }
}
