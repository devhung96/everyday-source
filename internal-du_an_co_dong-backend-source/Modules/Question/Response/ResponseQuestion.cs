//using AutoMapper.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Modules.Question.Entities;
using Project.Modules.Question.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Response
{
    public class ResponseQuestion
    {
        //public readonly IConfiguration _config;
        //public ResponseQuestion(IConfiguration config)
        //{
        //    _config = config;
        //}
        public string QuestionID { get; set; }

        public string Title { get; set; }

        [JsonIgnore]
        public string AppId { get; set; }

        public string Content { get; set; }

        public string Media { get; set; }

        public TypeQuestion? Type { get; set; }

        public string Answers { get; set; }

        public string FeedBack { get; set; }

        public int? CountAnswers { get; set; }

        public string Extends { get; set; }

        public string Option { get; set; }

        public int OrderBy { get; set; }

        public string Description { get; set; }
        public bool IsSent { get; set; }

        public string AnotherAnswer { get; set; }
        public StatusQuestion? Status { get; set; } = StatusQuestion.SHOW;

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; }

        public object AnotherAnswerJson { 
            get 
            {
                if (String.IsNullOrEmpty(AnotherAnswer))
                {
                    return null;
                }
                return Newtonsoft.Json.JsonConvert.DeserializeObject(AnotherAnswer);
            }
        }
        public JArray OptionJson
        {
            get
            {
                if (String.IsNullOrEmpty(Option))
                {
                    return new JArray();
                }
                return JArray.Parse(Option);
            }
        }
        public List<Selection> AnswersJson
        {
            get
            {
                if (String.IsNullOrEmpty(Answers))
                    return null;
                else
                {
                    IConfiguration _config = new ConfigurationBuilder()
                                                   .SetBasePath(Directory.GetCurrentDirectory())
                                                   .AddJsonFile("appsettings.json").Build();
                    //var listMediaJSON = new List<MediaCustome>();
                    var listAnswersJSON = JsonConvert.DeserializeObject<List<Selection>>(Answers)
                                    .Select(x =>
                                    {
                                        x.File.Select(
                                                f =>
                                                {
                                                    f.urlCustome = $"{ _config["BackEnd:Url"]}/{f.LocalPath}";
                                                    return f;
                                                })
                                        .ToList();
                                        return x;
                                    })
                                    .ToList();
                    return listAnswersJSON;
                }
            }
            //set { AnswersJson = value; }
        }
        public List<MediaCustome> MediaJson
        {
            get
            {
                if (String.IsNullOrEmpty(Media))
                    return null;
                else
                {
                    IConfiguration _config = new ConfigurationBuilder()
                                                   .SetBasePath(Directory.GetCurrentDirectory())
                                                   .AddJsonFile("appsettings.json").Build();
                    var listMediaJSON = new List<MediaCustome>();
                    listMediaJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MediaCustome>>(Media)
                                    .Select(x =>
                                    {
                                        x.urlCustome = $"{ _config["BackEnd:Url"]}/{x.LocalPath}";
                                        return x;
                                    })
                                    .ToList();
                    return listMediaJSON;
                }
            }
            //set { MediaJson = value; }
        }
        public Feedback FeedbackJson
        {
            get
            {
                if (String.IsNullOrEmpty(FeedBack))
                    return null;
                IConfiguration _config = new ConfigurationBuilder()
                                                   .SetBasePath(Directory.GetCurrentDirectory())
                                                   .AddJsonFile("appsettings.json").Build();
                //var listMediaJSON = new List<MediaCustome>();
                var feedbackJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<Feedback>(FeedBack);
                feedbackJSON
                .File
                    .Select(
                    x =>
                    {
                        x.urlCustome = $"{ _config["BackEnd:Url"]}/{x.LocalPath}";
                        return x;
                    })
                    .ToList();
                return feedbackJSON;
            }
        }
        //public int FlagQuestion { get
        //    {
        //        if (String.IsNullOrEmpty(QuestionID))
        //            return 0;
        //        IConfiguration _config = new ConfigurationBuilder()
        //                                           .SetBasePath(Directory.GetCurrentDirectory())
        //                                           .AddJsonFile("appsettings.json").Build();
        //        //var listMediaJSON = new List<MediaCustome>();
        //        var feedbackJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<Feedback>(FeedBack);
        //        feedbackJSON
        //        .File
        //            .Select(
        //            x =>
        //            {
        //                x.urlCustome = $"{ _config["BackEnd:Url"]}/{x.LocalPath}";
        //                return x;
        //            })
        //            .ToList();
        //        return feedbackJSON;
        //    }
        //}
    }
}
