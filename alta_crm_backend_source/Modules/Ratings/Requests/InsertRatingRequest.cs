using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Ratings.Requests
{
    public class InsertRatingRequest
    {
        public string RobotId { get; set; }
        public int Star { get; set; }
        public string UserId { get; set; }
        public string NameDisplay { get; set; }
    }
}
