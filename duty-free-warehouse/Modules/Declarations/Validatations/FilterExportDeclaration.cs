using Project.Modules.DeClarations.Entites;
using Project.Modules.Destroys.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Validatations
{
    public class FilterExportDeclaration
    {
        public string DeclarationNumber { get; set; }

        [ValidateStringDateTime]
        public string DateFrom { get; set; }

        [ValidateStringDateTime]
        public string DateTo { get; set; }
    }
}
