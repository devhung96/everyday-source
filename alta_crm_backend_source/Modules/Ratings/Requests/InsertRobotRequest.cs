using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Ratings.Requests
{
    public class InsertRobotRequest
    {
        [Required]
        public string RobotName { get; set; }
    }
}
