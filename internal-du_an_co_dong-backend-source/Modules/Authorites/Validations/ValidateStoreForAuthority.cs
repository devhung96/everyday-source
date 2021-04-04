using Microsoft.AspNetCore.Http;
using Project.App.Database;
using Project.Modules.Authorites.Requests;
using Project.Modules.Authorities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Authorites.Validations
{
    public class ValidateStoreForAuthority : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MariaDBContext MariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            StoreForAuthority storeRequest = value as StoreForAuthority;
            #region Set EventID from header
            var IHttpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            if (String.IsNullOrEmpty(IHttpContextAccessor.HttpContext.Request.Headers["Event-Id"].ToString()))
                return new ValidationResult("EventNotFound");
            storeRequest.EventID = IHttpContextAccessor.HttpContext.Request.Headers["Event-Id"].ToString();
            #endregion
            #region Check user must exists
            var checkEventTime = MariaDB.Events.FirstOrDefault(x => x.EventId.Equals(storeRequest.EventID) && x.EventFlag == Events.Entities.EVENT_FLAG.CREATED);
            if (checkEventTime is null)
                return new ValidationResult("TheEventHasStartedOrHasEnded");
            var userAuthorityEvent = MariaDB.EventUsers.FirstOrDefault(x => x.EventId.Equals(storeRequest.EventID) && x.UserId.Equals(storeRequest.UserID));
            if (userAuthorityEvent is null)
                return new ValidationResult("UserDoesNotExistInThisEvent");
            if (userAuthorityEvent.UserStock <= 0)
                return new ValidationResult("NumberOfAuthorizedSharesIsNotValid");
            var userReceiveEvent = MariaDB.EventUsers.FirstOrDefault(x => x.EventId.Equals(storeRequest.EventID) && x.UserId.Equals(storeRequest.UserReceiveID));
            if (userReceiveEvent is null)
                return new ValidationResult("AuthorizedRecipientDoesNotExistInTheEvent");
            #endregion

            #region Check authority exists
            // User đã ủy quyền cho người khác thì không thể ủy quyền hoặc nhận ủy quyền nữa
            var checkAuthorityExistsFullEvent = MariaDB.Authorities.FirstOrDefault(x =>
                x.EventID == storeRequest.EventID
                && (x.AuthorityUserID == storeRequest.UserID || x.AuthorityUserID == storeRequest.UserReceiveID)
                && x.AuthorityType == AuthorityType.EVENT
                );
            if (checkAuthorityExistsFullEvent != null)
                return new ValidationResult("UserHasPerformedAuthorizationDuringThisEvent");

            #endregion
            if (storeRequest.UserID.Equals(storeRequest.UserReceiveID))
                return new ValidationResult("CanNotAuthorizeYourself");

            #region check circle
            var checkCircle = MariaDB.Authorities.FirstOrDefault(x =>
                x.EventID == storeRequest.EventID
                && x.AuthorityUserID == storeRequest.UserReceiveID
                && x.AuthorityReceiveUserID == storeRequest.UserID
                );
            if (checkCircle != null)
                return new ValidationResult("ThisUserCannotBeAuthorized");
            #endregion

            return ValidationResult.Success;
        }
    }
}
