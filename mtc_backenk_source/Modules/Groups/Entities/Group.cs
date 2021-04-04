using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Entities
{
    [Table("mtc_group_tbl")]
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("group_id")]
        public string GroupId { get; set; }
        [Column("group_name")]
        public string GroupName { get; set; }
        [Column("group_code")]
        public string GroupCode { get; set; }
        [Column("group_image")]
        public string GroupImage { get; set; }
        [Column("group_status")]
        public STATUS GroupStatus { get; set; } = STATUS.Active;
        [Column("group_expired")]
        public DateTime? Expired { get; set; }
        [Column("group_created")]
        public DateTime Created { get; set; } = DateTime.UtcNow; 
   
        public  enum STATUS
        {
      
            UnActive =0,
            Active = 1,
            Expired =2,
            Delete =3
        }
        [NotMapped]
        public long TotalUser { get; set; }
        [NotMapped]
        public long TotalDevice { get; set; }

    }
}
