using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Seals.Request
{
    public class SealImportRequest
    {
        public string SealNumber { get; set; }
        public string SealNumberReturn { get; set; }
        public string FlightNumber { get; set; }
        public DateTime FlightDate { get; set; }
        public string AcReg { get; set; }
        public string Route { get; set; }
        public int? CityPairId { get; set; }
    }
}