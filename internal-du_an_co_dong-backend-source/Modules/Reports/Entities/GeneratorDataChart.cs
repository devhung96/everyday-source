using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Reports.Entities
{
    public class GeneratorDataChart
    {
        public bool displayShowChart { get; set; } = false;
        public object result { get; set; }
        public object sum { get; set; }
        public TotalDataChart total { get; set; }
        public string note { get; set; }
    }

    public class TotalDataChart
    {
        public int totalCumulativeVotes { get; set; }
        public int totalPeopleSubmit { get; set; }
        public int totalPeople { get; set; }
        public int totalNumberOfSharesRepresented { get; set; }
        public int totalShares { get; set; }
        public int totalStock { get; set; }
    }
}
