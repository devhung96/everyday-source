using Microsoft.Extensions.Configuration;
using Project.App.Database;
using Project.Modules.Authorities.Entities;
using Project.Modules.Documents.Entities;
using Project.Modules.Events.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Invitations.Entities;
using Project.Modules.Medias.Entities;
using Project.Modules.Organizes.Entities;
using Project.Modules.PermissionGroups.Entities;
using Project.Modules.PermissonUsers;
using Project.Modules.Question.Entities;
using Project.Modules.Sessions.Entities;
using Project.Modules.UserPermissionEvents.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Services
{
    public interface IOrganizeService
    {
        public (Organize Organize, string message , string code) Create(Organize newOrganize);
        public (Organize Organize, string message, string code) Update(string IdOrganize, Organize newOrganize);
        public (bool result, string message, string code) Delete(string IdOrganize);
        public (List<Organize> Organizes, string message, string code) ShowAll(string url = "");
        public (Organize Organize, string message, string code) Show(string IdOrganize, string url = "");

    }
    public class OrganizeService: IOrganizeService
    {
        private readonly IConfiguration _configuration;
        private readonly MariaDBContext _mariaDBContext;
        private readonly string _urlMedia = "";

        public OrganizeService(MariaDBContext mariaDBContext, IConfiguration configuration)
        {
            _mariaDBContext = mariaDBContext;
            _configuration = configuration;
            _urlMedia = _configuration["MediaService:MediaUrl"];
        }

        public (Organize Organize , string message, string code) Create(Organize newOrganize)
        {
            (Organize checkOrganize, string checkMessage, string checkCode) = this.Show(newOrganize.OrganizeId);
            if (checkOrganize != null) return (checkOrganize, checkMessage, checkCode);

            // check user 
            if (!String.IsNullOrEmpty(newOrganize.UserAdmin))
            {
                var checkUser = _mariaDBContext.UserSupers
                                               .FirstOrDefault(x => 
                                                                  x.UserSuperId == newOrganize.UserAdmin &&
                                                                  x.DeleteStatus != Users.Entities.User.DELETE_STATUS.DELETED && 
                                                                  !String.IsNullOrEmpty(x.OrganizeId)
                                                                  );
                if (checkUser != null) return (null, "Tài khoản quản trị đã tồn tại trong tổ chức khác rồi!", "TheAdminAccountAlreadyExistsInAnotherOrganization");
            }
            // update admin to chuc user 
            var userOrganize = _mariaDBContext.UserSupers.FirstOrDefault(x => 
                                                                            x.UserSuperId == newOrganize.UserAdmin &&
                                                                            x.DeleteStatus != Users.Entities.User.DELETE_STATUS.DELETED
                                                                            );
            if(userOrganize != null)
            {
                userOrganize.OrganizeId = newOrganize.OrganizeId;
                userOrganize.Level = UserSuper.USER_TYPE.ADMIN;
            }
            _mariaDBContext.Organizes.Add(newOrganize);
            _mariaDBContext.SaveChanges();
            newOrganize.OrganizeLogoUrl = newOrganize.OrganizeLogoUrl.UrlCombine(_urlMedia);
            return (newOrganize, "Tạo tổ chức thành công.","CreatedOrganizeSuccess");
        }


        public (Organize Organize, string message, string code) Update(string IdOrganize, Organize newOrganize)
        {
            
            (Organize checkOrganize, string checkMessage, string checkCode) = this.Show(IdOrganize);
            if (checkOrganize is null) return (checkOrganize, checkMessage, checkCode);
            newOrganize.OrganizeId = IdOrganize;

            if (!String.IsNullOrEmpty(newOrganize.UserAdmin))
            {
                var checkUser = _mariaDBContext.UserSupers.FirstOrDefault(x => x.UserSuperId == newOrganize.UserAdmin && x.DeleteStatus != Users.Entities.User.DELETE_STATUS.DELETED  && !String.IsNullOrEmpty(x.OrganizeId) && x.OrganizeId != IdOrganize);
                if (checkUser != null) return (null, "Tài khoản quản trị đã tồn tại trong tổ chức khác rồi!", "TheAdminAccountAlreadyExistsInAnotherOrganization");
            }

            var checkOrganizeCK = _mariaDBContext.Organizes.FirstOrDefault(x => x.OrganizeCodeCk == newOrganize.OrganizeCodeCk && x.OrganizeId != IdOrganize);
            if (checkOrganizeCK != null) return (null, "Mã chứng khoán đã tồn tại vui lòng nhập mã khác!", "StockCodeAlreadyExists,PleaseEnterAnotherCode");

            if(newOrganize.UserAdmin != checkOrganize.UserAdmin && !String.IsNullOrEmpty(newOrganize.UserAdmin))
            {
                var oldUserAdmin = _mariaDBContext.UserSupers.FirstOrDefault(x => x.UserSuperId == checkOrganize.UserAdmin && x.DeleteStatus != Users.Entities.User.DELETE_STATUS.DELETED);
                if (oldUserAdmin != null) oldUserAdmin.OrganizeId = null;
                var newUserAdmin = _mariaDBContext.UserSupers.FirstOrDefault(x => x.UserSuperId == newOrganize.UserAdmin && x.DeleteStatus != Users.Entities.User.DELETE_STATUS.DELETED);
                if (newUserAdmin != null)
                {
                    newUserAdmin.OrganizeId = newOrganize.OrganizeId;
                    newUserAdmin.Level = UserSuper.USER_TYPE.ADMIN;
                }
            }

            // check so co dong
            List<string> eventIds = _mariaDBContext.Events.Where(x => x.OrganizeId == newOrganize.OrganizeId).Select(x => x.EventId).ToList();
            List<EventUser> allEventUsers = _mariaDBContext.EventUsers.ToList();
            var eventUserByOrganize = allEventUsers.Where(x => eventIds.Contains(x.EventId)).GroupBy(x => x.EventId).ToList();

            foreach (var item in eventUserByOrganize)
            {
                var totalStockForEvent = item.Select(x => x.UserStock).Sum(x => x);
                if (newOrganize.OrganizeStocks < totalStockForEvent) return (null, "Số lượng cổ phần của tổ chức phải lớn hơn hoặc bằng tổng cổ phần của người tham gia", "TheNumberOfSharesOfAnOrganizationMustBeGreaterThanOrEqualToTheTotalStakeOfTheParticipant");
            }


            var _newOrganize = GeneralHelper.CheckUpdateObject<Organize>(checkOrganize, newOrganize);
            _newOrganize.UpdatedAt = DateTime.UtcNow.AddHours(7);
            _mariaDBContext.Entry(checkOrganize).CurrentValues.SetValues(_newOrganize);
            _mariaDBContext.SaveChanges();
            _newOrganize.OrganizeLogoUrl = _newOrganize.OrganizeLogoUrl.UrlCombine(_urlMedia);
            return (_newOrganize, "Cập nhật tổ chức thành công!", "UpdatedOrganizeSuccess");
        }

        public (bool result, string message, string code) Delete(string IdOrganize)
        {
            (Organize checkOrganize, string checkMessage, string checkCode ) = this.Show(IdOrganize);
            if (checkOrganize is null) return (false, checkMessage, checkCode);

            int countEvent = _mariaDBContext.Events.Count(x => x.OrganizeId == IdOrganize && x.EventFlag == EVENT_FLAG.BEGIN);
            if (countEvent != 0) return (false, "Tổ chức vẫn còn event đang diễn ra không thể xóa", "TheOrganizationStillHasOngoingEventsThatCannotBeDeleted");


            using (var transaction = _mariaDBContext.Database.BeginTransaction())
            {
                try
                {
                    List<Event> events = _mariaDBContext.Events.Where(x => x.OrganizeId == IdOrganize).ToList();

                    UserSuper userSuper = _mariaDBContext.UserSupers.FirstOrDefault(x => x.OrganizeId == IdOrganize);
                    if (userSuper != null) _mariaDBContext.UserSupers.Remove(userSuper);

                    foreach (var item in events)
                    {
                        // shareholder_authority
                        List<Authority> authorites = _mariaDBContext.Authorities.Where(x => x.EventID == item.EventId).ToList();
                        if (authorites != null)
                        {
                            _mariaDBContext.Authorities.RemoveRange(authorites);
                        }


                        //template
                        List<Template> templates = _mariaDBContext.Templates.Where(x => x.EventId == item.EventId).ToList();
                        _mariaDBContext.Templates.RemoveRange(templates);

                        // thu moi
                        List<Invitation> invitations = _mariaDBContext.Invitations.Where(x => x.EventId == item.EventId).ToList();
                        _mariaDBContext.Invitations.RemoveRange(invitations);


                        //
                        List<QuestionClient> questionClients = _mariaDBContext.QuestionClients.Where(x => x.EventId == item.EventId).ToList();
                        // command 
                        List<QuestionCommentClient> questionCommentClients = _mariaDBContext.QuestionCommentClients.Where(x => questionClients.Select(x => x.QuestionId).Contains(x.QuestionClientId)).ToList();
                        _mariaDBContext.QuestionCommentClients.RemoveRange(questionCommentClients);
                        _mariaDBContext.QuestionClients.RemoveRange(questionClients);
                        _mariaDBContext.SaveChanges();



                        // permisson group
                        List<UserPermissionEvent> userPermissionEvents = _mariaDBContext.UserPermissionEvents.Where(m => m.EventId.Equals(item.EventId)).ToList();
                        _mariaDBContext.RemoveRange(userPermissionEvents);

                        // group
                        List<Group> groups = _mariaDBContext.Groups.Where(m => m.EventId.Equals(item.EventId)).ToList();
                        List<int> groupIds = groups.Select(x => x.GroupID).ToList();
                        List<PermissionGroup> permissionGroups = _mariaDBContext.PermissionGroups.Where(m => groupIds.Equals(m.GroupId)).ToList();

                        _mariaDBContext.PermissionGroups.RemoveRange(permissionGroups);
                        _mariaDBContext.Groups.RemoveRange(groups);

                        //user in event
                        List<EventUser> eventUsers = _mariaDBContext.EventUsers.Where(m => m.EventId.Equals(item.EventId)).ToList();
                        _mariaDBContext.EventUsers.RemoveRange(eventUsers);


                        //DocumentFile
                        List<DocumentFile> documentFiles = _mariaDBContext.DocumentFiles.Where(x => x.EventId == item.EventId).ToList();
                        _mariaDBContext.RemoveRange(documentFiles);


                        // secction 
                        List<Session> sessions = _mariaDBContext.Sessions.Where(x => x.EventId == item.EventId).ToList();
                        _mariaDBContext.Sessions.RemoveRange(sessions);


                        List<MiddleQuestion> middleQuestions = _mariaDBContext.MiddleQuestions.Where(x => sessions.Select(y => y.SessionId).Contains(x.SessionID)).ToList();
                        _mariaDBContext.MiddleQuestions.RemoveRange(middleQuestions);

                        _mariaDBContext.SaveChanges();

                    }

                    _mariaDBContext.Events.RemoveRange(events);
                    _mariaDBContext.SaveChanges();


                    List<User> usersDelete = _mariaDBContext.Users.Where(m => m.OrganizeId == IdOrganize).ToList();
                    List<PermissionUser> permissionUsers = _mariaDBContext.PermissionUsers.Where(m => usersDelete.Select(x => x.UserId).Contains(m.UserId)).ToList();
                    _mariaDBContext.PermissionUsers.RemoveRange(permissionUsers);
                    _mariaDBContext.SaveChanges();
                    _mariaDBContext.Users.RemoveRange(usersDelete);
                    _mariaDBContext.SaveChanges();


                    List<Media> medias = _mariaDBContext.Medias.Where(x => x.OrganizeId == IdOrganize).ToList();
                    _mariaDBContext.Medias.RemoveRange(medias);

                    _mariaDBContext.Remove(checkOrganize);
                    _mariaDBContext.SaveChanges();
                    transaction.Commit();
                    return (true, "Xóa tổ chức thành công!", "DeletedOrganizeSuccess");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                    return (false, "Lỗi bất ngờ", "Erro");
                }
            }

            
           
        }


        public (Organize Organize , string message, string code) Show(string IdOrganize, string url = "")
        {
            Organize organize = _mariaDBContext.Organizes.FirstOrDefault(x => x.OrganizeId == IdOrganize);
            if (organize is null) return (null,"Không tìm thấy tổ chức!", "OrganizeNotFound");

            var userAdmin = _mariaDBContext.UserSupers.FirstOrDefault(x => x.OrganizeId == organize.OrganizeId);

            organize.UserAdminInfo = userAdmin;
            organize.UserAdmin = userAdmin == null ? "" : userAdmin.UserSuperId;
            if (!String.IsNullOrEmpty(url))
            {
                organize.OrganizeLogoUrl = organize.OrganizeLogoUrl.UrlCombine(_urlMedia);
            }
            return (organize, "Lấy thông tin tổ chức thành công!", "GetOrganizeSuccess");
        }


        public (List<Organize> Organizes, string message , string code) ShowAll(string url = "")
        {
            List<Organize> data = _mariaDBContext.Organizes.ToList();
            if (!String.IsNullOrEmpty(url))
            {
                foreach (var item in data)
                {
                    item.OrganizeLogoUrl = item.OrganizeLogoUrl.UrlCombine(_urlMedia);
                    var userAdmin = _mariaDBContext.UserSupers.FirstOrDefault(x => x.OrganizeId == item.OrganizeId);
                    item.UserAdminInfo = userAdmin;
                    item.UserAdmin = userAdmin == null ? "" : userAdmin.UserSuperId;
                }
            }
            return (data, "Hiển thị thành công", "ShowAllSuccess");
        }



    }
}
