using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class ListRevote
    {
        public string QuestionId { get; set; }
        public string EventId { get; set; }
    }
}
