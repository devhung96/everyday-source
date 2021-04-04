using Microsoft.Extensions.Configuration;
using Project.App.Database;
using Project.Modules.Question.Entities;
using Project.Modules.Sessions.Entities;
using Project.Modules.Sessions.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Sessions.Services
{
    public interface ISessionService
    {
        public (Session Session, string message, string code) Create(Session newSession);
        public (Session Session, string message, string code) Update(string IdSession, Session newSession);
        public (bool result, string message, string code) Delete(string IdSession);
        public (Session Session, string message, string code) Show(string IdSession, string url ="");
        public (List<Session> Sessions, string message, string code) ShowAll(string url = "");
        public (List<Session> Sessions, string message, string code) ShowByEvent(string idEvent, string url = "");
        public (bool result, string message, string code) SessionSort(string idSessionFirst, string idSessionSecond);

        public (bool result, string message, string code) MutipleSessionSort(UpdateMutipleSessionSortRequest request);
    }
    public class SessionService: ISessionService
    {
        private readonly IConfiguration _configuration;
        private readonly MariaDBContext _mariaDBContext;

        public SessionService(MariaDBContext mariaDBContext, IConfiguration configuration)
        {
            _mariaDBContext = mariaDBContext;
            _configuration = configuration;
        }

        public (Session Session, string message, string code) Create(Session newSession)
        {
            // check event end

            _mariaDBContext.Sessions.Add(newSession);
            _mariaDBContext.SaveChanges();
            return (newSession, "Tạo chương trình thành công!", "CreatedSessionSuccess");
        }


        public (Session Session, string message, string code) Update(string IdSession, Session newSession)
        {
            // check event end

            (Session checkSession, string checkMessage, string checkCode) = this.Show(IdSession);
            if (checkSession is null) return (checkSession, checkMessage, checkCode);

            newSession.SessionId = IdSession;

            var _newSession = GeneralHelper.CheckUpdateObject<Session>(checkSession, newSession);
            _newSession.UpdatedAt = DateTime.UtcNow.AddHours(7);
            _mariaDBContext.Entry(checkSession).CurrentValues.SetValues(_newSession);
            _mariaDBContext.SaveChanges();
            return (_newSession, "Cập nhật chương trình thành công","UpdatedSessionSuccess");
        }

        public (bool result, string message, string code) Delete(string IdSession)
        {
            (Session checkSession, string checkMessage, string checkCode) = this.Show(IdSession);
            if (checkSession is null) return (false, checkMessage,checkCode);
            checkSession.SessionStatus = SESSION_STATUS.DELETED;


            // xoa phan question session 
            List<MiddleQuestion> middleQuestions = _mariaDBContext.MiddleQuestions.Where(x => x.SessionID == checkSession.SessionId).ToList();
            middleQuestions.ForEach(x => x.Type = TypeMiddle.DELETE);
            _mariaDBContext.SaveChanges();
            //xoa question 

            _mariaDBContext.SaveChanges();
            return (true, "Xóa chương trình thành công", "DeletedSessionSuccess");
        }


        public (Session Session, string message , string code) Show(string IdSession, string url = "")
        {
            Session Session = _mariaDBContext.Sessions.FirstOrDefault(x => x.SessionId == IdSession);
            if (Session is null) return (null, "Không tìm thấy chương trình", "SessionNotFound");

            return (Session,"Hiện thị chương trình thành công", "GetSessionSuccess");
        }


        public (List<Session> Sessions, string message, string code) ShowAll(string url = "")
        {
            List<Session> data = _mariaDBContext.Sessions.ToList();
            return (data, "Hiện thị chương trình thành công" , "ShowAllSuccess");
        }

        public (List<Session> Sessions, string message, string code) ShowByEvent(string idEvent,string url = "")
        {
            List<Session> data = _mariaDBContext.Sessions.Where(x=> x.EventId == idEvent).ToList();
            return (data, "Hiển thị thành công! ", "ShowAllSuccess");
        }

        public (bool result , string message , string code ) SessionSort(string idSessionFirst,string idSessionSecond)
        {
            Session sessionFirst = _mariaDBContext.Sessions.FirstOrDefault(x => x.SessionId == idSessionFirst);
            Session sessionSecond = _mariaDBContext.Sessions.FirstOrDefault(x => x.SessionId == idSessionSecond);
            if (sessionFirst is null || sessionSecond is null) return (false, "Không tìm thấy chương trình", "SessionNotFound");

            int sessionSortFirst = sessionFirst.SessionSort;
            sessionFirst.SessionSort = sessionSecond.SessionSort;
            sessionSecond.SessionSort = sessionSortFirst;
            _mariaDBContext.SaveChanges();
            return (true, "Cập nhật chương trình thành công", "UpdatedSessionSuccess");
        }


        public (bool result, string message, string code) MutipleSessionSort(UpdateMutipleSessionSortRequest request)
        {
            using (var transaction = _mariaDBContext.Database.BeginTransaction())
            {
                foreach (var item in request.Sessions)
                {

                    Session sessionTmp = _mariaDBContext.Sessions.FirstOrDefault(x => x.SessionId == item.SessionId);
                    if (sessionTmp is null)
                    {
                        transaction.Rollback();
                        return (false, "Không tìm thấy chương trình", "SessionNotFound");
                    }
                    sessionTmp.SessionSort = item.SessionSort;
                }
                _mariaDBContext.SaveChanges();
                transaction.Commit();
                return (true, "Cập nhật chương trình thành công", "UpdatedSessionSuccess");
            }

           
        }



    }
}
