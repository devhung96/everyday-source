
using Project.Modules.Users.Validations;

namespace Project.Modules.Users.Requests
{
    [ValidateForgotPasswordRequestAttribute]
    public class ForgotPasswordRequest
    {
        public string PasswordNew { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
