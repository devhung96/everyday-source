using Project.App.Validations;
using Project.Modules.Slots.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Slots.Requests
{
    [StoreSlotValidation]
    public class StoreSlotRequest
    {
        [Required]
        public string SlotName { get; set; }
        [Required]
        public string SlotStartAt { get; set; }
        [Required]
        public string SlotEndAt { get; set; }
    }
}
