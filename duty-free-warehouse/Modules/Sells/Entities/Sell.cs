using Project.Modules.Exchangerates.Entities;
using Project.Modules.Seals.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Sells.Entities
{
    [Table("wh_sell")]
    public class Sell
    {
        [Key]
        [Column("sl_id")]
        public long SellID { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        [Column("sl_flight_no")]
        public string FlightNo { get; set; }
        [Column("sl_flight_number_detail")]
        public string FlightNumberDetail { get; set; }
        [Column("sl_type_invoice")]
        public int? TypeInvoice { get; set; }
        [Column("sl_flight_date")]
        public DateTime FlightDate { get; set; }
        [Column("sl_invoice_no")]
        public string InvoiceNo { get; set; }
        [Column("sl_customer_name")]
        public string CustomerName { get; set; }
        [Column("sl_passport_number")]
        public string PassportNumber { get; set; }
        [Column("sl_nationality")]
        public string Nationality { get; set; }
        [Column("sl_seat_number")]
        public string SeatNumber { get; set; }

        public List<SellDetail> SellDetails { get; set; }
        [NotMapped]
        public List<ToTalSell> ToTalSell { get; set; }

    }
}
