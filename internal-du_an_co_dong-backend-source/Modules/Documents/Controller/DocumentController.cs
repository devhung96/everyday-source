using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Project.App.Controllers;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.Documents.Entities;
using Project.Modules.Documents.Requests;
using Project.Modules.Documents.Services;
using Project.Modules.Question.Validation;
using Project.Modules.Users.Entities;

namespace Project.Modules.Documents.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : BaseController
    {
        private readonly IDocumentService _IdocumentService;
        private readonly MariaDBContext _mariaDBContext;
        private StringValues stringValues;
        public DocumentController(IDocumentService IdocumentService, MariaDBContext mariaDBContext)
        {
            _IdocumentService = IdocumentService;
            _mariaDBContext = mariaDBContext;
        }
        [DisplayNameAttribute(Modules = 3,Level = 1)]
        [Authorize]
        [HttpPost]
        public IActionResult Stored([FromBody] StoredDocumentRequest storedDocumentRequest)
        {
            string userId = (string)User.FindFirst("UserId").Value;
            stringValues = Request.Headers["Event-Id"];
            storedDocumentRequest.EventId = stringValues;
            DocumentFile documentFile = _IdocumentService.Stored(storedDocumentRequest, userId);
            return ResponseOk(documentFile, "CreateDocumentSuccess");
        }
        [DisplayNameAttribute(Modules = 3, Level = 16)]
        [Authorize]
        [HttpDelete]
        public IActionResult Delete([FromBody] DocumentParameterRequest deleteRequest)
        {
            stringValues = Request.Headers["Event-Id"];
            deleteRequest.EventId = stringValues;
            DocumentFile document = _IdocumentService.Delete(deleteRequest);
            if (document == null)
                return ResponseBadRequest("DocumentDoNotExists");
            return ResponseOk(document, "DeleteDocumentSuccess");
        }
        [DisplayNameAttribute(Modules = 3, Level = 2)]
        [Authorize]
        [HttpPost("show-all")]
        public IActionResult ShowAll([FromBody] RequestTable requestTable)
        {
            stringValues = Request.Headers["Event-Id"];
            object documentFiles = _IdocumentService.ShowAll(requestTable, stringValues);
            return ResponseOk(documentFiles, "ShowAllDocumentSuccess");
        }
        [DisplayNameAttribute]
        [HttpPost("show-detail")]
        public IActionResult ShowDetail([FromBody] DocumentParameterRequest documentParameterRequest)
        {
            string host = $"{Request.Scheme}://{Request.Host.Value}";
            DocumentFile documentFile = _IdocumentService.FindId(documentParameterRequest.DocumentId, documentParameterRequest.EventId);
            if (documentFile == null)
                return ResponseBadRequest("DocumentDoNotExists");
            return ResponseOk(documentFile, "ShowDetailDocumentSuccess");
        }
        [DisplayNameAttribute(Modules = 3, Level = 8)]
        [Authorize]
        [HttpPut("{documentId}")]
        public IActionResult Update([FromBody] UpdateDocumentRequest update, string documentId)
        {
            stringValues = Request.Headers["Event-Id"];
            update.EventId = stringValues;
            DocumentFile document = _IdocumentService.FindId(documentId, update.EventId);
            if (document == null)
                return ResponseBadRequest("DocumentDoNotExists");
            document = _IdocumentService.Update(documentId, update);
            User user = _mariaDBContext.Users.FirstOrDefault(x => x.UserId.Equals(document.UserId));
            document.UserName = user == null ? null : user.FullName;
            return ResponseOk(document, "UpdateDocumentSuccess");
        }
    }
}