using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Contacts.Requests
{
    public class CreateContactRequest
    {
        [Required(ErrorMessage = "TheContactNameFieldIsRequired"),StringLength(255,ErrorMessage = "ContactNameValueCannotExceed255Characters")]
        public string ContactName { get; set; }
        [Required(ErrorMessage = "TheContactPhoneFieldIsRequired"), StringLength(10, ErrorMessage = "ContactPhoneValueCannotExceed10Characters")]
        public string ContactPhone { get; set; }
        [Required(ErrorMessage = "TheContactEmailFieldIsRequired"), StringLength(255, ErrorMessage = "ContactEmailValueCannotExceed255Characters")]
        public string ContactEmail { get; set; }
        [Required(ErrorMessage = "TheContactContentFieldIsRequired")]
        public string ContactContent { get; set; }
    }
}
