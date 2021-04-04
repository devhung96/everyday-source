using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Response
{
    public class ResponseSurveyQuestion
    {
        public object Data { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
        public int? StatusCode { get; set; }
    }
}
