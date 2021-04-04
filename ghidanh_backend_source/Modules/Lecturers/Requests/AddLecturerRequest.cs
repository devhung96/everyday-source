using Microsoft.AspNetCore.Http;
using Project.App.Validations;
using Project.Modules.Lecturers.Validations;
using Project.Modules.Students.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Project.Modules.Students.Entities.Student;

namespace Project.Modules.Lecturers
{
    public class AddLecturerRequest
    {
        [Required]
        [CodeValidation]
        [ValidateLecturerCode]
        public string LecturerCode { get; set; }
        [Required, ValidateName]
        public string LecturerLastName { get; set; }
        [Required, ValidateName]
        public string LecturerFistName { get; set; }
        public string LecturerTaxCode { get; set; }
        [Required]
        [ValidateBirthday]
        public DateTime LecturerBirthday { get; set; }
        [Required]
        public GENDER LecturerGender { get; set; }
        [Required]
        [EmailValidation]
        [ValidateLecturerEmail]
        public string LecturerEmail { get; set; }
        [Required, ValidationPhoneNumber, ValidateLecturerPhone]
        public string LecturerPhone { get; set; }
        public string LecturerAddress { get; set; }
        [ValidateSubjectId]
        public string SubjectId { get; set; }
        public IFormFile Image { get; set; }
        [Required]
        public string Password { get; set; }
        [ValidateExtraSubjectIds]
        public List<string> ExtraSubjectIds { get; set; } = new List<string>();
    }
    public class UpdateLecturerRequest
    {
        [ValidateName]
        public string LecturerLastName { get; set; }
        [ValidateName]
        public string LecturerFistName { get; set; }
        public string LecturerTaxCode { get; set; }
        [ValidateBirthday]
        public DateTime LecturerBirthday { get; set; }
        public GENDER LecturerGender { get; set; }
        [EmailValidation]
        public string LecturerEmail { get; set; }
        [ValidationPhoneNumber]
        public string LecturerPhone { get; set; }
        public string LecturerAddress { get; set; }
        [ValidateSubjectId]
        public string SubjectId { get; set; }
        public IFormFile Image { get; set; }
        [ValidateExtraSubjectIds]
        public List<string> ExtraSubjectIds { get; set; } = new List<string>();
    }
}
