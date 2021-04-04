using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Project.Modules.Classes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Project.Modules.Classes.Entities.Class;

namespace Project.Modules.Classes.Requests
{
    [ValidateUpdate]
    public class UpdateClassRequest
    {
        public string ClassID { get; set; }
        public string ClassCode { get; set; }
        public string ClassName { get; set; }
        public int? ClassQuantityStudent { get; set; }
        public string CourseID { get; set; }
        public string SchoolYearID { get; set; }
        public string ClassDescription { get; set; }
        public STATUS_OPEN? Admission { get; set; }
        public IFormFile Image { get; set; }
        public double? ClassAmount { get; set; }
        public string ClassAmountDescription { get; set; }
        public string Surcharge { get; set; }
        public List<UpdateSurcharge> SurchargeData
        {
            get
            {
                if (!String.IsNullOrEmpty(Surcharge))
                {
                    return JsonConvert.DeserializeObject<List<UpdateSurcharge>>(Surcharge);
                }
                return null;
            }
            set { }
        }
    }
    public class UpdateSurcharge
    {
        public string SurchargeId { get; set; }
        [Required]
        public string SurchargeName { get; set; }
        [Required]
        public double? SurchargeAmount { get; set; }
        public bool CheckAmount => SurchargeAmount.Value > 0 && SurchargeAmount <= double.MaxValue;
    }
}
