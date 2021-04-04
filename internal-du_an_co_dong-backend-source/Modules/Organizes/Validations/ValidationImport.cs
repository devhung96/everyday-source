using Project.App.Database;
using Project.Modules.Organizes.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Validations
{
    public class ValidationImport : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ImportRequest request = value as ImportRequest;
            List<ItemImportUserToEvent> listUser = request.ImportUsers.ToList();
            MariaDBContext maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
           
            var group = maria.Groups.Find(request.GroupId);
            if (group is null)
            {
                return new ValidationResult("GroupIdNotExist");
            }
            
            var _event = maria.Events.Find(request.EventId);
            //if (_event is null)
            //{
            //    return new ValidationResult("Sự kiện không tồn tại.");
            //}
            
            List<string> shareholderCodes =new List<string>();
            
            foreach (var item in listUser)
            {
                if (!shareholderCodes.Contains(item.ShareholderCode))
                {
                    shareholderCodes.Add(item.ShareholderCode);
                    if (!DateTime.TryParseExact(item.IssueDate, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                    {
                        return new ValidationResult("Ngày cấp CMND truyền vào không hợp lệ.");
                    }
                }
                else
                {
                    return new ValidationResult("ShareholderCodeAreadlyExist");
                }
              
            }
            return ValidationResult.Success;
        }
    }
}
