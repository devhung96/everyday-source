using Project.Modules.Sessions.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Sessions.Requests
{
    /// <summary>
    /// + Check event id
    /// </summary>
    public class CreateSessionRequest
    {
        public string SessionName { get; set; }

        [Required(ErrorMessage = "SessionTitleIsRequired")]
        public string SessionTitle { get; set; }

        public string SessionDescription { get; set; }



        public int SessionSort { get; set; }

        //[Required]
        [CheckEventExistsAttribute]
        public string EventId { get; set; }
    }
}
