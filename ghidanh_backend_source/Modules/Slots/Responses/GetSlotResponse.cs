using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Slots.Responses
{
    public class GetSlotResponse
    {
        public string SlotId { get; set; }
        public string SlotStartAt { get; set; }
        public string SlotEndAt { get; set; }

    }
}
