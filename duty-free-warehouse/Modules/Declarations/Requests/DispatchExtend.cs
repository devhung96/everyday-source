using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Requests
{
    public class DispatchExtend
    {
        [Required]
        public string DeclaNumber { get; set; }
        [Required]
        public string DispatchNumber { get; set; }
        [Required]
        public string DeClaRenewalDate { get; set; }
        public DateTime DeClaRenewalDateData { get { return DateTime.ParseExact(DeClaRenewalDate, "dd/MM/yyyy", null); } }
        public string DispatchDate { get; set; }
        public DateTime? DispatchDateData
        {
            get
            {
                if (!String.IsNullOrEmpty(DispatchDate)) return DateTime.ParseExact(DispatchDate, "dd/MM/yyyy", null);
                else return null;
            }
        }
    }
}
