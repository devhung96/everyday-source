using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Project.Modules.Classes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Classes.Requests
{
    [ValidateStore]
    public class StoreClassRequest
    {
        [Required]
        public string ClassCode { get; set; }
        [Required]
        public string ClassName { get; set; }
        [Required]
        public int? ClassQuantityStudent { get; set; }
        [Required]
        public string CourseID { get; set; }
        [Required]
        public string SchoolYearID { get; set; }
        public string ClassDescription { get; set; }
        public IFormFile ClassImage { get; set; }
        [Required]
        public double? ClassAmount { get; set; }
        public string ClassAmountDescription { get; set; }
        public string Surcharge { get; set; }
        public List<StoreSurchargeRequest> SurchargeData 
        {
            get
            {
                if (!String.IsNullOrEmpty(Surcharge))
                {
                    return JsonConvert.DeserializeObject<List<StoreSurchargeRequest>>(Surcharge);
                }
                return null;
            }
            set { }
        }
    }
    public class StoreSurchargeRequest
    {
        [Required]
        public string SurchargeName { get; set; }
        [Required]
        public double? SurchargeAmount { get; set; }
        public bool CheckAmount => SurchargeAmount.Value > 0 && SurchargeAmount <= double.MaxValue;
    }
}
