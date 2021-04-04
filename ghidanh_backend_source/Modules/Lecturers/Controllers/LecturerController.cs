using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Lecturers.Entities;
using Project.Modules.Lecturers.Services;
using System.Threading.Tasks;

namespace Project.Modules.Lecturers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturerController : BaseController
    {
        private readonly ILecturerService lecturerService;
        public LecturerController(ILecturerService lecturerService)
        {
            this.lecturerService = lecturerService;
        }
        [HttpPost("store")]
        public async Task<IActionResult> StoreLecturer([FromForm] AddLecturerRequest request)
        {
            (Lecturer lecturer, string message) = await lecturerService.StoreAsync(request);
            if (lecturer is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(lecturer, message);
        }
        [HttpPut("update/{lecturerId}")]
        public async Task<IActionResult> UpdateLecturer([FromForm] UpdateLecturerRequest request, string lecturerId)
        {
            (Lecturer lecturer, string message) = await lecturerService.UpdateAsync(request, lecturerId);
            if (lecturer is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(lecturer, message);
        }

        [HttpPost("showTable")]
        public IActionResult ShowTable([FromBody] RequestTable request)
        {
            (ResponseTable result, string message) = lecturerService.ShowTable(request);
            return ResponseOk(result, message);
        }
        [HttpGet("detail/{lecturerId}")]
        public IActionResult DetailLecturer(string lecturerId)
        {
            (Lecturer lecturer, string message) = lecturerService.GetLecturer(lecturerId);
            if (lecturer is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(lecturer, message);
        } 
        [HttpGet("showAll")]
        public IActionResult ShowAll()
        {
           
            return ResponseOk(lecturerService.ShowAll());
        }
        [HttpDelete("delete/{lecturerId}")]
        public IActionResult DeleteLecturer(string lecturerId)
        {
            (Lecturer lecturer, string message) = lecturerService.DeleteLecturer(lecturerId);
            if (lecturer is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(lecturer, message);
        }
        [HttpGet("LecturerSubject/{subjectId}")]
        public IActionResult GetLecturerTeachSubject(string subjectId)
        {
            return ResponseOk(lecturerService.GetListLectureTeachSubject(subjectId), "Success");
        }
        [HttpGet("Count")]
        public IActionResult GetNumberLecturer()
        {
            return ResponseOk(lecturerService.GetNumberLecturer(), "Success");
        }
    }
}
