using Project.App.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Modules.PlayLists.Entities;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.PlayLists.Requests
{
    public class StorePlayListRequest 
    {
        [Required]
        public string PlayListName { get; set; }
        public string PlayListComment { get; set; }
        public PlayListLoopEnum PlayListLoop { get; set; }
        public string PlayListAssignUserId { get; set; }
    }
}
