using Project.App.Database;
using Project.Modules.Authorites.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Authorites.Validations
{
    public class ValidateUpdateForAuthority : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            UpdateForAuthorityWithID request = value as UpdateForAuthorityWithID;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            var authority = mariaDB.Authorities.Find(request.AuthorityID.Value);
            if (authority is null)
                return new ValidationResult("AuthorizationDoesNotExist");
            #region Check user Receive must contains Event
            var checkEventUser = mariaDB.EventUsers.FirstOrDefault(x => x.EventId.Equals(authority.EventID) && x.UserId.Equals(request.UserReceive));
            if (checkEventUser is null)
                return new ValidationResult("UserDidNotExistInTheEvent");
            #endregion
            #region check user existed in this event
            var checkAuthorityEvent = mariaDB.Authorities.FirstOrDefault(x => x.EventID.Equals(authority.EventID) && x.AuthorityUserID.Equals(request.UserReceive));
            if (checkAuthorityEvent != null)
                return new ValidationResult("UserHasPerformedAuthorizationDuringThisEvent");
            #endregion
            return ValidationResult.Success;
        }
    }
}
