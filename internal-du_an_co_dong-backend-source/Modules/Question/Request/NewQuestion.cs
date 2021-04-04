using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Project.Modules.Question.Entities;
using Project.Modules.Question.Response;
using Project.Modules.Question.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Request
{
    public class NewQuestion
    {
        //[Required]
        public string AppId { get; set; }
        [Required(ErrorMessage = "Tiêu đề không được bỏ trống.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Nội dung không được bỏ trống.")]
        public string Content { get; set; }
        //[CheckJArray]
        [CheckListMedia]
        public List<MediaCustome> Media { get; set; } = new List<MediaCustome>();
        [EnumDataType(typeof(TypeQuestion))]
        public TypeQuestion Type { get; set; }
        [CheckAnswerValidation]
        public List<Selection> Answers { get; set; }
        [CheckFeedbackValidation]
        public Feedback Feedback { get; set; }
        public Extends Extends { get; set; }

        public string Description { get; set; }
        [ArrayEnumOption]
        public JArray Option { get; set; } = new JArray();
        public StatusQuestion StatusQuestion { get; set; } = StatusQuestion.SHOW;
        public int? CountAnswers { get; set; }
        public object AnotherAnswer { get; set; }
        //[CheckSurveyID]
        [CheckSessionID]
        public string SessionID { get; set; }
        public int OrderBy { get; set; }

        //response
        public string QuestionID { get; set; }
    }
    public enum OptionQuestion
    {
        SEE_RESULTS_RIGHT_AWAY = 1,
        RECONFIRM_THE_ANSWERS = 2,
        SEE_THE_RESULTS_AFTER_COMPLETING_THE_EVENT = 3
    }

    public enum TypeAnswers
    {
        Text = 1,
        File = 2
    }
    public class Feedback
    {
        //[Required]
        public string Title { get; set; }
        [JsonIgnore]
        public string Code { get; set; } = GeneralHelper.generateID();
        //[Required]
        public string Content { get; set; }
        public bool Selected { get; set; } = false;
        public int? Stock { get; set; } = 0;
        public bool Bingo { get; set; } = false;
        public bool Auto { get; set; } = false;
        //[CheckJArray]
        [CheckListMedia]
        public List<MediaCustome> File { get; set; } = new List<MediaCustome>();
        [EnumDataType(typeof(TypeAnswers))]
        public TypeAnswers TypeAnswer { get; set; }
        public bool UploadFile { get; set; } = false; 
    }
    public class Selection
    {
        [Required(ErrorMessage = "Tiêu đề trong câu trả lời không được bỏ trống.")]
        public string Title { get; set; }
        [JsonIgnore]
        public string Code { get; set; } = GeneralHelper.generateID();
        [Required(ErrorMessage = "Nội dung không được bỏ trống.")]
        public string Content { get; set; }
        public bool Selected { get; set; } = false;
        public int? Stock { get; set; } = 0;
        public bool Bingo { get; set; } = false;
        public bool Auto { get; set; } = false;
        //[CheckJArray]
        [CheckListMedia]
        public List<MediaCustome> File { get; set; } = new List<MediaCustome>();

        public string toString()
        {
            return this.Code + " | " + this.Stock+" | "+ this.Title;
        }
    }
    public class UpdateQuestion
    {
        //[Required]
        public string AppId { get; set; }
        [Required(ErrorMessage ="Tiêu đề không được bỏ trống.")]
        public string Title { get; set; }
        [Required(ErrorMessage = " Nội dung không được bỏ trống")]
        public string Content { get; set; }
        //[CheckJArray]
        [CheckListMedia]
        public List<MediaCustome> Media { get; set; } = new List<MediaCustome>();
        [EnumDataType(typeof(TypeQuestion))]
        public TypeQuestion Type { get; set; }
        [CheckAnswerValidation]
        public List<Selection> Answers { get; set; }
        [CheckFeedbackValidation]
        public Feedback Feedback { get; set; }
        public Extends Extends { get; set; }

        public string Description { get; set; }
        [ArrayEnumOption]
        public JArray Option { get; set; } = new JArray();
        public StatusQuestion StatusQuestion { get; set; } = StatusQuestion.SHOW;
        public int? CountAnswers { get; set; }
        public object AnotherAnswer { get; set; }
        //[CheckSurveyID]
        [CheckSessionID]
        public string SessionID { get; set; }
        public int OrderBy { get; set; }
        //response
        public string QuestionID { get; set; }
    }
    public class Extends
    {
        public int FlagBeforeEnvent { get; set; }
    }

    public class Rating
    {
        public int? NoStep { get; set; }
        public string ImageIcon { get; set; }
        public bool Selected { get; set; } = false;
        public int? Result { get; set; }
    }
    public class NumricScale
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
        public string IconMin { get; set; }
        public string IconMax { get; set; }
        public bool Selected { get; set; } = false;
        public int? Result { get; set; }
    }
}
