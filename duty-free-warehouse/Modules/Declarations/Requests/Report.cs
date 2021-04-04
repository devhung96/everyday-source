using Project.App.Requests;
using Project.Modules.Declarations.Validatations;
using Project.Modules.Destroys.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Requests
{
    public class Report : RequestTable
    {
        [Required]
        public int? Type { get; set; }
        [ValidateStringDateTime]
        public string DateRegisterFrom { get; set; }
        [ValidateStringDateTime]
        public string DateRegisterTo { get; set; }
        public DateTime RegisterFromData { get { if (DateRegisterFrom == null || String.IsNullOrEmpty(DateRegisterFrom)) return DateTime.MinValue; return DateTime.ParseExact(DateRegisterFrom, "dd/MM/yyyy", null); } }
        public DateTime RegisterToData { get { if (DateRegisterTo == null || String.IsNullOrEmpty(DateRegisterTo)) return DateTime.MaxValue; return DateTime.ParseExact(DateRegisterTo, "dd/MM/yyyy", null); } }
    }
}
