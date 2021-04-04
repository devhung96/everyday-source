using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Enities
{
    [Table("hd_bank_group")]
    public class Group
    {
        [Key, Column("group_id")]
        public string GroupId { get; set; } = Guid.NewGuid().ToString();

        [Column("group_name")]
        public string GroupName { get; set; }
        
        [Column("group_code")]
        public string GroupCode { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = GeneralHelper.DateTimeDatabase;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

    }
}
