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
    public class CreateEventRequest
    {
        [Required(ErrorMessage = "EventNameIsRequired")]
        public string EventName { get; set; }


        [Required(ErrorMessage= "EventTimeBeginIsRequired")]
        [CheckDateValidate]
        [DateMoreTimeNow]
        [DateLessThanValidate("EventTimeEnd")]
        public DateTime EventTimeBegin { get; set; }

        [Required(ErrorMessage = "EventTimeEndIsRequired")]
        [CheckDateValidate]
        public DateTime EventTimeEnd { get; set; }

        //[DataType(DataType.Upload)]
        //[MaxFileSizeAttribute(200)]
        //[AllowedExtensions(new string[] { ".png", ".sgv", ".jpg" })]
        //public IFormFile EventLogo { get; set; }

        [AllowedExtensionsUrl(new string[] { ".png", ".sgv", ".jpg",".jpeg" })]
        public string EventLogoUrl { get; set; }

        public int? EventType { get; set; } = 1;

        [Required(ErrorMessage = "OrganizeIdIsRequired")]
        [CheckOrganizeExists]
        public string OrganizeId { get; set; }


        public string EventSetting { get; set; }


    }
}
