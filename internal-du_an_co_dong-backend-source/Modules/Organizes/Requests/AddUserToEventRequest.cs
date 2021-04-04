using Project.Modules.Events.Validations;
using Project.Modules.Organizes.Validations;
using Project.Modules.Users.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.Users.Entities.User;

namespace Project.Modules.Organizes.Requests
{
    [ValidateAddUserEventRequest]
    public class AddUserToEventRequest
    {
        public int GroupId { get; set; }
        public string ShareholderCode { get; set; }
        public string EventId { get; set; }
        //[IntegerValidator(MinValue = 1, MaxValue = 10)]
        [Required(ErrorMessage ="NotNull")]
        public int? UserStock { get; set; }
        public string UserPassword { get; set; }
        public string UserImage { get; set; }
        [ValidationEmail]
        public string UserEmail { get; set; }
        public string FullName { get; set; }
        public string IdentityCard { get; set; }
        public string PlaceOfIssue { get; set; }
        public DateTime IssueDate { get; set; }
        [Required(ErrorMessage ="NotNull")]
        [ValidatePhoneNumber]
        public string PhoneNumber { get; set; }
        public string StockCode { get; set; }
        public string OrganizeId { get; set; }
        public USER_TYPE UserType { get; set; } = USER_TYPE.CLIENT;
    }

}
