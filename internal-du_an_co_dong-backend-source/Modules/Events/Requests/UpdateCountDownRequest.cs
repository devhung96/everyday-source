using Project.Modules.Events.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Events.Requests
{
    public class UpdateCountDownRequest
    {
        [Required(ErrorMessage = "CountDownStatusIsRequired")]
        public EVENT_COUNT_DOWN CountDownStatus { get; set; }
    }
}
