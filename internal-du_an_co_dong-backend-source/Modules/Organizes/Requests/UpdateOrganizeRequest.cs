using Microsoft.AspNetCore.Http;
using Project.Modules.Organizes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Requests
{
    public class UpdateOrganizeRequest
    {

        public string OrganizeLogoUrl { get; set; }

        [StringLength(maximumLength: 2000)]
        public string OrganizeName { get; set; }
        public string OrganizeAddress { get; set; }
        public string OrganizeIntroduce { get; set; }

        [StringLength(maximumLength: 2000)]
        public string OrganizeCodeCk { get; set; }
        [StringLength(maximumLength: 2000)]
        public string OrganizeCodeDn { get; set; }

        public double? OrganizeStocks { get; set; }
        [EmailAddress]
        public string OrganizeEmail { get; set; }
        [StringLength(maximumLength: 2048)]
        [CheckUrlAttribute]
        public string OrganizeWebsiteUrl { get; set; }
        [StringLength(maximumLength: 2048)]
        [CheckUrlAttribute]
        public string OrganizeLandingPageUrl { get; set; }
        [StringLength(maximumLength: 2048)]
        [CheckUrlAttribute]
        public string OrganizeFacebookUrl { get; set; }
        public string OrganizeSetting { get; set; }
        [CheckUserExistsAttribute]
        public string UserAdmin { get; set; }
    }
}
