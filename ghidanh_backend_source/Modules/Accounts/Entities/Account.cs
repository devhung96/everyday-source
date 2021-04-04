using Project.Modules.Lecturers.Entities;
using Project.Modules.Students.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.Students.Entities.Student;

namespace Project.Modules.Accounts.Entities
{
    [Table("rc_accounts")]
    public class Account
    {
        [Key]
        [Column("account_id")]
        public string AccountId { get; set; } = Guid.NewGuid().ToString();
        [Column("account_code")]
        public string AccountCode { get; set; }
        [Column("account_password")]
        public string Password { get; set; }
        [Column("account_saft")]
        public string Saft { get; set; }
        [Column("login_at")]
        public DateTime? LoginAt { get; set; }
        [Column("account_update")]
        public DateTime? AccountUpdate { get; set; } 
        [Column("account_type")]
        public TYPE_ACCOUNT AccountType{ get; set; }  
        [Column("group_code")]
        public string GroupCode{ get; set; }
        [NotMapped]
        public string Token { get; set; }

        public enum TYPE_ACCOUNT
        {
            CMS =0,
            STUDENT=1,
            LECTURER=2,
            DEFAULT = 3
        }
    }
    public class AccountResponse
    {
        public string AccountId { get; set; }
        public string AccountCode { get; set; }
        public DateTime? LoginAt { get; set; }
        public string Token { get; set; }
        public AccountResponse(Account account)
        {
            AccountId = account.AccountId;
            AccountCode = account.AccountCode;
            LoginAt = account.LoginAt;
            Token = account.Token;
         }
    }
 
    public class Personal
    {
        public string LastName { get; set; }
        public string FisrtName { get; set; }
        public GENDER Gender { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string AccountId { get; set; }

        public Personal() { }
        public Personal(Student s)
        {
            LastName = s.StudentLastName;
            FisrtName = s.StudentFirstName;
            Gender = s.StudentGender;
            Code = s.StudentCode;
            Image = s.StudentImage;
            Email = s.StudentEmail;
            Birthday = s.StudentBirthday;
            Address = s.StudentAddress;
            Phone = s.StudentPhone;
            AccountId = s.AccountId;
    }
        public Personal(Lecturer l)
        {
            LastName = l.LecturerLastName;
            FisrtName = l.LecturerFistName;
            Gender = l.LecturerGender;
            Code = l.LecturerCode;
            Image = l.LecturerImage;
            Email = l.LecturerEmail;
            Birthday = l.LecturerBirthday;
            Address = l.LecturerAddress;
            Phone = l.LecturerPhone;
            AccountId = l.AccountId;
        } 
        public Personal(User u)
        {
            FisrtName = u.UserName;
            Code = u.UserEmail;
            Image = u.UserImage;
            Email = u.UserEmail;
            AccountId = u.AccountId;
            LastName = "";
            Address = "";
            Phone = "";
        }
    }
}
