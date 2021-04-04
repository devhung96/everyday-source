using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Seals.Entity
{
    [Table("wh_airport")]
    public class Airport
    {
        [Key]
        [Column("airport_code")]
        public string AirportCode { get; set; }
        [Column("airport_name")]
        public string AirportName { get; set; }
        [Column("country_code")]
        public string CountryCode { get; set; }
        [Column("airport_altitude")]
        public string AirportAltitude { get; set; }
        [Column("airport_latitude")]
        public string AirportLatitude { get; set; }
        [Column("airport_longtitude")]
        public string AirportLongtitude { get; set; }
    }
}