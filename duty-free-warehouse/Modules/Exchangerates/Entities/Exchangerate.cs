using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Exchangerates.Entities
{
    [Table("wh_exchangerates")]
    public class Exchangerate
    {
        [Key]
        [Column("exchangerate_id")]
        public string ExchangerateId { get; set; } = Guid.NewGuid().ToString();
        [Column("exchangerate_code")]
        public string ExchangerateCode { get; set; }
        [Column("exchangerate_rate")]
        public double ExchangerateRate { get; set; }
        [Column("exchangerate_status")]
        public EnumExchangerateStatus ExchangerateStatus { get; set; } = EnumExchangerateStatus.Display;
        [Column("exchangerate_order")]
        public int ExchangerateOrder { get; set; }
    }
    public enum EnumExchangerateStatus
    {
        Display = 1,
        UnDisplay = 0
    }
}
