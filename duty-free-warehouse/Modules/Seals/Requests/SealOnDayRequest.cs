using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Seals.Request
{
    public class SealOnDayRequest
    {
        [Required]
        public string FlightNumber { get; set; }
        [Required]
        public string Segment { get; set; }
        public string SealNumber { get; set; }
        public string SealNumberReturn { get; set; }
        public string AcReg { get; set; }
    }
}
