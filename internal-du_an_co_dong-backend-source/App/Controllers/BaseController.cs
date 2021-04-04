using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Project.App.Controllers
{
    [Route("/")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        // 2XX
        const int OK = (int)HttpStatusCode.OK;
        const int CREATED = (int)HttpStatusCode.Created;
        const int NO_CONTENT = (int)HttpStatusCode.NoContent;
        // 4XX
        const int BAD_REQUEST = (int)HttpStatusCode.BadRequest;
        const int UNAUTHORIZED = (int)HttpStatusCode.Unauthorized;
        const int FORBIDDEN = (int)HttpStatusCode.Forbidden;
        // 5xx
        const int INTERNAL_SERVER_ERROR = (int)HttpStatusCode.InternalServerError;

        protected IActionResult ResponseOk(dynamic dataResponse = null, string messageResponse = null, string codeResponse = null)
        {
            return StatusCode(OK, new { data = dataResponse, message = messageResponse, code = codeResponse, statusCode = OK });
        }

        protected IActionResult ResponseCreated(dynamic dataResponse = null, string messageResponse = null, int codeResponse = CREATED)
        {
            return StatusCode(CREATED, new { data = dataResponse, message = messageResponse, code = codeResponse, statusCode = OK });
        }

        protected IActionResult ResponseNoContent()
        {
            return StatusCode(NO_CONTENT);
        }

        protected IActionResult ResponseBadRequest(string messageResponse = null, string codeResponse = null)
        {
            return StatusCode(BAD_REQUEST, new { message = messageResponse, code = codeResponse, statusCode = BAD_REQUEST });
        }

        protected IActionResult ResponseUnauthorized(string messageResponse = null, string codeResponse = null)
        {
            return StatusCode(UNAUTHORIZED, new { message = messageResponse, code = codeResponse, statusCode = UNAUTHORIZED });
        }

        protected IActionResult ResponseForbidden(string messageResponse = null, string codeResponse = null)
        {
            return StatusCode(FORBIDDEN, new { message = messageResponse, code = codeResponse, statusCode = FORBIDDEN });
        }

        protected IActionResult ResponseInternalServerError(string messageResponse = null, int codeResponse = INTERNAL_SERVER_ERROR)
        {
            return StatusCode(INTERNAL_SERVER_ERROR, new { message = messageResponse, code = codeResponse, statusCode = INTERNAL_SERVER_ERROR });
        }
        

        ////[HttpGet]
        //public IActionResult Test()
        //{
        //    return Ok(new
        //    {
        //        message = "Server Is Started, Hello to My System"
        //    });
        //}
    }
}