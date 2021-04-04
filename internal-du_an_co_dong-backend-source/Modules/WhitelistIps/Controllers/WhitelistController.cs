using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.WhitelistIps.Entities;
using Project.Modules.WhitelistIps.Requests;
using Project.Modules.WhitelistIps.Services;
using System;

namespace Project.Modules.WhitelistIps.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhitelistController : BaseController
    {
        private readonly IWhiteListIpService whiteListIpService;
        public WhitelistController(IWhiteListIpService WhiteListIpService)
        {
            whiteListIpService = WhiteListIpService;
        }

        [HttpGet]
        public IActionResult ShowAll()
        {
            return ResponseOk(whiteListIpService.ShowAll(), "Show List Success");
        }

        [HttpGet("{idWhitelist}")]
        public IActionResult ShowDetail(string idWhitelist)
        {
            WhitelistIp whitelistIp = whiteListIpService.FindId(idWhitelist);
            if(whitelistIp is null)
            {
                return ResponseBadRequest("Id Whitelist Not Found");
            }    

            return ResponseOk(whitelistIp, "Show Detail Success");
        }

        [HttpPost]
        public IActionResult AddWhitelistIp([FromBody]AddWhitelistIpRequest request)
        {
            WhitelistIp whitelistIp = new WhitelistIp { IpAddress = request.IpAddress };
            if (whiteListIpService.FindId(null, whitelistIp.IpAddress) != null)
            {
                return ResponseBadRequest("IpAddress Is Exists On System");
            }

            return ResponseOk(whiteListIpService.AddNewWhitelistIp(whitelistIp), "Add WhiteList Success");
        }

        [HttpPut("{whitelistId}")]
        public IActionResult AddWhitelistIp([FromBody]UpdateWhitelistIpRequest request, string whitelistId)
        {
            WhitelistIp whitelistIp = whiteListIpService.FindId(whitelistId);
            if (whitelistIp is null)
            {
                return ResponseBadRequest("Id Whitelist Not Found");
            }

            if (!string.IsNullOrEmpty(request.IpAddress))
            {
                var checkWhiteListIp = whiteListIpService.FindId(null, request.IpAddress);
                if (checkWhiteListIp != null && checkWhiteListIp.WhitelistId != whitelistId)
                {
                    return ResponseBadRequest("IpAddress Is Exists On System");
                }

                whitelistIp.IpAddress = request.IpAddress;
            }

            if (request.Status != null)
            {
                whitelistIp.WhitelistStatus = request.Status.Value;
            }

            return ResponseOk(whiteListIpService.UpdateWhitelistIp(whitelistIp), "Update WhiteList Success");
        }

        [HttpDelete("{whitelistId}")]
        public IActionResult DeleteWhitelistIp(string whitelistId)
        {
            WhitelistIp whitelistIp = whiteListIpService.FindId(whitelistId);
            if (whitelistIp is null)
            {
                return ResponseBadRequest("Id Whitelist Not Found");
            }

            whitelistIp.DeletedAt = DateTime.Now;
            whiteListIpService.UpdateWhitelistIp(whitelistIp);
            return ResponseOk(null, "Delete WhiteList Success");
        }
    }
}