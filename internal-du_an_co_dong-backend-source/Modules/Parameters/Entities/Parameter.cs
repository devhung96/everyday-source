using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Parameters.Entities
{
    [Table("shareholder_parameter")]
    public class Parameter
    {
        [Key]
        [Column("parameter_id")]
        public string ParameterId { get; set; } = Guid.NewGuid().ToString();
        [Column("parameter_key")]
        public string ParameterKey { get; set; }
        [Column("parameter_name")]
        public string ParameterName { get; set; }
        [Column("parameter_value")]
        public string ParameterValue { get; set; }
        [Column("parameter_type")]
        public TYPE_PARAMS Type { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }

    public enum TYPE_PARAMS
    {
        TEXT = 1,
        ARRAY = 2
    }
}
