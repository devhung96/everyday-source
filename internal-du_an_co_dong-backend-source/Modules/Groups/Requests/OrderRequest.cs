using Project.Modules.Groups.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Requests
{
    public class OrderRequest
    { 
        public List<Group> Groups { get; set; }
    }
}
