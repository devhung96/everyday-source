using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Modules.Question.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Response
{
    public class ResponseResultSubmit
    {
        public string ResultId { get; set; }
        public string QuestionID { get; set; }
        public string AppId { get; set; }
        public string Content { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public SubmitQuestion ContentJson
        {
            get
            {
                if (String.IsNullOrEmpty(Content))
                    return null;
                return JsonConvert.DeserializeObject<SubmitQuestion>(Content);
            }
        }
        //public JArray AnswersJson
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(Answers))
        //            return new JArray();
        //        return JArray.Parse(Answers);
        //    }
        //}
    }
}
