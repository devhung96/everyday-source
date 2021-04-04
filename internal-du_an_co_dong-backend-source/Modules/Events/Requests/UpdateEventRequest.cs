using Microsoft.AspNetCore.Http;
using Project.Modules.Events.Validations;
using Project.Modules.Organizes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Events.Requests
{
    /// <summary>
    /// + Time begin < time_End
    /// + EventType : 123
    /// + OrganizeId: Có tồn tại chưa
    /// </summary>
    public class UpdateEventRequest
    {
       
        public string EventName { get; set; }


        [CheckDateValidate]
        //[DateMoreTimeNow]
        [DateLessThanValidate("EventTimeEnd")]
        public DateTime EventTimeBegin { get; set; }

        [CheckDateValidate]
        public DateTime EventTimeEnd { get; set; }

        [AllowedExtensionsUrl(new string[] { ".png", ".sgv", ".jpg",".jpge" })]
        public string EventLogoUrl { get; set; }
        public int? EventType { get; set; } = 1;

        [CheckOrganizeExists]
        public string OrganizeId { get; set; }

        public string EventSetting { get; set; }
    }
}
