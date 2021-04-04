using Project.Modules.WhitelistIps.Validations;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.WhitelistIps.Requests
{
    public class AddWhitelistIpRequest
    {
        [Required]
        [CheckIpValidation]
        public string IpAddress { get; set; }
    }
}
