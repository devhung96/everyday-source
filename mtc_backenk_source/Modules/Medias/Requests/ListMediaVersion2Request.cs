using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class ListMediaVersion2Request : PaginationRequest
    {
        public string TypeMedia { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
    }
}
