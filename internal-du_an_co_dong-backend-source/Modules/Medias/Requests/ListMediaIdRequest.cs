using Project.Modules.Medias.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class ListMediaIdRequest
    {
        [CheckListIdsValidation]
        public List<string> MediaIds { get; set; }
    }
}
