using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Enities
{
    [Table("hd_bank_mode_authentication")]
    public class ModeAuthentication
    {
        [Key, Column("mode_authentication_id")]
        public string ModeAuthenticationId { get; set; }

        [Column("mode_authentication_name")]
        public string ModeAuthenticationName { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = GeneralHelper.DateTimeDatabase;
    }
}
