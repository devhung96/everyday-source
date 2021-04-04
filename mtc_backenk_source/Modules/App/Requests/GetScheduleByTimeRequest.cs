using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.App.Requests
{
    public class GetScheduleByTimeRequest
    {
        [Required]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd HH:mm:ss}")]
        public DateTime timeBegin { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd HH:mm:ss}")]
        public DateTime timeEnd { get; set; }
    }
}
