using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Entities
{
    public class ChartForEvent
    {
        public int totalNumberOfShareholders { get; set; }
        public int totalShares { get; set; }
        public int totalNumberOfShareholdersAttending { get; set; }
        public int totalNumberOfSharesRepresented { get; set; }
        public int totalStock { get; set; }
    }
}
