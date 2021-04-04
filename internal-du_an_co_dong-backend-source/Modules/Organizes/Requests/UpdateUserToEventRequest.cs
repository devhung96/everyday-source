using Project.Modules.Organizes.Validations;
using Project.Modules.Users.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Requests
{
    public class UpdateUserToEventRequest
    {
        #region Thông tin trong bảng EventUser
        public int? GroupId { get; set; }
        public int? UserStock { get; set; }
        #endregion

        #region Thông tin của bảng User

        public string ShareholderCode { get; set; }
        //public string UserPassword { get; set; }
        public string UserImage { get; set; }
        [ValidationEmail]
        public string UserEmail { get; set; }
        public string FullName { get; set; }
        public string IdentityCard { get; set; }
        public string PlaceOfIssue { get; set; }
        public DateTime IssueDate { get; set; }
        [ValidatePhoneNumber]
        public string PhoneNumber { get; set; }
        #endregion
        //public string StockCode { get; set; }
        //public string OrganizeId { get; set; }
    }
}
