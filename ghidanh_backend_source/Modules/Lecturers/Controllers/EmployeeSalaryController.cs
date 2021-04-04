using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Lecturers.Requests;
using Project.Modules.Lecturers.Services;

namespace Project.Modules.Lecturers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeSalaryController : BaseController
    {
        private readonly IEmployeeSalaryService _employeeSalaryService;
        private readonly ILectureSalaryService lectureSalaryService;
        public EmployeeSalaryController(IEmployeeSalaryService employeeSalaryService, ILectureSalaryService lectureSalaryService)
        {
            _employeeSalaryService = employeeSalaryService;
            this.lectureSalaryService = lectureSalaryService;
        }


        /// <summary>
        /// Show all lương nhân viên
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ShowAll([FromBody] ShowAllEmployeeSalary request )
        {
            ResponseTable result =  _employeeSalaryService.ShowAll(request);
            return Ok(result);
        }

        [HttpGet("{courseId}/{lectureId}")]
        public IActionResult GetEmployeeSalaryBill(string courseId, string lectureId)
        {
            var result = lectureSalaryService.GetEmployeeSalaryBill(courseId, lectureId);
            if(result.data is null)
            {
                return ResponseBadRequest(result.messasge);
            }
            return ResponseOk(result.data, result.messasge);
        }

        [HttpPost("storeEmployeeSalary")]
        public IActionResult StoreEmployeeSalary([FromBody] StoreSalary request)
        {
            var result = lectureSalaryService.StoreEmployeeSalary(request);
            if(result.data is null)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.data, result.message);
        }


        [HttpGet("confirm/{courseId}")]
        public IActionResult GetEmployeeSalaryBill(string courseId)
        {
            var result = lectureSalaryService.ConfirmSalaryForCourse(courseId);
            if (result.data is null)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.data, result.message);
        }
    }
}