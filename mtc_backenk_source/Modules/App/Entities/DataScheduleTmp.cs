using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.App.Entities
{
    [Table("mtc_data_schedule_tmp")]
    public class DataScheduleTmp
    {
        [Key]
        [Column("data_schedule_tmp_id")]
        public string DataScheduleTmpId { get; set; } = Guid.NewGuid().ToString();


        [Column("data_schedule_tmp_key")]
        public string DataScheduleTmpKey { get; set; }

        [Column("data_schedule_tmp_data")]
        public string DataScheduleTmpData { get; set; }

        [Column("data_schedule_tmp_created_at")]
        public DateTime DataScheduleTmpCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
