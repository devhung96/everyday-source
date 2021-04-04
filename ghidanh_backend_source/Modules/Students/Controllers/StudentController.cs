using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Students.Entities;
using Project.Modules.Students.Requests;
using Project.Modules.Students.Services;

namespace Project.Modules.Students.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : BaseController
    {
        private readonly IStudentService studentService;
        public StudentController(IStudentService studentService)
        {
            this.studentService = studentService;
        }

        [HttpPost("store")]
        public async Task<IActionResult> Store([FromForm] AddStudent request)
        {
          
            (Student student, string message) = await studentService.Store(request);
            if (student is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(student, message);
        }
        [HttpGet("Count")]
        public IActionResult GetNumberStudent()
        {
            return ResponseOk(studentService.GetNumberStudent(), "Success");
        }
        [HttpGet("detail/{studentId}")]
        public IActionResult Detail(string studentId)
        {
            (Student student, string message) = studentService.Detail(studentId);
            if (student is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(student, message);
        }
        [HttpPost("update/{studentId}")]
        public async Task<IActionResult> Update([FromForm] UpdateStudentRequest request, string studentId)
        {
           (Student student, string message) = await studentService.Update(request, studentId);
            if (student is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(student, message);
        }
        [HttpDelete("delete/{studentId}")]
        public IActionResult Delete(string studentId)
        {
            (Student student, string message) = studentService.Delete(studentId);
            if (student is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(student, message);
        }
        [HttpPost("showTable")]
        public IActionResult ShowTable([FromBody] RequestTable request)
        {
            ResponseTable result = studentService.ShowStudent(request);
            if (result is null)
            {
                return ResponseBadRequest("RequestFaild");
            }
            return ResponseOk(result, "ShowSuccess");
        }   
        [HttpPost("showClass/{studentId}")]
        public IActionResult ShowClass([FromBody] RequestTable request, string studentId)
        {
            ResponseTable result = studentService.ShowClassOfStudent(request, studentId);
            if (result is null)
            {
                return ResponseBadRequest("RequestFaild");
            }
            return ResponseOk(result, "ShowSuccess");
        }
        [HttpPost("showStudentInClass/{classId}")]
        public IActionResult ShowStudentInClass(string classId, [FromBody] RequestTable request)
        {
            (ResponseTable result, string message)  = studentService.GetStudentsInClass(classId,request);
            if (result is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(result, "ShowSuccess");
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegistrationAsync([FromForm] AddRegistration request)
        {
            (RegistrationStudy study, string message) = await studentService.RegistrationAsync(request);
            if (study is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(study, message);
        }
#if DEBUG
#elif GHIDANH
        [Authorize]
        [HttpPost("register2")]
        public IActionResult Registration2([FromBody] Registration2Request request)
        {

            string accountId = User.FindFirstValue("AccountId");
            (RegistrationStudy study, string message) = studentService.RegistrationAsync2(request, accountId);
            if (study is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(study, message);
        }
       
        [HttpPost("register_student")]
        public async Task<IActionResult> RegistrationStudentAsync([FromForm] AddRegistration request)
        {
            (RegistrationStudy study, string message) = await studentService.RegistrationStudentAsync(request);
            if (study is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(study, message);
        }
#else
        [Authorize]
        [HttpPost("register2")]
        public IActionResult Registration2([FromBody] Registration2Request request)
        {

            string accountId = User.FindFirstValue("AccountId");
            (RegistrationStudy study, string message) = studentService.RegistrationAsync2(request, accountId);
            if (study is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(study, message);
        }
        [HttpPost("register_student")]
        public async Task<IActionResult> RegistrationStudentAsync([FromForm] AddRegistration request)
        {
            (RegistrationStudy study, string message) = await studentService.RegistrationStudentAsync(request);
            if (study is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(study, message);
        }
#endif



        [HttpDelete("deleteRegister")]
        public IActionResult DeleteRegistred([FromBody] DeleteStudentFromClassRequest request)
        {
            (RegistrationStudy study, string message) =  studentService.DeleteStudentFromClass(request);
            if (study is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(study, message);
        }

        [Authorize]
        [HttpPost("tuition")]
        public IActionResult CollectedTuition([FromBody] CollectedTuitionRequest valueInput)
        {
            string accountId = User.FindFirstValue("AccountId").ToString();
            (object data, string message) = studentService.CollectedTuition(valueInput, accountId);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        [HttpPost("studentsNotYetTuition")]
        public IActionResult StudentsNotYetTuition([FromBody] RequestTable request)
        {
            ResponseTable result = studentService.StudentNotYetTuition(request);
            return ResponseOk(result);
        }
    }
}
