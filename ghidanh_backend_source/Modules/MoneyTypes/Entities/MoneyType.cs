using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.MoneyTypes.Entities
{

    /// <summary>
    /// Mức thu học phí:
    /// DevHung
    /// </summary>
    [Table("rc_money_type")]
    public class MoneyType
    {
       
        [Key]
        [Column("money_type_id")]
        public string MoneyTypeId { get; set; } = Guid.NewGuid().ToString();
        [Column("money_type_name")]
        public string MoneyTypeName { get; set; }

        [Column("money_type_percent")]
        public float MoneyTypePercent { get; set; }

    }
}
