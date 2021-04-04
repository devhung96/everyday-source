using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Kafka
{
    public class KafkaRequest
    {
        public string DataResult { get; set; } = "";
        public string DataRequest { get; set; }
        public string Message { get; set; } = "";
        public bool IsSuccess { get; set; } = false;
        public string TransactionId { get; set; }
    }
}
