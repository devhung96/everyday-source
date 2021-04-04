using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Response
{
    public class InsertTagKafkaResponse 
    {
        public string RepositoryId { get; set; }
        public string TagCode { get; set; }
    }

    public class BaseTagKafkaResponse
    {
        public string DataResult { get; set; }
        public bool IsSuccess { get; set; }
        //public string DataRequest { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
    }
}
