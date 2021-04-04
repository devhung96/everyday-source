using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Ratings.Enities
{
    [Table("hd_bank_ratings")]
    public class Rating
    {
        [Key, Column("rating_id")]
        public string RatingId { get; set; } = Guid.NewGuid().ToString();

        [Column("robot_id")]
        public string RobotId { get; set; }

        [Column("rating_star")]
        public int Star { get; set; }


        [Column("user_id")]
        public string UserId { get; set; }
        
        [Column("user_name_display")]
        public string NameDisplay { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = GeneralHelper.DateTimeDatabase;
    }
}
