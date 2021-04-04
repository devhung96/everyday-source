using Project.Modules.Authorities.Entities;
using Project.Modules.Authorities.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Authorities.Requests
{
    [ValidateStoreForVote]
    public class StoreForVoteRequest
    {
        [Required(ErrorMessage = "EventIDIsRequired")]
        public string EventID { get; set; }
        [Required(ErrorMessage = "UserIDIsRequired")]
        public string UserID { get; set; }
        [Required(ErrorMessage = "QuestionIDIsRequired")]
        public string QuestionID { get; set; }
        [Required(ErrorMessage = "VoteRequestsIsRequired")]
        public List<VoteRequest> VoteRequests { get; set; }
        public List<object> Errors { get; set; } = new List<object>();
    }
    public class VoteRequest
    {
        [Required(ErrorMessage = "UserReceiveIDIsRequired")]
        public string UserReceiveID { get; set; }
        [Required(ErrorMessage = "ShareIsRequired")]
        public int? Share { get; set; }
        public string MessageError { get; set; }
    }
}
