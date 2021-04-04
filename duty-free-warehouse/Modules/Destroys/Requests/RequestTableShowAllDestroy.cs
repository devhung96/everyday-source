using Project.App.Requests;
using Project.Modules.Destroys.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Destroys.Requests
{
    public class RequestTableShowAllDestroy: RequestTable
    {
        [ValidateStringDateTime]
        public string timeBegin { get; set; }

        public string destroyCode { get; set; }

        [ValidateStringDateTime]
        public string timeEnd { get; set; }
        public DateTime timeEndDt { get { if (timeEnd == null) return DateTime.MaxValue; return DateTime.ParseExact(timeEnd + " 23:59:59", "dd/MM/yyyy HH:mm:ss", null); } }
        public DateTime timeBeginDt { get { if (timeBegin == null) return DateTime.MinValue; return DateTime.ParseExact(timeBegin +" 00:00:00" , "dd/MM/yyyy HH:mm:ss", null); } }



    }
}
