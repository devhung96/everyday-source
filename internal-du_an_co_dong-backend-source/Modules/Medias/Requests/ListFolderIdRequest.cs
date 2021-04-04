using Project.Modules.Medias.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class ListFolderIdRequest
    {
        [CheckListIdsValidation]
        public List<string> FolderIds { get; set; }
    }
}
