using Project.Modules.Users.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class RequestForgot
    {
        [Required]
        [ValidateEmailSystem]
        public string ShareholderCode { get; set; }
        [Required]
      //  public string StockCode { get; set; }
        public string EventId { get; set; }

    }
    public class RequestForgotAdmin
    {
        [Required]
        public string UserEmail { get; set; }
        public string StockCode { get; set; }

    }
}
