using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class DeleteMultiMongo
    {
        public string AppId { get; set; }
        public List<string> ListResult { get; set; }
    }
}
