using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ScoreTypes.Entities
{
    [Table("rc_score_types")]
    public class ScoreType
    {
        [Key]
        [Column("score_type_id")]
        public string ScoreTypeId { get; set; } = Guid.NewGuid().ToString();
        [Column("score_type_name")]
        public string ScoreTypeName { set; get; }
        [Column("multiplier")]
        public double ScoreTypeMultiplier { set; get; }
        
    }
}
