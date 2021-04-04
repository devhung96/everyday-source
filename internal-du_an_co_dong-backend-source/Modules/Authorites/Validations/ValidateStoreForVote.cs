using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.Authorities.Entities;
using Project.Modules.Authorities.Requests;
using Project.Modules.Events.Entities;
using Project.Modules.Question.Request;
using Project.Modules.Question.Response;
using Project.Modules.Question.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Project.Modules.Authorities.Validations
{
    public class ValidateStoreForVote : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MariaDBContext MariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            IQuestionAnswersService IQuestionAnswersService = (IQuestionAnswersService)validationContext.GetService(typeof(IQuestionAnswersService));
            StoreForVoteRequest storeRequest = value as StoreForVoteRequest;
            var checkEvent = MariaDB.Events.FirstOrDefault(x => x.EventId.Equals(storeRequest.EventID) /*&& x.EventFlag == EVENT_FLAG.BEGIN*/);
            if (checkEvent is null)
                return new ValidationResult("TheEventHasNotTakenPlaceOrHasEnded.");
            var checkUserExistInEvent = MariaDB.Authorities
                .FirstOrDefault(x => x.EventID.Equals(storeRequest.EventID)
                && (x.AuthorityUserID.Equals(storeRequest.UserID) || storeRequest.VoteRequests.Select(x => x.UserReceiveID).Contains(x.AuthorityUserID))
                && x.AuthorityType == AuthorityType.EVENT);
            if (checkUserExistInEvent != null)
                return new ValidationResult("YouHavedoneACumulativeLoadInThisQuestion");
            var checkExistAuthorityVote = MariaDB.Authorities.FirstOrDefault(x => x.EventID.Equals(storeRequest.EventID) && x.QuestionID.Equals(storeRequest.QuestionID) && x.AuthorityUserID.Equals(storeRequest.UserID));
            if (checkExistAuthorityVote != null)
                return new ValidationResult("YouHavedoneATumpageInThisQuestion");
            bool anyDuplicate = storeRequest.VoteRequests.GroupBy(x => x.UserReceiveID).Any(g => g.Count() > 1);
            if (anyDuplicate)
                return new ValidationResult("AuthorizedRecipientsCannotBeDuplicated");
            var userAuthorityEvent = MariaDB.EventUsers.FirstOrDefault(x => x.EventId.Equals(storeRequest.EventID) && x.UserId.Equals(storeRequest.UserID));
            if (userAuthorityEvent is null)
                return new ValidationResult("UserDidNotExistInTheEvent");
            int userShare = MariaDB.EventUsers.FirstOrDefault(x => x.UserId.Equals(storeRequest.UserID) && x.EventId.Equals(storeRequest.EventID)).UserStock // tổng cổ phần cá nhân
                + MariaDB.Authorities.Where(x => x.AuthorityReceiveUserID.Equals(storeRequest.UserID)
                && x.EventID.Equals(storeRequest.EventID)
                && x.AuthorityType == AuthorityType.EVENT)
                .Sum(x => x.AuthorityShare).Value //tổng cổ phần nhận ủy quyền (nếu có)
                + MariaDB.Authorities.Where(x => x.AuthorityReceiveUserID.Equals(storeRequest.UserID)
                && x.EventID.Equals(storeRequest.EventID)
                && x.QuestionID.Equals(storeRequest.QuestionID))
                .Sum(x => x.AuthorityShare).Value; //tổng cổ phần nhận bầu dồn (nếu có)
            int totalShare = storeRequest.VoteRequests.Sum(x => x.Share).Value;
            if (userShare != totalShare || totalShare <= 0)
                //return new ValidationResult($"Số cổ phần không hợp lệ. Số cổ phần phải là: {userShare}");
                return new ValidationResult($"InvalidNumberOfShares");

            //check người dùng nhập ngu
            var ngu = storeRequest.VoteRequests.FirstOrDefault(x => x.UserReceiveID.Equals(storeRequest.UserID));
            if (ngu != null)
                return new ValidationResult("YouCannotVoteForYourself");
            //var checkDouble = MariaDB.Authorities.FirstOrDefault(x => x.EventID.Equals(storeRequest.EventID)
            //&& x.QuestionID.Equals(storeRequest.QuestionID)
            //&& storeRequest.VoteRequests.Select(x => x.UserReceiveID).Contains(x.AuthorityUserID)
            //);

            //if (checkDouble != null)
            //{
            //    storeRequest.Errors.Add(storeRequest.VoteRequests.FirstOrDefault(x => x.UserReceiveID.Equals(checkDouble.AuthorityUserID)));
            //    //return new ValidationResult($"{checkDouble.AuthorityUserID} không thể nhận bầu dồn phiếu.");
            //}

            foreach (var item in storeRequest.VoteRequests)
            {
                #region kiểm tra người nhận đã trả lời câu hỏi chưa
                GetResultSubmit getResultSubmit = new GetResultSubmit();
                getResultSubmit.QuestionID = storeRequest.QuestionID;
                var result = IQuestionAnswersService.GetResultSubmitVoid(getResultSubmit);
                if (result.result != null)
                {
                    var ListcheckAnswered = (List<ResponseResultSubmit>)result.result;
                    // check người nhận đã trả lời rồi
                    var checkUserIDAnswered = ListcheckAnswered.FirstOrDefault(x => x.ContentJson.UserID.Equals(storeRequest.UserID));
                    if (checkUserIDAnswered != null)
                    {
                        item.MessageError = "ThisUserAnsweredTheQuestion";
                        storeRequest.Errors.Add(item);
                        continue;
                    }

                    // check người nhận đã trả lời rồi
                    var checkReceiveAnswered = ListcheckAnswered.FirstOrDefault(x => x.ContentJson.UserID.Equals(item.UserReceiveID));
                    if(checkReceiveAnswered != null)
                    {
                        item.MessageError = "ThisUserAnsweredTheQuestion";
                        storeRequest.Errors.Add(item);
                        continue;
                    }
                }
                #endregion

                var checkDouble = MariaDB.Authorities.FirstOrDefault(x => x.EventID.Equals(storeRequest.EventID) // tránh trường hợp bầu dồn cùng 1 lúc
                && x.QuestionID.Equals(storeRequest.QuestionID)
                && item.UserReceiveID.Equals(x.AuthorityUserID)
                );
                if (checkDouble != null)
                {
                    item.MessageError = "ThisUserVotedForOthers";
                    storeRequest.Errors.Add(item);
                    continue;
                }

                #region Check user exists
                var userReceiveEvent = MariaDB.EventUsers.FirstOrDefault(x => x.EventId.Equals(storeRequest.EventID) && x.UserId.Equals(item.UserReceiveID));
                if (userReceiveEvent is null)
                {
                    item.MessageError = "RecipientsStumpedVotesDidNotExistInTheEvent";
                    storeRequest.Errors.Add(item);
                    continue;
                    //return new ValidationResult("Người nhận bầu dồn phiếu không tồn tại trong sự kiện.");
                }
                #endregion

                #region Check Duplicate question in a event
                var checkQuestion = MariaDB.Authorities.FirstOrDefault(x =>
                x.EventID == storeRequest.EventID
                && x.AuthorityReceiveUserID.Equals(item.UserReceiveID)
                && x.AuthorityUserID.Equals(storeRequest.UserID)
                && x.QuestionID == storeRequest.QuestionID);
                if (checkQuestion != null)
                {
                    item.MessageError = "ThisCumulativeVoteAlreadyExists";
                    storeRequest.Errors.Add(item);
                    continue;
                    //return new ValidationResult("Bầu dồn phiếu này đã tồn tại.");
                }
                #endregion


                #region Check user can't receive more authorized
                //var checkAmountUserReceive = MariaDB.Authorities
                //    .FirstOrDefault(x => x.EventID.Equals(storeRequest.EventID)
                //    && x.QuestionID.Equals(storeRequest.QuestionID)
                //    && x.AuthorityReceiveUserID.Equals(item.UserReceiveID));
                //if (checkAmountUserReceive != null)
                //    return new ValidationResult("Người nhận bầu dồn phiếu này đã tồn tại.");
                #endregion

                #region Check Rotate the loop
                //var checkCircle = MariaDB.Authorities.FirstOrDefault(x =>
                //   x.EventID == storeRequest.EventID
                //    && x.AuthorityReceiveUserID.Equals(storeRequest.UserID)
                //    && x.AuthorityUserID.Equals(item.UserReceiveID)
                //    && x.QuestionID == storeRequest.QuestionID
                //    );
                //if (checkCircle != null)
                //{
                //    storeRequest.Errors.Add(item);
                //    continue;
                //    //return new ValidationResult("Người dùng không thể bầu dồn cho người dùng nhận này.");
                //}
                #endregion

                #region Check Share
                //var userShare = MariaDB.EventUsers.FirstOrDefault(x => x.UserId.Equals(storeRequest.UserID));
                //if (userShare is null)
                //    return new ValidationResult("Người dùng không tồn tại trong sự kiện.");
                //int totalShare = item.Share.Value + MariaDB.Authorities
                //    .Where(x => x.EventID.Equals(storeRequest.EventID)
                //    && x.QuestionID.Equals(storeRequest.QuestionID)
                //    && x.AuthorityUserID.Equals(storeRequest.UserID))
                //    .Sum(x => x.AuthorityShare.Value);
                //if (totalShare > userShare.UserStock)
                //    return new ValidationResult("Số cổ phần vượt mức cho phép.");
                if (totalShare <= 0)
                {
                    item.MessageError = "InvalidNumberOfShares";
                    storeRequest.Errors.Add(item);
                    continue;
                    //return new ValidationResult("Số cổ phần không hợp lệ.");
                }
                #endregion
            }

            #region Xóa câu trả lời nếu thêm bầu dồn khi revote
            //DeleteSubmitOne deleteSubmitOne = new DeleteSubmitOne()
            //{
            //    QuestionId = storeRequest.QuestionID,
            //    UserId = storeRequest.UserID
            //};
            //var callDeleteSubmitOnePerson = IQuestionAnswersService.DeleteSubmitOnePerson(deleteSubmitOne);
            //if (callDeleteSubmitOnePerson.result is null)
            //{
            //    return new ValidationResult(callDeleteSubmitOnePerson.message);
            //}
            #endregion

            return ValidationResult.Success;
        }
    }
}
