using Microsoft.EntityFrameworkCore;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.Events.Entities;
using Project.Modules.Sessions.Entities;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Users.Services
{
    public interface IQuestionService
    {    
        QuestionClient GetQuestion(string questionUserId);
        (ResponseTable response, string message) ShowQuestionByUser(string userId, RequestTable request);
        (ResponseTable response, string message, List<QuestionClientSocket>) ShowQuestionOnEvent(string eventId, RequestTable request, bool showAll=true);
        List<QuestionClient> ShowAll();
        Event DetailEvent(string eventId);
        (QuestionClient question, string message) StoreQuestion(AddQuestionClient request, string userId);
        (QuestionClient question, string message) EditQuestion(QuestionClient question);
        (QuestionClient question, string message) ShowOrHideQuestion(QuestionClient question, bool show = true);
        (QuestionClient question, string message) RemoveQuestion(QuestionClient questionClient);
        (QuestionCommentClient questionComment, string message) CommentQuestion(QuestionCommentClient questionComment, QuestionClient questionClient);
        IEnumerable<QuestionCommentClient> ShowCommentQuestion(string questionId);
    }
    public class QuestionService : IQuestionService
    {
        private readonly MariaDBContext mariaDBContext;
     
        public QuestionService(MariaDBContext mariaDBContext)
        {
            this.mariaDBContext = mariaDBContext;
        }

        public Event DetailEvent(string eventId)
        {
            return mariaDBContext.Events.FirstOrDefault(x => x.EventId.Equals(eventId));
        }

        public QuestionClient GetQuestion(string questionUserId)
        {
            return mariaDBContext.QuestionClients
                .Include(x => x.User)
                .Include(x => x.QuestionCommentClient)
                .FirstOrDefault(m => m.QuestionId.Equals(questionUserId) && m.QuestionDeleted == null);
        }

        public (QuestionClient question, string message) RemoveQuestion(QuestionClient questionClient)
        {
            questionClient.QuestionDeleted = DateTime.Now;
            mariaDBContext.QuestionClients.Update(questionClient);
            mariaDBContext.SaveChanges();
            return (questionClient, "Xóa câu hỏi thành công");
        }

        public List<QuestionClient> ShowAll()
        {
            List<QuestionClient> questions = mariaDBContext.QuestionClients.Where(m => m.QuestionDeleted == null).ToList();
            return questions;
        }

        public (ResponseTable response, string message, List<QuestionClientSocket>) ShowQuestionOnEvent(string eventId, RequestTable request, bool showProjector=true)
        {                
            List<QuestionClient> questionClients = mariaDBContext.QuestionClients
                .Include(x=>x.User)
                .Include(x => x.QuestionCommentClient)
                .Where(m => m.EventId == eventId && m.QuestionContent.Contains(request.Search) && m.QuestionDeleted == null)
                .OrderByDescending(m => m.QuestionCreatedAt)
                .ToList();
            List<QuestionClientSocket> questionClientSockets = new List<QuestionClientSocket>();
            if (showProjector)
            {
                questionClients = questionClients.Where(x => x.QuestionActive == true).ToList();
                questionClientSockets = questionClients.Select(x => new QuestionClientSocket
                {
                    questionContent = x.QuestionContent,
                    comment = x.QuestionCommentClient.FirstOrDefault()?.QuestionContent,
                    questionCreatedAt = x.QuestionCreatedAt,
                    questionId = x.QuestionId,
                    userComment = x.QuestionCommentClient.FirstOrDefault()?.UserName,
                    userName = x.User?.FullName
                }).ToList();
            }

            ResponseTable response = new ResponseTable
            {
                DateResult = request.Page == 0 ? 
                            questionClients : 
                            questionClients.Skip((request.Page - 1) * request.Results).Take(request.Results),
                Total = request.Page == 0 ? questionClients.Count() : questionClients.Count(),
                Info = new Info
                {
                    Page = request.Page,
                    Results = request.Results,
                    TotalRecord = questionClients.Skip((request.Page - 1) * request.Results).Take(request.Results).Count()
                }
            };

            return (response, "Danh sách thành công", questionClientSockets);
        }

        public (ResponseTable response, string message) ShowQuestionByUser(string userId, RequestTable request)
        {
            var user = mariaDBContext.Users.Where(m => m.UserId.Equals(userId))
                                            .FirstOrDefault();

            if (user is null)
                return (null, "Người dùng không tồn tại");
            
            List<QuestionClient> questionClients = mariaDBContext.QuestionClients
                .Include(x => x.User)
                .Include(x => x.QuestionCommentClient)
                .Where(m => m.UserId.Equals(userId) && m.QuestionContent.Contains(request.Search) && m.QuestionDeleted == null)
                .OrderByDescending(m => m.QuestionCreatedAt)
                .ToList();

            ResponseTable response = new ResponseTable
            {
                DateResult = request.Page == 0 ?
                            questionClients :
                            questionClients.Skip((request.Page - 1) * request.Results).Take(request.Results),
                Total = request.Page == 0 ? questionClients.Count() : questionClients.Count(),
                Info = new Info
                {
                    Page = request.Page,
                    Results = request.Results,
                    TotalRecord = questionClients.Skip((request.Page - 1) * request.Results).Take(request.Results).Count()
                }
            };

            return (response, "Hiển thị danh sách thành công");
        }

        public (QuestionClient question, string message) StoreQuestion(AddQuestionClient request, string userId)
        {
            Session session = mariaDBContext.Sessions.FirstOrDefault(x => x.EventId.Equals(request.EventId));
            QuestionClient question = new QuestionClient
            {
                QuestionContent = request.Content,
                EventId = request.EventId,
                UserId = userId,
                SessionId = session?.SessionId
            };

            mariaDBContext.QuestionClients.Add(question);
            mariaDBContext.SaveChanges();
            return (question, "Thêm đáp thành công");
        }

        public (QuestionClient question, string message) EditQuestion(QuestionClient question)
        {
            mariaDBContext.QuestionClients.Update(question);
            mariaDBContext.SaveChanges();

            return (question, "Đổi trạng thái câu hỏi thành công");
        }

        public (QuestionClient question, string message) ShowOrHideQuestion(QuestionClient question, bool show = true)
        {
            question.QuestionActive = show;

            mariaDBContext.QuestionClients.Update(question);
            mariaDBContext.SaveChanges();

            return (question, "Ẩn/ hiện câu hỏi thành công");
        }

        public (QuestionCommentClient questionComment, string message) CommentQuestion(QuestionCommentClient questionComment, QuestionClient question)
        {
            User user = mariaDBContext.Users.FirstOrDefault(x => x.UserId.Equals(questionComment.UserId));
            if(user != null)
            {
                questionComment.UserName = user.FullName;
            }
            else
            {
                UserSuper userSuper = mariaDBContext.UserSupers.FirstOrDefault(x => x.UserSuperId.Equals(questionComment.UserId));
                questionComment.UserName = userSuper?.FullName;
            }

            question.QuestionStatus = QUESTION_STATUS.ANSWERED;
            mariaDBContext.QuestionClients.Update(question);
            mariaDBContext.SaveChanges();

            mariaDBContext.QuestionCommentClients.RemoveRange(
                mariaDBContext.QuestionCommentClients.Where(x => x.QuestionCommentId.Equals(questionComment.QuestionClientId))
            );
            mariaDBContext.SaveChanges();
            mariaDBContext.QuestionCommentClients.Add(questionComment);
            mariaDBContext.SaveChanges();

            return (questionComment, "Thêm trả lời thành công");
        }

        public IEnumerable<QuestionCommentClient> ShowCommentQuestion(string questionId)
        {
            return mariaDBContext.QuestionCommentClients.Where(x => x.QuestionClientId.Equals(questionId)).OrderByDescending(x => x.CreatedAt);
        }
    }
}
