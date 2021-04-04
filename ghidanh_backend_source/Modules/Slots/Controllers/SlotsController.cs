using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Slots.Entities;
using Project.Modules.Slots.Requests;
using Project.Modules.Slots.Responses;
using Project.Modules.Slots.Services;


namespace Project.Modules.Slots.Controllers
{
#if THPT
    [Route("api/[controller]")]
    [ApiController]
    public class SlotsController : BaseController
    {
        
        private readonly ISlotService _slotService;

        public SlotsController(ISlotService slotService)
        {
            _slotService = slotService;
        }
        [HttpPost("add")]
        public IActionResult StoreSlot([FromBody] StoreSlotRequest request)
        {
            return ResponseOk(_slotService.Store(request), "StoreNewSlotSuccess");
        }
        [HttpPost("Search")]
        public IActionResult SearchSlot([FromBody] SearchSlotRequest request)
        {
            return ResponseOk(_slotService.Search(request), "Success");
        }
        [HttpPut("{slotId}")]
        public IActionResult UpdateSlot([FromBody] UpdateSlotRequest request, string slotId)
        {
            Slot slot = _slotService.GetById(slotId);
            if (slot is null)
            {
                return ResponseBadRequest("SlotNotFound");
            }
            slot = _slotService.Update(request, slot);
            return ResponseOk(slot, "UpdateSlotSuccess");
        }
        [HttpDelete("{slotId}")]
        public IActionResult DeleteSlot(string slotId)
        {
            Slot slot = _slotService.GetById(slotId);
            if (slot is null)
            {
                return ResponseBadRequest("SlotNotFound");
            }
            _slotService.Delete(slot);
            return ResponseOk(null, "DeleteSlotSuccess");
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return ResponseOk(_slotService.GetAll(), "Success");
        }
        [HttpGet("{slotId}")]
        public IActionResult GetSlot(string slotId)
        {
            Slot slot = _slotService.GetById(slotId);
            if (slot is null)
            {
                return ResponseBadRequest("SlotNotFound");
            }
            return ResponseOk(slot, "GetDetailsSlotSuccess");
        }
    }
#endif
}
