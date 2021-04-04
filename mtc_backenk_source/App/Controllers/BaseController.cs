using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Project.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult ResponseOk(dynamic dataResponse = null, string messageResponse = null, int codeResponse = (int)HttpStatusCode.OK)
        {
            return StatusCode((int)HttpStatusCode.OK, new { data = dataResponse, message = messageResponse, code = codeResponse, statusCode = (int)HttpStatusCode.OK });
        }

        protected IActionResult ResponseCreated(dynamic dataResponse = null, string messageResponse = null, int codeResponse = (int)HttpStatusCode.Created)
        {
            return StatusCode((int)HttpStatusCode.Created, new { data = dataResponse, message = messageResponse, code = codeResponse, statusCode = (int)HttpStatusCode.Created });
        }

        protected IActionResult ResponseNoContent()
        {
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        protected IActionResult ResponseBadRequest(string messageResponse = null, int codeResponse = (int)HttpStatusCode.BadRequest)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, new { message = messageResponse, code = codeResponse, statusCode = (int)HttpStatusCode.BadRequest });
        }

        protected IActionResult ResponseUnauthorized(string messageResponse = null, int codeResponse = (int)HttpStatusCode.Unauthorized)
        {
            return StatusCode((int)HttpStatusCode.Unauthorized, new { message = messageResponse, code = codeResponse, statusCode = (int)HttpStatusCode.Unauthorized });
        }

        protected IActionResult ResponseForbidden(string messageResponse = null, int codeResponse = (int)HttpStatusCode.Forbidden)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, new { message = messageResponse, code = codeResponse, statusCode = (int)HttpStatusCode.Forbidden });
        }

        protected IActionResult ResponseInternalServerError(string messageResponse = null, int codeResponse = (int)HttpStatusCode.InternalServerError)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new { message = messageResponse, code = codeResponse, statusCode = (int)HttpStatusCode.InternalServerError });
        }
    }
}