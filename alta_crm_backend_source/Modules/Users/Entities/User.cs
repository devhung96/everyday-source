using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Modules.Groups.Enities;
using Project.Modules.Tags.Enities;
using Project.Modules.UsersModes.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Entities
{
    [Table("hd_bank_users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        [Column("group_id")]
        public string GroupId { get; set; }

        [JsonIgnore]
        [Column("user_code_active")]
        public string UserCode { get; set; }
        [Column("user_first_name")]
        public string UserFirstName { get; set; }
        [Column("user_last_name")]
        public string UserLastName { get; set; }
        [Column("user_gender")]
        public int UserGender { get; set; }
        [Column("user_image")]
        public string UserImage { get; set; }
        [Column("user_phone")]
        public string UserPhone { get; set; }
        [Column("user_email")]
        public string UserEmail { get; set; }

        [Column("user_address")]
        public string UserAddress { get; set; }

        [Column("user_status")]
        public int UserStatus { get; set; } = 1;
        [Column("user_created_at")]
        public DateTime UserCreatedAt { get; set; } = DateTime.UtcNow;
        [Column("user_updated_at")]
        public DateTime? UserUpdatedAt { get; set; }
        public Group Group { get; set; }

        [NotMapped]
        public bool IsImportSuccess { get; set; } = true;

        [NotMapped]
        public List<string> ErrorImport { get; set; } = new List<string>();

        [NotMapped]
        public string TagCodeImport { get; set; }
        [NotMapped]
        public string GroupCodeImport { get; set; }



        /// <summary>
        /// String: list id tags của user.
        /// </summary>
        [Column("user_tag_ids")]
        public string UserTagIds { get; set; }

        /// <summary>
        /// Mục đích để tạo ra không cần parse mỗi lần get ra.
        /// </summary>
        [NotMapped]
        public List<string> UserTagIdsParse
        {
            get
            {
                if (!String.IsNullOrEmpty(UserTagIds))
                {
                    return JsonConvert.DeserializeObject<List<string>>(UserTagIds);
                }
                else
                {
                    return new List<string>();
                }
            }
        }

        [NotMapped]
        public List<string> UserImages { get; set; } = new List<string>();

        [NotMapped]
        public List<object> UserObjectImages { get; set; } = new List<object>();

        [NotMapped]
        public List<UserMode> UserModes { get; set; } = new List<UserMode>();

        /// <summary>
        /// Trả về danh sách tag object cho frontend
        /// </summary>
        [NotMapped]
        public List<Tag> UserTags { get; set; } = new List<Tag>();


        /// <summary>
        /// Flat check import customer
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public bool UserOld { get; set; } = false;
    }
    public class CustomerWelCome
    {
        public string CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerImage { get; set; }
        public DateTime? CustomerBirthday { get; set; }
        public string CustomerAddress { get; set; }
        public string KeyWelcome { get; set; }
        public JToken[] KeyCodeValue { get; set; }
        public string KeyDescription { get; set; }
        public int CustomerGender { get; set; }
        public string NamePage { get; set; }
        public string Token { get; set; }
        public CustomerWelCome() { }
        public CustomerWelCome(User customer)
        {
            string gender = "Anh";
            if (customer.UserGender != 1)
            {
                gender = "Chị";
            }
            CustomerId = customer.UserId;
            CustomerCode = customer.UserCode;
            CustomerName = $"{gender} {customer.UserLastName} {customer.UserFirstName}";
            CustomerFirstName = customer.UserFirstName;
            CustomerLastName =  customer.UserLastName;
            CustomerEmail = customer.UserEmail;
            CustomerAddress = customer.UserAddress;
            CustomerGender = customer.UserGender;
        }
    }
}
