using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Kafka.Requests
{
    public class ProducerRequest
    {
        [Required]
        public string Producer { get; set; }

        [Required]
        public object Request { get; set; }
    }
}
