using Project.Modules.Users.Validation;
using Project.Modules.Users.Validations;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Users.Request
{
    [ValidationCreateUser]
    public class RequestCreateNewUser
    {
        [Required]
        [ValidationEmail]
        public string UserName { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string DepartmentCode { get; set; }
    }
}
