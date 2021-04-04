using Project.Modules.WhitelistIps.Validations;

namespace Project.Modules.WhitelistIps.Requests
{
    public class UpdateWhitelistIpRequest
    {
        [CheckIpValidation]
        public string IpAddress { get; set; }
        public int? Status { get; set; }
    }
}
