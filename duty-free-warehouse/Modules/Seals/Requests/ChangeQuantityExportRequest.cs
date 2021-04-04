using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Seals.Request
{
    public class ChangeQuantityExportRequest
    {
        public string SealNumber { get; set; }
        public string FlightNumber { get; set; }
        public DateTime FlightDate { get; set; }
        public string AcReg { get; set; }
    }
}