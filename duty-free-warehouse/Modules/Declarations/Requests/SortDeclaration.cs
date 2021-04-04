using Project.App.Requests;
using Project.Modules.Declarations.Validatations;
using Project.Modules.Destroys.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Requests
{
    public class SortDeclaration : RequestTable
    {
        public string DeclarationNumber { get; set; }
        [ValidateStringDateTime]
        public string DateRegisterFrom { get; set; }
        [ValidateStringDateTime]
        public string DateRegisterTo { get; set; }
        [ValidateStringDateTime]
        public string DeadlineFrom { get; set; }
        [ValidateStringDateTime]
        public string DeadlineTo { get; set; }
        public int? Type { get; set; }
        public DateTime RegisterFromData { get { if (DateRegisterFrom == null || String.IsNullOrEmpty(DateRegisterFrom)) return DateTime.MinValue; return DateTime.ParseExact(DateRegisterFrom , "dd/MM/yyyy", null); }}
        public DateTime RegisterToData { get { if (DateRegisterTo == null || String.IsNullOrEmpty(DateRegisterTo)) return DateTime.MaxValue; return DateTime.ParseExact(DateRegisterTo, "dd/MM/yyyy", null); } }
        public DateTime? DeadlineFromData { get { if (DeadlineFrom == null || String.IsNullOrEmpty(DeadlineFrom)) return null; return DateTime.ParseExact(DeadlineFrom, "dd/MM/yyyy", null); } }
        public DateTime? DeadlineToData { get { if (DeadlineTo == null || String.IsNullOrEmpty(DeadlineTo)) return null; return DateTime.ParseExact(DeadlineTo, "dd/MM/yyyy", null); } }
    }
}
