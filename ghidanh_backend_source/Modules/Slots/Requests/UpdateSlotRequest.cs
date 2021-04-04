using Project.App.Validations;
using Project.Modules.Slots.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Slots.Requests
{
    [UpdateSlotValidation]
    public class UpdateSlotRequest
    {
        public string SlotName { get; set; }
        public string SlotStartAt { get; set; }
        public string SlotEndAt { get; set; }
    }
}
