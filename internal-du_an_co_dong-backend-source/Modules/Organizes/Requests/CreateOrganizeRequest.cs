using Microsoft.AspNetCore.Http;
using Project.Modules.Events.Validations;
using Project.Modules.Organizes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Requests
{
    /// <summary>
    /// + Có phải Email không
    /// + OrganizeWebsiteUrl : website k 
    /// + OrganizeLandingPageUrl : website k 
    /// + OrganizeFacebookUrl : website k 
    /// + UserAdmin : Có tồn tại chưa
    /// </summary>
    public class CreateOrganizeRequest
    {
        [AllowedExtensionsUrl(new string[] { ".png", ".sgv", ".jpg",".jpge" })]
        public string OrganizeLogoUrl { get; set; }

        //[DataType(DataType.Upload)]
        //[MaxFileSizeAttribute(200)]
        //[AllowedExtensions(new string[] { ".png", ".sgv", ".jpg" })]
        //public IFormFile OrganizeLogo { get; set; }
        [Required(ErrorMessage = "OrganizeNameIsRequired")]
        [StringLength(maximumLength: 2000, ErrorMessage = "OrganizeNameMaxLength2000")]
        public string OrganizeName { get; set; }

        public string OrganizeAddress { get; set; }
        public string OrganizeIntroduce { get; set; }
        [Required(ErrorMessage = "OrganizeCodeCkIsRequired")]
        [StringLength(maximumLength:2000, ErrorMessage = "OrganizeCodeCkMaxLength2000")]
        [CheckOrganizeCodeCkUniqueAttribute]
        public string OrganizeCodeCk { get; set; } // Mã số chứng khoán

        [StringLength(maximumLength: 2000, ErrorMessage = "OrganizeCodeDnMaxLength2000")]
        public string OrganizeCodeDn { get; set; } // Mã số doanh nghiệp
        [Required(ErrorMessage = "OrganizeStocksIsRequired")]
        public double? OrganizeStocks { get; set; }
        [EmailAddress]
        public string OrganizeEmail { get; set; }
        [StringLength(maximumLength: 2048, ErrorMessage = "OrganizeWebsiteUrlMaxLength2048")]
        [CheckUrlAttribute]
        public string OrganizeWebsiteUrl { get; set; }

        [StringLength(maximumLength: 2048, ErrorMessage = "OrganizeLandingPageUrlMaxLength2048")]
        [CheckUrlAttribute]
        public string OrganizeLandingPageUrl { get; set; }

        [StringLength(maximumLength: 2048, ErrorMessage = "OrganizeFacebookUrlMaxLength2048")]
        [CheckUrlAttribute]
        public string OrganizeFacebookUrl { get; set; }

        public string OrganizeSetting { get; set; }

        [CheckUserExistsAttribute]
        public string UserAdmin { get; set; }
    }
}
