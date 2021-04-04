using Microsoft.AspNetCore.Http;
using Project.App.Validations;
using Project.Modules.Classes.Validations;
using Project.Modules.Students.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.Students.Entities.Student;

namespace Project.Modules.Students.Requests
{
    public class AddStudent
    {
        //[CodeValidation]
        //[Required]
      //  public string StudentCode { get; set; }
        [ValidateLastName]
        [Required]
        public string StudentLastName { get; set; }
        [ValidateFistName]
        [Required]
        public string StudentFirstName { get; set; }
        [EmailValidation]
        public string StudentEmail { get; set; }
        [Required]
        [ValidateBirthday]
        public DateTime StudentBirthday { get; set; }
        [Required]
        public GENDER StudentGender { get; set; }
        public string StudentAddress { get; set; }
        [ValidationPhoneNumber]
        public string StudentPhone { get; set; }
        public IFormFile Image { get; set; }
        [ValidateName]
        public string ParentName { get; set; }
        [Required]
        public string Password { get; set; }
        public AddStudent() { }
        public AddStudent(AddRegistration reg)
        {
            StudentLastName = reg.StudentLastName;
            StudentFirstName = reg.StudentFirstName;
            StudentEmail = reg.StudentEmail;
            StudentBirthday = reg.StudentBirthday;
            StudentAddress = reg.StudentAddress;
            StudentPhone = reg.StudentPhone;
            Image = reg.Image;
            ParentName = reg.ParentName;
            Password = reg.Password;
            StudentGender = reg.StudentGender;
        }
    }
    [ValidateAddRegistration]
    public class AddRegistration
    {
        public string ClassId { get; set; }
        public string StudentId { get; set; }
        [ValidateLastName]
        public string StudentLastName { get; set; }
        [ValidateFistName]

        public string StudentFirstName { get; set; }
        [EmailValidation]
        public string StudentEmail { get; set; }
        [ValidateBirthday]
        public DateTime StudentBirthday { get; set; }
        public GENDER StudentGender { get; set; }
        public string StudentAddress { get; set; }
        [ValidationPhoneNumber]
        public string StudentPhone { get; set; }
        public IFormFile Image { get; set; }
        [ValidateName]
        public string ParentName { get; set; }
        public string Password { get; set; }
        //    public AddStudent addStudent { get; set; }
    }
    public class UpdateStudentRequest
    {
        [ValidateName]
        public string StudentLastName { get; set; }
        [ValidateName]
        public string StudentFirstName { get; set; }
        [EmailValidation]
        public string StudentEmail { get; set; }
        [ValidateBirthday]
        public DateTime StudentBirthday { get; set; }
        public string StudentAddress { get; set; }
        [ValidationPhoneNumber]
        public string StudentPhone { get; set; }
        public IFormFile Image { get; set; }
        [ValidateName]
        public string ParentName { get; set; }
    }
    [ValidationDeleteStudentFromClass]
    public class DeleteStudentFromClassRequest
    {
        [Required]
        public string ClassId { get; set; }
        [Required]
        public string StudentId { get; set; }
    }
}
