using Project.App.Requests;
using Project.Modules.Declarations.Validatations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Requests
{
    public class TrackLiquidityRequest : RequestTable
    {
        // Thêm mới trường type để export excel : devhung
        [Required]
        public int? Type { get; set; }
    }
}
