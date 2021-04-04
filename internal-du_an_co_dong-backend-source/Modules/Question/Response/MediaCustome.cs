using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Response
{
    public class MediaCustome
    {
        [Required]
        public string MediaID { get; set; }
        [Required]
        public string MediaName { get; set; }
        [Required]
        public string MediaPath { get; set; }
        [Required]
        public string MediaTitle { get; set; }
        [Required]
        public string MediaType { get; set; }
        public string MediaAltText { get; set; }
        public string MediaTag { get; set; }
        public string MediaDescription { get; set; }
        [Required]
        public string OrganizeId { get; set; }
        public string ParentId { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        public string UserId { set; get; }
        [Required]
        public string MediaURL { get; set; }
        public string UserName { set; get; }
        public string LocalPath { get; set; }
        public string urlCustome { get; set; }
    }
}
