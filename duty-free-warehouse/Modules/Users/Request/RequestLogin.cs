using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Users.Request
{
    public class RequestLogin
    {
        [Required]
        //[ValidationEmail]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}
