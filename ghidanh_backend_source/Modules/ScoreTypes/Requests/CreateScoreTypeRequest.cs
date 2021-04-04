using Project.Modules.ScoreTypes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ScoreTypes.Requests
{
    [CheckCreateSocreTypeValidation]
    public class CreateScoreTypeRequest
    {
        [Required, StringLength(255, ErrorMessage = "ScoreTypeNameValueCannotExceed4Characters")]
        public string ScoreTypeName { set; get; }
        [Required]
        private double scoreTypeMultiplier { set; get; }
        public double ScoreTypeMultiplier
        {
            set
            {
                scoreTypeMultiplier = value;
            }
            get
            {
                if (scoreTypeMultiplier == 0)
                    return scoreTypeMultiplier = 1;
                return scoreTypeMultiplier;
            }
        }
    }
}
