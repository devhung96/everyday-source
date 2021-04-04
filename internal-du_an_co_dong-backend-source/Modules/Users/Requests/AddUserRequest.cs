using Project.Modules.Users.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.Users.Entities.User;

namespace Project.Modules.Users.Requests
{
    public class AddUserRequest
    {
        public string UserPassword { get; set; }
        public string UserImage { get; set; }
        [ValidationEmail]
        public string UserEmail { get; set; }
        public string FullName { get; set; }
        public string IdentityCard { get; set; }
        public string PlaceOfIssue { get; set; }
        public DateTime IssueDate { get; set; }
        public string PhoneNumber { get; set; }
        public string StockCode { get; set; }
        public string OrganizeId { get; set; }
        public USER_TYPE UserType { get; set; }
        public string ShareholderCode { get; set; }
   
    }
    public class Profile
    {
        public string FullName { get; set; }
        public string IdentityCard { get; set; }
        public string PlaceOfIssue { get; set; }
        public DateTime IssueDate { get; set; }
        public string PhoneNumber { get; set; }
        public string StockCode { get; set; }
    }
    public class FilterShareholderCode
    {
        public string OrganizeId { get; set; }
        public string ShareholderCode { get; set; }
    }
}
