using Project.Modules.Users.Validations;

namespace Project.Modules.Users.Requests
{
    
    public class SendMailForgot
    {
        [ValidationEmail]
        public string Email { get; set; }
    }
}
