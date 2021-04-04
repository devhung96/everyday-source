using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Seals.Entity
{
    [Table("wh_copy_seal")]
    public class CopySeal
    {
        [Key]
        [Column("citypair_id")]
        public int CitypairId { get; set; }
        [Column("data_copy")]
        public string DataCopy { get; set; }

    }
}