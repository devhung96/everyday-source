using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.Database;
using Project.Modules.Question.Request;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Modules.Question.Response
{
    public class ResponseResultSubmitQuestion
    {
        //[JsonIgnore]
        //public string ResultId { get; set; }
        public string QuestionID { get; set; }
        //[JsonIgnore]
        //public string AppId { get; set; }
        //[JsonIgnore]
        //public string Content { get; set; }
        //[JsonIgnore]
        //public SubmitQuestion ContentJson
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(Content))
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return Newtonsoft.Json.JsonConvert.DeserializeObject<SubmitQuestion>(Content);
        //        }
        //    }
        //    set
        //    {
        //        ContentJson = value;
        //    }
        //}
        public Feedback Feedback
        {
            get;set;
        }
        public User User
        {
            get;set;
        }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
}
