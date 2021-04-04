using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Ratings.Enities
{
    [Table("hd_bank_robot")]
    public class Robot
    {
        [Key, Column("robot_id")]
        public string RobotId { get; set; } = Guid.NewGuid().ToString();

        [Column("robot_name")]
        public string RobotName { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = GeneralHelper.DateTimeDatabase;

        public List<Rating> Ratings { get; set; }
    }
}
