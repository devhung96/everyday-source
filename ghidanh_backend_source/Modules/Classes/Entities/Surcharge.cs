using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Classes.Entities
{
    [Table("rc_surcharge")]
    public class Surcharge
    {
        [Key]
        [Column("surcharge_id")]
        [StringLength(36)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string SurchargeId { get; set; }
        [Column("class_id")]
        public string ClassId { get; set; }
        [Column("surcharge_name")]
        public string SurchargeName { get; set; }
        [Column("surcharge_amount")]
        public double SurchargeAmount { get; set; }
        [Column("surcharge_created_at")]
        public DateTime SurchargeCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
