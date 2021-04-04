using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Destroys.Requests
{
    public class StoreDestroyDetailRequest
    {
        [Required]
        public string DestroyCode  { get; set; }
        [Required]
        public List<ChildDestroyDetail> ChildDestroyDetails { get; set; }
        public string DestroyDate { get; set; }
    }

    public class ChildDestroyDetail
    {
        public string DeNumber { get; set; }
        public string ProductCode { get; set; }
        public int DestroyDetailQuantity { get; set; }
        public string DestroyDetailNote { get; set; }
    }
}
