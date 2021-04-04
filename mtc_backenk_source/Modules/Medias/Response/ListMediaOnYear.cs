using Project.Modules.Medias.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Medias.Response
{
    public class ListMediaOnYear
    {
        public int Year { get; set; }
        public List<DataOnMonth> Months { get; set; }

        public ListMediaOnYear(int year, IEnumerable<Media> medias)
        {
            Year = year;
            Months = medias.GroupBy(x => x.CreateAt.Month).Select(x => new DataOnMonth
            {
                Month = x.Key,
                CountData = x.Count()
            }).ToList();
        }
    }

    public class DataOnMonth
    {
        public int Month { get; set; }
        public int CountData { get; set; }
    }
}
