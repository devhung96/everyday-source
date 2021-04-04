using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Reports.Entities
{
    public class Report
    {
        public int TotalDelegate { get; set; } = 0;
        public int TotalQuestion { get; set; } = 0;
        public int TotalVoting { get; set; } = 0;
        public List<ItemSession> Sessions { get; set; }

       
        public List<DelegateAlta> Delegates { get; set; } 

    }

    public class ItemSession
    {
        public List<string> QuestionIds { get; set; }
        public int SessionSort { get; set; }
        public string SessionName { get; set; }
        public int NumberOfParticipants { get; set; }
        public int NumberOfQuestionsDiscussed { get; set; }
        public int NumberOfVotingQuestions { get; set; }
        public object DataExport { get; set; }
    }

    public class DelegateAlta
    {
        public object InforDelegateAlta { get; set; }
    }
}
