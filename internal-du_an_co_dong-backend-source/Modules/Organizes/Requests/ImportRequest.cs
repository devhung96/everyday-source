using Project.Modules.Organizes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Requests
{
    [ValidationImport]
    public class ImportRequest
    {
        [ValidateGroup]
        public int GroupId { get; set; }
       // [ValidateEvent]
        public string EventId { get; set; }
        [Required(ErrorMessage = " bắt buộc.")]
        public List<ItemImportUserToEvent> ImportUsers { get; set; }
    }
    public class ItemImportUserToEvent
    {
        public string ShareholderCode { get; set; }
        public int UserStock { get; set; }
        public string UserEmail { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string IdentityCard { get; set; }
        public string PlaceOfIssue { get; set; }
        public string IssueDate { get; set; }
        [Required(ErrorMessage = " không để trống.")]
        public string PhoneNumber { get; set; }
        public bool ImportStatus { get; set; }

        public DateTime IssueDateFormat {
            get
            {
                DateTime.TryParseExact(IssueDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime);
                return dateTime;
            }
            set { this.IssueDateFormat = value; }
        }
    }
}
