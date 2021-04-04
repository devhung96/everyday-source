using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Chats.Entities;
using Project.Modules.Chats.Requests;
using Project.Modules.Chats.Services;

namespace Project.Modules.Chats.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : BaseController
    {
        private readonly IChatService _IchatService;
        public ChatController(IChatService IchatService)
        {
            _IchatService = IchatService;
        }
        [HttpPost]
        public IActionResult Stored(StoredChatRequest value)
        {
            (Chat data, string message) = _IchatService.Stored(value);
            return ResponseOk(data,message);
        }


    }
}