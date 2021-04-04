using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Seals.Entity
{
    [Table("wh_seal")]
    public class Seal
    {
        //
        [Key]
        [Column("se_number")]
        public string SealNumber { get; set; }
        [Column("se_flightnumber")]
        public string FlightNumber { get; set; }
        [Column("se_flightdate")]
        public DateTime FlightDate { get; set; }
        [Column("se_acreg")]
        public string AcReg { get; set; }
        [Column("se_status")]
        public int Status { get; set; } = (int)StatusSeals.NEW;
        [Column("se_export_date")]
        public DateTime? ExportDate { get; set; } = null;
        [Column("se_return")]
        public string Return { get; set; }
        [Column("se_import_date")]
        public DateTime? ImportDate { get; set; } = null;
        [Column("se_citypair_id")]
        public int? CityPairId { get; set; }
        [Column("se_route")]
        public string Route { get; set; }
        public List<SealDetail> SealDetails { get; set; }
        public List<SealProduct> SealProduct { get; set; }
        [NotMapped]
        public int ExportQuantity { get; set; }
        [NotMapped]
        public int QuantitySell { get; set; }
    }

    public enum StatusSeals
    {
        NEW = 0,
        EXPORT = 1,
        SELL = 2
    }
}