using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Contacts.Entities;
using Project.Modules.Contacts.Requests;
using Project.Modules.Contacts.Services;

namespace Project.Modules.Contacts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : BaseController
    {
        private readonly IContactService contactService;
        public ContactController(IContactService _contactService)
        {
            contactService = _contactService;
        }
        [HttpPost("showAllContact")]
        public IActionResult ShowAllContact(RequestTable requestTable)
        {
            (object data, string message) = contactService.ShowAllContact(requestTable);
            return ResponseOk(data, message);
        }
        [HttpPost("createContact")]
        public IActionResult CreateContact([FromBody] CreateContactRequest valueInput)
        {
            (Contact data, string message) = contactService.CreateContact(valueInput);
            return ResponseOk(data, message);
        }
        [HttpGet("showDetailContact/{contactId}")]
        public IActionResult ShowDetailContact(string contactId)
        {
            (Contact data, string message) = contactService.ShowDetailContact(contactId);
            if(data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }

    }
}