using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Modules.Seals.Request
{
    public class ReportExportRequest
    {
        public string Search { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public DateTime? DateFromObj 
        { 
            get
            {
                if (!DateTime.TryParseExact(DateFrom, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                    return null;
                return dateTime;
            }
            set
            {
                DateFrom = value?.ToString("dd/MM/yyyy");
            }
        }
        public DateTime? DateToObj
        {
            get
            {
                if (!DateTime.TryParseExact(DateTo, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
                    return null;
                return dateTime;
            }
            set
            {
                DateFrom = value?.ToString("dd/MM/yyyy");
            }
        }
    }
}