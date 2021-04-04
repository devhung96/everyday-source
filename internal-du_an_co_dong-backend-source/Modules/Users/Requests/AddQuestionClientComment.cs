using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class AddQuestionClientComment
    {
        public string Content { get; set; }
        public string UserId { get; set; }
    }
}
