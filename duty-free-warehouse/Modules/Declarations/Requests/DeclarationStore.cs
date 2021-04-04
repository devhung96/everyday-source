using Newtonsoft.Json.Linq;
using Project.Modules.Declarations.Validatations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Requests
{
    public class DeclarationStore
    {
        [Required]
        [ValidateDeclanumberAttribute]
        public string DeClaNumber { get; set; }

        [Required]
        public string DeClaDateRe { get; set; }

        [Required]
        public int Type { get; set; }
        [ValidateDateAddAndExport]
        public JObject Content { get; set; }
        [ValidateParentDeClarationAttribute]
        public string DeClaParentNumber { get; set; }


        
        public DateTime DeClaDateReData { get { return DateTime.ParseExact(DeClaDateRe, "dd/MM/yyyy", null); } }
    }
}
