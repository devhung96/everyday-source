using Project.Modules.Groups.Requests;
using Project.Modules.Question.Validation;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class SubmitQuestion
    {
        public int Id { get; set; }
        public string AppId { get; set; }
        public string QuestionID { get; set; }
        public string UserID { get; set; }
        [CheckAnswerValidation]
        public List<Selection> Answers { get; set; } = new List<Selection>();
        [CheckFeedbackValidation]
        public Feedback Feedback { get; set; }
        //[Required(ErrorMessage = "EventId không được bỏ trống.")]
        public string EventID { get; set; }
        public float? Stock { get; set; } = 0;
        public object AnotherAnswer { get; set; }
        /// <summary>
        /// flag = 1 : người vận hành auto gửi submit cho người tham gia
        /// </summary>
        public int Flag { get; set; }
        public int FlagBeforeEvent { get; set; }
        public string Token { get; set; }

        public SubmitQuestion Copy()
        {
            SubmitQuestion c = new SubmitQuestion()
            {
                Id = this.Id,
                AnotherAnswer = this.AnotherAnswer,
                //Answers = this.Answers,
                AppId = this.AppId,
                EventID = this.EventID,
                Feedback = this.Feedback,
                Flag = this.Flag,
                FlagBeforeEvent = this.FlagBeforeEvent,
                QuestionID = this.QuestionID,
                Stock = this.Stock,
                Token = this.Token,
                UserID = this.UserID
            };
            foreach (var it in this.Answers)
            {
                c.Answers.Add(it);
            }

            return c;
        }
    }
}
