using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class LoginRequest
    {
        //[DataType(DataType.EmailAddress, ErrorMessage = "UserEmailInValid")]
        /// <summary>
        /// 
        /// </summary>
        ///<example>noreply@alta.com.vn</example>
        public string UserEmail { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <example>123123123</example>
        public string Password { get; set; }
    }
}
