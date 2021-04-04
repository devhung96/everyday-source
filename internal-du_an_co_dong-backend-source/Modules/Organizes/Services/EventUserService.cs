using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Project.App.Database;
using Project.App.Providers;
using Project.App.Requests;
using Project.Modules.Authorities.Entities;
using Project.Modules.Authorities.Services;
using Project.Modules.Events.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Invitations.Entities;
using Project.Modules.Organizes.Entities;
using Project.Modules.Organizes.Requests;
using Project.Modules.PermissionGroups.Entities;
using Project.Modules.Permissions.Validations;
using Project.Modules.PermissonUsers;
using Project.Modules.UserPermissionEvents.Entities;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Services
{
    public interface IEventUserService
    {
        (int responseStatus, string message) DeleteAllUserEvent(string eventId);
        (EventUser result, string message) AddUserToEvent(AddUserToEventRequest request);
        (EventUser result, string message) UpdateUserToEvent(UpdateUserToEventRequest request, int eventUserId);
        (EventUser result, string message) DeleteUserInEvent(int eventUserId);
        (ResponseTable response, string message) ShowUsersByEvent(string eventId, RequestTable request, string orgnazieID);
        public (EventUser result, string message) BlockUserInEvent(int eventUserId, int status);
        (User result, string message) FilterUser(FilterShareholderCode filter);
        (List<User> users, string message) UserNoneStocks(string eventId);
        List<User> GetListUserFromOrganize(string orgnaizeID, string eventID);
        (ResponseTable response, string message) ShowUsersStartEvent(string eventId, RequestTable request);
        (ResponseTable response, string message) ShowUsersLoginEvent(string eventId, RequestTable request);
        (bool status, string message, string code) LogOutLanding(string eventId, string userId);
        (List<ItemImportUserToEvent> result, string message) ImportUserInEvent(ImportRequest request);
    
    }
    public class EventUserService : IEventUserService
    {
        private readonly MariaDBContext mariaDBContext;
        private readonly IAuthorityService authorityService;
        private readonly IUserService userService;
        public EventUserService(MariaDBContext mariaDBContext, IAuthorityService authorityService, IUserService userService)
        {
            this.mariaDBContext = mariaDBContext;
            this.authorityService = authorityService;
            this.userService = userService;
        }
        void SendMailInOrganize(User user, Organize organize)
        {
           if(user.SendMail == User.SEND_STATUS.CHUA)
            {
                // lần đầu gửi thư sẽ gửi mật khẩu
                TransportPatternProvider.Instance.Emit("SendEmail", new SendMailRequest
                {

                    MessageSubject = "Tạo mới tài khoản trang Đại Hội Cổ Đông",
                    MessageContent = "Email của bạn đã được tạo tài khoản đăng nhập vào Tổ chức: " + organize.OrganizeName + ". Với thông tin đăng nhập như sau:"
                                               + "<br/>Mã cổ đông: <b>" + user.ShareholderCode + "</b>"
                                               + "<br/>Mã tổ chức: <b>" + user.StockCode + "</b>"
                                               + "<br/>Mật khẩu: <b>" + user.PasswordSystem + "</b>"
                                               + "<br/>Để đảm bảo tính bảo mật, vui lòng đăng nhập hệ thống và thay đổi mật khẩu."
                                               + "<br/>Trân trọng!",
                    Contacts = new List<SendMailContact>
                {
                    new SendMailContact{ContactEmail = user.UserEmail}
                }
                });
            }

            else
            {
                // Đã gửi thư rồi không gửi mật khẩu
                TransportPatternProvider.Instance.Emit("SendEmail", new SendMailRequest
                {

                    MessageSubject = "Tạo mới tài khoản trang Đại Hội Cổ Đông",
                    MessageContent = "Email của bạn đã được tạo tài khoản đăng nhập vào Tổ chức: " + organize.OrganizeName + ". Với thông tin đăng nhập như sau:"
                                               + "<br/>Mã cổ đông: <b>" + user.ShareholderCode + "</b>"
                                               + "<br/>Mã tổ chức: <b>" + user.StockCode + "</b>"
                                               + "<br/>Trân trọng!",
                    Contacts = new List<SendMailContact>
                {
                    new SendMailContact{ContactEmail = user.UserEmail}
                }
                });
            }
            user.SendMail = User.SEND_STATUS.DA;
            mariaDBContext.Users.Update(user);
            
        }
        public (EventUser result, string message) AddUserToEvent(AddUserToEventRequest request)
        {
            #region Nếu không nhập password tự động random 6 kí tự
            if (String.IsNullOrEmpty(request.UserPassword))
            {
                request.UserPassword = 6.RandomString();
            }
            #endregion

            Event eventOrganize = mariaDBContext.Events
                                                .Where(m => m.EventId.Equals(request.EventId))
                                                .Include(m => m.Organize)
                                                .FirstOrDefault();
          
            int perGroup = mariaDBContext.PermissionGroups.Count(m => m.GroupId == request.GroupId);

            string saft = 5.RandomString();
            User user = mariaDBContext.Users
                                        .Where(m => m.ShareholderCode.Equals(request.ShareholderCode) && (m.OrganizeId.Equals(eventOrganize.OrganizeId)))
                                        .FirstOrDefault();

            using (IDbContextTransaction transaction = mariaDBContext.Database.BeginTransaction())
            {
                try
                {

                    if (eventOrganize.Organize is null)
                    {
                        return (null, "OrganizeNotExist");
                    }

                  
                   
                    int totalStock = mariaDBContext.EventUsers
                                                        .Where(m => m.EventId.Equals(request.EventId))
                                                        .Sum(m => m.UserStock);

                    #region Nếu chưa có User thêm mới    
                    if (user is null)
                    {
                        user = new User
                        {
                            OrganizeId = eventOrganize.OrganizeId,
                            ShareholderCode = request.ShareholderCode,
                            UserEmail = request.UserEmail,
                            UserImage = request.UserImage,
                            IdentityCard = request.IdentityCard,
                            PlaceOfIssue = request.PlaceOfIssue,
                            IssueDate = request.IssueDate,
                            StockCode = eventOrganize.Organize.OrganizeCodeCk,
                            PhoneNumber = request.PhoneNumber,
                            FullName = request.FullName,
                        };
                        mariaDBContext.Users.Add(user);
                        mariaDBContext.SaveChanges();

                        PermissionUser permissonUser = new PermissionUser
                        {
                            UserId = user.UserId,
                            PermissionCode = "CLIENT",
                        };
                        mariaDBContext.PermissionUsers.Add(permissonUser);
                        if (user.SendMail == User.SEND_STATUS.CHUA && perGroup > 0)
                        {

                            user.PasswordSystem = request.UserPassword;
                            user.UserSalt = saft;
                            user.UserPassword = (saft + request.UserPassword).HashPassword();

                        }
                        mariaDBContext.SaveChanges();
                    }
                    else
                    {
                        if ( perGroup>0)
                        {
                            if(user.SendMail == User.SEND_STATUS.CHUA)
                            {
                                user.PasswordSystem = request.UserPassword;
                                user.UserSalt = saft;
                                user.UserPassword = (saft + request.UserPassword).HashPassword();

                            }

                        //    SendMailInOrganize(user,eventOrganize.Organize);
                            mariaDBContext.SaveChanges();
                        }
                    }
                    #endregion

                    EventUser eventUser = mariaDBContext.EventUsers
                                                        .Where(m => m.EventId.Equals(eventOrganize.EventId) && m.UserId.Equals(user.UserId))
                                                        .FirstOrDefault();

                    #region Thêm user vào event 
                
                    if (eventUser is null)
                    {
                        if (totalStock + request.UserStock <= eventOrganize.Organize.OrganizeStocks)
                        {
                            eventUser = new EventUser
                            {
                                EventId = eventOrganize.EventId,
                                UserId = user.UserId,
                                UserStock = request.UserStock.Value,
                                GroupId = request.GroupId,
                                UserPassword = (saft + request.UserPassword).HashPassword(),
                                UserSalt = saft,
                                PasswordSystem = request.UserPassword,
                            };
                            mariaDBContext.EventUsers.Add(eventUser);
                         

                            #region lấy tất cả các quyền trong group update vào bảng mới UserPermissionEvent
                            var check = mariaDBContext.PermissionGroups.Where(m => m.GroupId == request.GroupId).ToList();

                            // Huy thêm
                            if(check.Count() > 0)
                            {
                                AddUserPermissionByGroup(request.GroupId, user.UserId, eventUser.EventId);
                                SendMailInOrganize(user, eventOrganize.Organize);
                                eventUser.SendMailCMS = User.SEND_STATUS.DA;
                            }
                            // Hết code huy thêm

                         

                            #endregion
                            mariaDBContext.SaveChanges();
                        }
                        else
                        {
                            return (null, "Số cổ phần của tất cả người dùng không vượt quá tổng cổ phần của tổ chức.");
                        }

                    }
                    else
                    {
                        return (null, "Người dùng đã tồn tại trong sự kiện.");
                    }
                    #endregion



                    transaction.Commit();
                    return (eventUser, "AddUserEventSuccess");


                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return (null, "AddUserEventFaild");
                }


            }
        }
        public (EventUser result, string message) UpdateUserToEvent(UpdateUserToEventRequest request, int eventUserId)
        {
            EventUser eventUser = mariaDBContext.EventUsers.Where(m=>m.EventUserId.Equals(eventUserId)).Include(m=>m.User).FirstOrDefault();
            if(eventUser.UserLatch == USER_LATCH.ON)
            {
                return (null, "StreamingEventNotUpdate");
            }
            if (eventUser is null)
            {
                return (null, "UserEventNotExist");
            }

            #region Kiểm tra số cổ phiếu cập nhât 
            int totalStockInEvent = mariaDBContext.EventUsers
                                               .Where(m => m.EventId.Equals(eventUser.EventId) && !(m.UserId.Equals(eventUser.UserId)))
                                               .Sum(m => m.UserStock);

            Event oganizeEvent = mariaDBContext.Events.Where(m => m.EventId.Equals(eventUser.EventId))
                                        .Include(m => m.Organize)
                                        .FirstOrDefault();
            if (totalStockInEvent + request.UserStock > oganizeEvent.Organize.OrganizeStocks)
            {
                return (null, "TotalSumStockEventUserIsLargerThanStockOrganize");
            }
            #endregion

            #region cập nhập thông tin trong Event User
            if (request.GroupId.Value != eventUser.GroupId)
            {
                #region Xóa quyền của user trong Event theo Group cũ
            
                List<int> permissionGroupOld = mariaDBContext.PermissionGroups.Where(m => m.GroupId == eventUser.GroupId)
                                                                              .Select(m=>m.PermissionId)
                                                                              .ToList();

                List<UserPermissionEvent> permissionEventOld = mariaDBContext.UserPermissionEvents.Where(m => 
                                                                                                    m.UserId.Equals(eventUser.UserId) && 
                                                                                                    m.EventId.Equals(eventUser.EventId) && 
                                                                                                    permissionGroupOld.Contains(m.PermissionId)
                                                                                                    ).ToList();

                mariaDBContext.UserPermissionEvents.RemoveRange(permissionEventOld);

                #endregion

                #region Thêm quyền của Group mới vào UserPermissionEvent
                List<PermissionGroup> permissionGroupNews = mariaDBContext.PermissionGroups.Where(m => m.GroupId == request.GroupId).ToList();
               
                // gửi mail thông báo có tài khoản trong tổ chức
                if (permissionGroupNews.Count()>0)
                {
                    if( eventUser.User.SendMail == User.SEND_STATUS.CHUA)
                    {
                        var userSend = eventUser.User;
                        userSend.PasswordSystem = 6.RandomString();
                        userSend.UserSalt = 5.RandomString();
                        userSend.UserPassword = (userSend.UserSalt + userSend.PasswordSystem).HashPassword();
                    }    
                    if(eventUser.SendMailCMS==User.SEND_STATUS.CHUA)
                    {
                        eventUser.SendMailCMS = User.SEND_STATUS.DA;
                        SendMailInOrganize(eventUser.User, oganizeEvent.Organize);
                    }    
                   
                }    
                List<UserPermissionEvent> userPermissionEvents = new List<UserPermissionEvent>();
                
                foreach( var item in permissionGroupNews)
                {
                    userPermissionEvents.Add(new UserPermissionEvent {
                                                            EventId= eventUser.EventId,
                                                            UserId = eventUser.UserId,
                                                            PermissionId = item.PermissionId,
                                                            PermissionCode = item.PermissionCode
                                                            });
                }
                mariaDBContext.UserPermissionEvents.AddRange(userPermissionEvents);

                #endregion
                eventUser.GroupId = request.GroupId.Value;
                
            
            }

            _ = request.UserStock!= null ? eventUser.UserStock = request.UserStock.Value : eventUser.UserStock;
            mariaDBContext.EventUsers.Update(eventUser);
            #endregion

          


            #region cập nhật thông tin trong User

            User userAdmin = mariaDBContext.Users.Find(eventUser.UserId);
            if (userAdmin is null)
                return (null, "Cập nhật thất bại.");

            _ = !String.IsNullOrEmpty(request.UserEmail) ? userAdmin.UserEmail = request.UserEmail : userAdmin.UserEmail;
            _ = !String.IsNullOrEmpty(request.FullName) ? userAdmin.FullName = request.FullName : userAdmin.FullName;
            _ = !String.IsNullOrEmpty(request.ShareholderCode) ? userAdmin.ShareholderCode = request.ShareholderCode : userAdmin.ShareholderCode;
            _ = !String.IsNullOrEmpty(request.IdentityCard) ? userAdmin.IdentityCard = request.IdentityCard : userAdmin.IdentityCard;
            _ = !String.IsNullOrEmpty(request.PlaceOfIssue) ? userAdmin.PlaceOfIssue = request.PlaceOfIssue : userAdmin.PlaceOfIssue;
            _ = !String.IsNullOrEmpty(request.PhoneNumber) ? userAdmin.PhoneNumber = request.PhoneNumber : userAdmin.PhoneNumber;
            _ = !String.IsNullOrEmpty(request.IssueDate.ToString()) ? userAdmin.IssueDate = request.IssueDate : userAdmin.IssueDate;
            _ = !String.IsNullOrEmpty(request.UserImage) ? userAdmin.UserImage = request.UserImage : userAdmin.UserImage;

            #endregion
            mariaDBContext.Users.Update(userAdmin);
            mariaDBContext.SaveChanges();
            return (eventUser, "success");

        }
        public (EventUser result, string message) BlockUserInEvent(int eventUserId, int status)
        {

            EventUser eventUser = mariaDBContext.EventUsers.Find(eventUserId);
            if (eventUser is null)
            {
                return (null, "UserEventNotExist");
            }

            if (status == (int)EventUser.STATUS.BLOCK)
            {
                //
                // Gọi Bỏ phân quyền Authory 
                //

                eventUser.UserActive = (int)EventUser.STATUS.BLOCK;

            }
            else
            {
                _ = status == (int)EventUser.STATUS.ACTIVE ? eventUser.UserActive = EventUser.STATUS.ACTIVE : eventUser.UserActive;
            }
            mariaDBContext.EventUsers.Update(eventUser);
            mariaDBContext.SaveChanges();
            return (eventUser, "BlockUserSuccess");

        }

        public (EventUser result, string message) DeleteUserInEvent(int eventUserId)
        {
            EventUser eventUser = mariaDBContext.EventUsers.Find(eventUserId);
            if (eventUser is null)
            {
                return (null, "UserEventNotExist");
            }

            using (IDbContextTransaction transaction = mariaDBContext.Database.BeginTransaction())
            {
                try
                {

                    #region checked event va user co ton tai khong xoa

                    List<int> authoryCheck = mariaDBContext.Authorities.Where(m => m.EventID.Equals(eventUser.EventId)
                    && m.AuthorityType == AuthorityType.EVENT
                    && (m.AuthorityReceiveUserID.Equals(eventUser.UserId)
                    || m.AuthorityUserID.Equals(eventUser.UserId)))
                        .Select(m => m.AuthorityID).ToList();

                    if (authoryCheck.Count() > 0)
                        return (null, "CheckAuthorizationBeforeDelete");
                    // authorityService.DeleteFromEvent(eventUser.UserId, eventUser.EventId);
                    #endregion

                    #region Xoa thu moi
                    List<Invitation> invitations = mariaDBContext.Invitations.Where(m => m.UserId.Equals(eventUser.UserId) && m.EventId.Equals(eventUser.EventId)).ToList();
                    mariaDBContext.Invitations.RemoveRange(invitations);
                    #endregion

                    #region xoa hoi dap cua user
                    List<QuestionClient> questionClients = mariaDBContext.QuestionClients.Where(m => m.UserId.Equals(eventUser.UserId) && m.EventId.Equals(eventUser.EventId)).ToList();
                    mariaDBContext.QuestionClients.RemoveRange(questionClients);
                    #endregion

                    string organizeId = mariaDBContext.Events.Where(m => m.EventId.Equals(eventUser.EventId))
                                                             .Select(m => m.OrganizeId)
                                                             .FirstOrDefault();

                    List<string> eventIds = mariaDBContext.Events.Where(m => m.OrganizeId.Equals(organizeId))
                                                                 .Select(m => m.EventId)
                                                                 .ToList();
                    // Dem so luong event ma user dang ton tai khac voi event dang xoa
                    int userEvent = mariaDBContext.EventUsers.Where(m =>
                                                                        m.UserId.Equals(eventUser.UserId) &&
                                                                        !(m.EventId.Equals(eventUser.EventId))
                                                                   ).Count();

                    mariaDBContext.EventUsers.Remove(eventUser);

                    if (userEvent == 0)
                    {
                        User user = mariaDBContext.Users.Find(eventUser.UserId);
                        mariaDBContext.Users.Remove(user);
                        PermissionUser permissionUser = mariaDBContext.PermissionUsers
                                                                      .Where(m => m.UserId.Equals(user.UserId))
                                                                      .FirstOrDefault();
                        if (!(permissionUser is null))
                        {
                            mariaDBContext.PermissionUsers.Remove(permissionUser);
                        }


                    }
                    mariaDBContext.SaveChanges();
                    transaction.Commit();
                    return (eventUser, "DeleteUserEventSuccess");
                }
                catch
                {
                    transaction.Rollback();
                    return (null, "DeleteFaild");

                }
            }
        }

        public (ResponseTable response, string message) ShowUsersByEvent(string eventId, RequestTable request, string orgnazieID)
        {
            List<string> userAuthority = mariaDBContext.Authorities
                                                            .Where(x => x.EventID.Equals(eventId) && x.AuthorityType == AuthorityType.EVENT)
                                                            .Select(x => x.AuthorityUserID).ToList();
            Event events = mariaDBContext.Events.Find(eventId);
            if (events is null)
            {
                return (null, "EventNotExist");
            }
            string idEvent = mariaDBContext.EventUsers
                                                    .Where(m => m.EventId.Equals(eventId))
                                                    .Select(m => m.EventId)
                                                    .FirstOrDefault();

            var eventUsers = mariaDBContext.EventUsers
                                              .Where(m => m.EventId.Equals(eventId) && !userAuthority.Contains(m.UserId))
                                              .Include(m => m.User)
                                              .Include(m => m.Group)
                                              .Select(m => new
                                               EventUser
                                              {
                                                  UserId = m.UserId,
                                                  EventId = m.EventId,
                                                  GroupId = m.GroupId,
                                                  UserPassword = m.UserPassword,
                                                  UserSalt = m.UserSalt,
                                                  UserLoginStatus = m.UserLoginStatus,
                                                  EventUserId = m.EventUserId,
                                                  CreatedAt = m.CreatedAt,
                                                  User =new User 
                                                  {
                                                      UserId = m.User.UserId,
                                                      FullName = m.User.FullName,
                                                      UserEmail = m.User.UserEmail,
                                                      UserStock = m.UserStock,
                                                      UserImage = m.User.UserImage,
                                                      IdentityCard = m.User.IdentityCard,
                                                      IssueDate = m.User.IssueDate,
                                                      PlaceOfIssue = m.User.PlaceOfIssue,
                                                      PhoneNumber = m.User.PhoneNumber,
                                                      UserCreatedAt = m.User.UserCreatedAt,
                                                      ShareholderCode = m.User.ShareholderCode,

                                                  },
                                                  PasswordSystem = m.PasswordSystem,
                                                  Group = m.Group,
                                                  UserStock = m.UserStock,
                                                  SendInvitation = m.SendInvitation,
                                                  UserActive = m.UserActive,
                                                  StockReceive = mariaDBContext.Authorities.Where(x => x.EventID.Equals(m.EventId) && x.AuthorityReceiveUserID.Equals(m.UserId) && x.AuthorityType == AuthorityType.EVENT).Sum(x => x.AuthorityShare).Value
                                              })
                                              .ToList();


            eventUsers = eventUsers.Where(x => string.IsNullOrEmpty(request.SearchGroup) ||
                                            (!string.IsNullOrEmpty(request.SearchGroup) &&
                                                x.Group.GroupName.ToLower().Contains(request.SearchGroup.ToLower())
                                            ))
                                    .ToList();

            var search = request.Search.ToLower();
            eventUsers = eventUsers.Where(x => x.User != null).ToList();
            eventUsers = eventUsers.Where(x =>
                                           string.IsNullOrEmpty(request.Search) ||
                                           (!string.IsNullOrEmpty(request.Search) &&
                                           (
                                                (!String.IsNullOrEmpty(x.User.FullName) && x.User.FullName.ToLower().Contains(request.Search.ToLower())) ||
                                                (!String.IsNullOrEmpty(x.User.IdentityCard) && x.User.IdentityCard.ToLower().Contains(request.Search.ToLower())) ||
                                                (!String.IsNullOrEmpty(x.User.ShareholderCode) && x.User.ShareholderCode.ToLower().Contains(request.Search.ToLower())) ||
                                                (!String.IsNullOrEmpty(x.User.PhoneNumber) && x.User.PhoneNumber.ToLower().Contains
                                                (request.Search.ToLower())) ||
                                                (!String.IsNullOrEmpty(x.User.UserEmail) && x.User.UserEmail.ToLower().Contains(request.Search.ToLower()))
                                            )
                                           )).ToList();

            double totakStockInEvent = 0;
            totakStockInEvent = mariaDBContext.EventUsers.Where(m => m.EventId.Equals(eventId)).Sum(m => m.UserStock);
            ResponseTable response = new ResponseTable
            {
                DateResult = new
                {
                    eventUsers = eventUsers
                                    .Skip((request.Page - 1) * request.Results)
                                    .Take(request.Results)
                                    .ToList(),
                    totalShare = mariaDBContext.Organizes.FirstOrDefault(x => x.OrganizeId.Equals(orgnazieID)).OrganizeStocks,
                    stockInEvent = totakStockInEvent
                },
                Info = new Info
                {
                    Page = request.Page,
                    Results = request.Results,
                    TotalRecord = eventUsers.Count(),
                }

            };

            return (response, "ShowListUserEventSuccess");
        }

        public (User result, string message) FilterUser(FilterShareholderCode filter)
        {
            User user = mariaDBContext.Users
                                      .FirstOrDefault(m =>
                                                        m.ShareholderCode.Equals(filter.ShareholderCode)
                                                        && m.OrganizeId.Equals(filter.OrganizeId)
                                                      );
            if (user is null)
            {
                return (user, "ShareholderCodeNotExist");
            }
            return (user, "ShowUserEventSuccess");
        }

        void AddUserPermissionByGroup(int groupId, string userId, string eventId)
        {
            List<PermissionGroup> permissionGroups = mariaDBContext.PermissionGroups
                                                                   .Where(m => m.GroupId == groupId)
                                                                   .ToList();
            List<UserPermissionEvent> permissionEvents = new List<UserPermissionEvent>();
            foreach (var item in permissionGroups)
            {
                permissionEvents.Add(new UserPermissionEvent
                {
                    EventId = eventId,
                    UserId = userId,
                    PermissionId = item.PermissionId,
                    PermissionCode = item.PermissionCode,
                });
            }
            mariaDBContext.UserPermissionEvents.AddRange(permissionEvents);
            mariaDBContext.SaveChanges();
        }

        void ImportUserPermissionByGroup(int groupId, string userId, string eventId, List<UserPermissionEvent> permissionEvents)
        {
            List<PermissionGroup> permissionGroups = mariaDBContext.PermissionGroups
                                                                   .Where(m => m.GroupId == groupId)
                                                                   .ToList();
            foreach (var item in permissionGroups)
            {
                permissionEvents.Add(new UserPermissionEvent
                {
                    EventId = eventId,
                    UserId = userId,
                    PermissionId = item.PermissionId,
                    PermissionCode = item.PermissionCode,
                });
            }
            //mariaDBContext.UserPermissionEvents.AddRange(permissionEvents);
            //mariaDBContext.SaveChanges();
        }
        public (List<User> users, string message) UserNoneStocks(string eventId)
        {
            Event @event = mariaDBContext.Events.Find(eventId);
            if (@event is null)
            {
                return (null, "EventNotExist");
            }
            //List<string> userIds = mariaDBContext.EventUsers.Where(m => m.EventId.Equals(eventId) && m.UserStock == 0)
            //                                              .Select(m => m.UserId)
            //                                            .ToList();
            List<string> userIds = mariaDBContext.UserPermissionEvents.Where(m => m.EventId.Equals(eventId)).GroupBy(m => m.UserId).Select(m=>m.Key).ToList();
            List<User> users = mariaDBContext.Users.Where(m => userIds.Contains(m.UserId)).ToList();
            return (users, "ShowListSuccess");
        }

        public (int responseStatus, string message) DeleteAllUserEvent(string eventId)
        {


            List<int> authoryCheck = mariaDBContext.Authorities.Where(m => m.EventID.Equals(eventId) && m.AuthorityType == AuthorityType.EVENT).Select(m => m.AuthorityID).ToList();
            if (authoryCheck.Count() > 0)
                return (400, "CheckAuthorizationBeforeDelete");
            List<string> userIds = mariaDBContext.EventUsers
                                                 .Where(m => m.EventId.Equals(eventId))
                                                 .Select(m => m.UserId)
                                                 .ToList();
            if(userIds.Count ==0)
            {
                return (200, "Success");
            }
            using (IDbContextTransaction transaction = mariaDBContext.Database.BeginTransaction())
            {
                try
                {


                    #region Xoa thu moi
                    List<Invitation> invitations = mariaDBContext.Invitations.Where(m => m.EventId.Equals(eventId)).ToList();
                    mariaDBContext.Invitations.RemoveRange(invitations);
                    mariaDBContext.SaveChanges();
                    #endregion

                    #region Xoa comment hoi dap
                    
                    #endregion
                    #region xoa hoi dap cua user
                    List<QuestionClient> questionClients = mariaDBContext.QuestionClients.Where(m => m.EventId.Equals(eventId)).ToList();
                    var comments = mariaDBContext.QuestionCommentClients.Where(x => questionClients.Select(v => v.QuestionId).Contains(x.QuestionClientId));
                    mariaDBContext.QuestionCommentClients.RemoveRange(comments);
                    mariaDBContext.SaveChanges();

                    mariaDBContext.QuestionClients.RemoveRange(questionClients);
                    mariaDBContext.SaveChanges();
                    #endregion

                    #region Xoa quyen user trong event
                    List<UserPermissionEvent> userPermissionEvents = mariaDBContext.UserPermissionEvents
                                                                                   .Where(m => m.EventId.Equals(eventId))
                                                                                   .ToList();
                    mariaDBContext.RemoveRange(userPermissionEvents);
                    #endregion

                    #region Xoa vai tro va quyen cua vai tro

                    List<int> groupIds = mariaDBContext.Groups.Where(m => m.EventId.Equals(eventId))
                                                              .Select(m => m.GroupID)
                                                              .ToList();

                    List<PermissionGroup> permissionGroups = mariaDBContext.PermissionGroups
                                                                           .Where(m => groupIds.Equals(m.GroupId))
                                                                           .ToList();

                    mariaDBContext.PermissionGroups.RemoveRange(permissionGroups);

                    List<Group> groupDeletes = mariaDBContext.Groups.Where(m => m.EventId.Equals(eventId)).ToList();
                    mariaDBContext.Groups.RemoveRange(groupDeletes);

                    #endregion

                    #region Xoa user trong event

                    List<EventUser> eventUsers = mariaDBContext.EventUsers.Where(m => m.EventId.Equals(eventId)).ToList();
                    mariaDBContext.EventUsers.RemoveRange(eventUsers);

                    #endregion

                    #region Xoa user va quyen mac dinh trong bảng user tổ chức

                    List<string> userIdNotDelete = mariaDBContext.EventUsers
                                                                .Where(m => userIds.Contains(m.UserId) && !(m.EventId.Equals(eventId)))
                                                                .Select(m => m.UserId)
                                                                .ToList();

                    List<string> userIdDeletes = new List<string>();
                    foreach (string item in userIds)
                    {
                        if (!userIdNotDelete.Contains(item))
                        {
                           
                            userIdDeletes.Add(item);
                        }
                    }
                    List<User> usersDelete = mariaDBContext.Users.Where(m => userIdDeletes.Contains(m.UserId)).ToList();
                    
                    List<PermissionUser> permissionUsers = mariaDBContext.PermissionUsers.Where(m => userIds.Contains(m.UserId)).ToList();
                    mariaDBContext.PermissionUsers.RemoveRange(permissionUsers);
                    mariaDBContext.Users.RemoveRange(usersDelete);

                    #endregion

                    mariaDBContext.SaveChanges();
                    transaction.Commit();
                    return (200, "");

                }
                catch
                {
                    transaction.Rollback();
                    return (400, "Xóa thất bại.");

                }
            }
        }

        public List<User> GetListUserFromOrganize(string orgnaizeID, string eventID)
        {
            List<string> userIds = mariaDBContext.EventUsers.Where(x => x.EventId.Equals(eventID)).Select(x => x.UserId).ToList();
            List<User> users = mariaDBContext.Users.Where(x => x.OrganizeId.Equals(orgnaizeID) && !userIds.Contains(x.UserId)).ToList();
            return users;
        }

        public (ResponseTable response, string message) ShowUsersStartEvent(string eventId, RequestTable request)
        {
            var checkEvent = mariaDBContext.Events.Find(eventId);
            if (checkEvent is null)
                return (null, "EventNotExist");
            ResponseTable response = new ResponseTable() 
            {
                DateResult = new List<object>(),
                Info = new Info
                {
                    Page = 0,
                    Results = 0,
                    TotalRecord =0,
                }
            };
            if (checkEvent.EventFlag != EVENT_FLAG.BEGIN)
            {
                return (response, "EventNotYetStart");
            }    
            List<EventUser> users = mariaDBContext.EventUsers.Where(m => m.EventId.Equals(eventId)  && m.UserLatch == USER_LATCH.ON)
                                                        .Include(m => m.User)
                                                        .Select(m => new EventUser
                                                        {
                                                            UserId = m.UserId,
                                                            EventId = m.EventId,
                                                            GroupId = m.GroupId,
                                                            UserLoginStatus = m.UserLoginStatus,
                                                            EventUserId = m.EventUserId,
                                                            User = m.User,
                                                            UserStock = m.UserStock,
                                                            Group = m.Group,
                                                            SendInvitation = m.SendInvitation,
                                                            UserActive = m.UserActive,
                                                            UserInviteStatus = m.UserInviteStatus
                                                        })
                                              .ToList();
           
            if (request.Page == 0)
            {
                response = new ResponseTable
                {
                    DateResult = users,
                    Info = new Info
                    {
                        Page = 1,
                        Results = users.Count,
                        TotalRecord = users.Count,
                    }
                };
            }
            else
            {
                response = new ResponseTable
                {
                    DateResult = users.Skip((request.Page-1)*request.Results).Take(request.Results).ToList(),
                    Info = new Info
                    {
                        Page = 1,
                        Results = users.Count,
                        TotalRecord = users.Count,
                    }
                };
            }
            return (response, "ShowListUserEventLoginSuccess");
        }

        public (bool status, string message, string code) LogOutLanding(string eventId, string userId)
        {
            EventUser user = mariaDBContext.EventUsers.Where(m => m.UserId.Equals(userId) && m.EventId.Equals(eventId)).FirstOrDefault();
            if(user is null)
            {
                return (false, "TokenFaild", "TokenFaild");
            }
            user.UserLoginStatus =USER_LOGIN_STATUS.OFF;
            mariaDBContext.EventUsers.Update(user);
            mariaDBContext.SaveChanges();
            return (true, "LogOutSuccess", "LogOutSuccess");
        }

        public (ResponseTable response, string message) ShowUsersLoginEvent(string eventId, RequestTable request) // Danh sách chốt trước Event
        {
            var checkEvent = mariaDBContext.Events.Find(eventId);
            if (checkEvent is null)
                return (null, "EventNotExist");
            ResponseTable response = new ResponseTable()
            {
                DateResult = new List<object>(),
                Info = new Info
                {
                    Page = 0,
                    Results = 0,
                    TotalRecord = 0,
                }
            };
            if (checkEvent.EventFlag == EVENT_FLAG.CREATED)
            {
                return (response, "EventNotYetStart");
            }

            List<EventUser> users = mariaDBContext.EventUsers.Where(m => m.EventId.Equals(eventId) && !(m.LoginAt ==null ) && m.UserLatch==USER_LATCH.ON)
                                                        .Include(m => m.User)
                                                        .Select(m => new EventUser
                                                        {
                                                            UserId = m.UserId,
                                                            EventId = m.EventId,
                                                            GroupId = m.GroupId,
                                                            UserLoginStatus = m.UserLoginStatus,
                                                            EventUserId = m.EventUserId,
                                                            LoginAt = m.LoginAt,
                                                            User = m.User,
                                                            UserStock = m.UserStock,
                                                            Group = m.Group,
                                                            SendInvitation = m.SendInvitation,
                                                            UserActive = m.UserActive,
                                                           
                                                        })
                                              .ToList();

           // ResponseTable response;
            if (request.Page == 0)
            {
                response = new ResponseTable
                {
                    DateResult = users,
                    Info = new Info
                    {
                        Page = 1,
                        Results = users.Count,
                        TotalRecord = users.Count,
                    }
                };
            }
            else
            {
                response = new ResponseTable
                {
                    DateResult = users.Skip((request.Page - 1) * request.Results).Take(request.Results).ToList(),
                    Info = new Info
                    {
                        Page = 1,
                        Results = users.Count,
                        TotalRecord = users.Count,
                    }
                };
            }
            return (response, "ShowListUserEventLoginSuccess");
        }

        public (List<ItemImportUserToEvent> result, string message) ImportUserInEvent(ImportRequest request)
        {

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            List<User> users = mariaDBContext.EventUsers.Where(m => m.EventId.Equals(request.EventId))
                                                        .Include(m => m.User)
                                                        .Select(m => m.User)
                                                        .ToList();
            //Console.WriteLine("---------check include user-------"+stopwatch.ElapsedMilliseconds);
            //stopwatch.Reset();
            //stopwatch.Start();
            //stopwatch.Stop();



            List<string> shareholderCodes = users.Where(m => m.ShareholderCode != null).Select(m => m.ShareholderCode).ToList();
          
            Event _event = mariaDBContext.Events.Where(m => m.EventId.Equals(request.EventId))
                                                .Include(m => m.Organize)
                                                .FirstOrDefault();

            #region Kiem tra tong so co phan truyen vao

            List<string> sharehodlerCodeNew = request.ImportUsers.Select(m => m.ShareholderCode).ToList();
            List<string> duplicateUserId = users.Where(m => sharehodlerCodeNew.Contains(m.ShareholderCode)).Select(m => m.UserId).ToList();
         
            int totalStockRequest = request.ImportUsers.Sum(m => m.UserStock);
            int totalStockEvent = mariaDBContext.EventUsers
                                                      .Where(m => m.EventId.Equals(request.EventId)&&!duplicateUserId.Contains(m.UserId))
                                                      .Sum(m => m.UserStock);

            if (totalStockRequest + totalStockEvent > _event.Organize.OrganizeStocks)
            {
                return (null, "TotalSumStockEventUserIsLargerThanStockOrganize");
            }

            #endregion
           
            #region Khai bao bien

            List<EventUser> listEventUsers = new List<EventUser>();

            // Lấy danh sách cần gửi mail: thêm mới hoàn toàn user có quyền và cập nhật quyền của user trong event và chưa gửi thư
            List<User> lstUsersSendMail = new List<User>();

            List<User> lstUsers = new List<User>();
            string saft;
            string password;
            User user;
            EventUser eventUser;
            List<UserPermissionEvent> permissionEvents = new List<UserPermissionEvent>();
            List<PermissionUser> permissionUsers = new List<PermissionUser>();
            #endregion

            System.Text.RegularExpressions.Regex phone = new System.Text.RegularExpressions.Regex(@"(0)+([0-9]{9})");
            System.Text.RegularExpressions.Regex mail = new System.Text.RegularExpressions.Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");

            // lấy số luong quyen cua group
            int totalPermissionGroup = mariaDBContext.PermissionGroups.Count(m => m.GroupId == request.GroupId);
            // lấy tất cả mã cổ đông của tổ chức
            List<string> shareholderOranizes = mariaDBContext.Users.Where(m=>m.OrganizeId.Equals(_event.OrganizeId))
                .Select(m=>m.ShareholderCode).ToList();
            // lấy tất cả user có trong tổ chức
            List<User> userOrganize = mariaDBContext.Users.Where(m => m.OrganizeId.Equals(_event.OrganizeId)).ToList();
            foreach (var item in request.ImportUsers)
            {

                if (!(phone.IsMatch(item.PhoneNumber)) || !(mail.IsMatch(item.UserEmail)))
                {
                    item.ImportStatus = false;
                    continue;
                }

               item.ImportStatus = true;
               saft = 5.RandomString();
               password = item.Password is null? 6.RandomString() : item.Password;
               
                // nếu mã cổ đông đã tồn tại trong tổ chức
                if (shareholderOranizes.Contains(item.ShareholderCode))
                {
                    //lấy user có mã cổ đông trong request
                    user = userOrganize.Where(m => shareholderOranizes.Contains(m.ShareholderCode)&&m.ShareholderCode.Equals(item.ShareholderCode)&& m.OrganizeId.Equals(_event.OrganizeId)).FirstOrDefault();
                    eventUser = mariaDBContext.EventUsers.Where(m => m.EventId.Equals(request.EventId) && (m.UserId.Equals(user.UserId))).FirstOrDefault();
                    
                    #region Người đã có trong tổ chức thêm vào event
                   
                    if (eventUser is null)
                    {   
                            eventUser = new EventUser
                            {
                                EventId = _event.EventId,
                                UserId = user.UserId,
                                UserStock = item.UserStock,
                                GroupId = request.GroupId,
                                UserPassword = (saft + password).HashPassword(),
                                UserSalt = saft,
                                PasswordSystem = password,
                            };
                            listEventUsers.Add(eventUser);
                    } 
                    else
                    {
                        eventUser.UserStock = item.UserStock;
                        mariaDBContext.EventUsers.Update(eventUser);

                    }
                    // nếu dữ liệu user thay đổi cập nhật lại
                    user.FullName = item.FullName is null ? user.FullName : item.FullName;
                    user.UserEmail = item.FullName is null ? user.UserEmail : item.UserEmail;
                    user.PhoneNumber = item.PhoneNumber;
                    user.PlaceOfIssue = item.PlaceOfIssue is null ? user.PlaceOfIssue : item.PlaceOfIssue;
                    user.IdentityCard = item.IdentityCard is null ? user.IdentityCard : item.IdentityCard;
                    user.IssueDate = item.IssueDateFormat;
                    mariaDBContext.Users.Update(user);
                
                    #endregion

                    if(eventUser.SendMailCMS== User.SEND_STATUS.CHUA && totalPermissionGroup>0)
                    {
                        eventUser.SendMailCMS = User.SEND_STATUS.DA;
                        mariaDBContext.EventUsers.Update(eventUser);
                        lstUsersSendMail.Add(user);
                    }    
                }
                else
                {
                    
                    #region mã cổ đông chưa có trong tổ chức thêm vào bảng user
                    user = new User
                    {
                        UserId = Guid.NewGuid().ToString(),
                        OrganizeId = _event.OrganizeId,
                        ShareholderCode = item.ShareholderCode,
                        UserEmail = item.UserEmail,
                        IdentityCard = item.IdentityCard,
                        PlaceOfIssue = item.PlaceOfIssue,
                        IssueDate = item.IssueDateFormat,
                        StockCode = _event.Organize.OrganizeCodeCk,
                        PhoneNumber = item.PhoneNumber,
                        FullName = item.FullName,
                    };


                    #endregion

                    #region thêm vào bảng User Event

                    eventUser = new EventUser
                    {
                        EventId = _event.EventId,
                        UserId = user.UserId,
                        UserStock = item.UserStock,
                        GroupId = request.GroupId,
                        UserPassword = (saft + password).HashPassword(),
                        UserSalt = saft,
                        PasswordSystem = password,
                    };

                    #endregion
                    if (totalPermissionGroup > 0)
                    {
                        user.UserSalt = saft;
                        user.UserPassword = (saft + password).HashPassword();
                        user.PasswordSystem = password;
                        lstUsersSendMail.Add(user);
                        eventUser.SendMailCMS = User.SEND_STATUS.DA;
                    }
                    lstUsers.Add(user);
                    listEventUsers.Add(eventUser);

                }


            }

            //int countPermission = 0;

            //Nếu group truyền vào có quyền sẽ gửi mail thông báo cho tất cả
            if( totalPermissionGroup >0)
            {
                foreach(var itemUser in lstUsers)
                {
                    ImportUserPermissionByGroup(request.GroupId, itemUser.UserId, request.EventId, permissionEvents);
                }
                mariaDBContext.UserPermissionEvents.AddRange(permissionEvents);
            }    
           

            mariaDBContext.Users.AddRange(lstUsers);
            mariaDBContext.EventUsers.AddRange(listEventUsers);


            foreach (var item in listEventUsers)
            {
                permissionUsers.Add(new PermissionUser { PermissionCode = "CLIENT", UserId = item.UserId });
                if (totalPermissionGroup > 0)
                {
                    item.SendMailCMS = User.SEND_STATUS.DA;
                }
            }
            mariaDBContext.PermissionUsers.AddRange(permissionUsers);
            mariaDBContext.SaveChanges();
            if (totalPermissionGroup > 0)
            {
                foreach (var userSend in lstUsersSendMail)
                {
                    SendMailInOrganize(userSend, _event.Organize);
                }
            }
            mariaDBContext.SaveChanges();
            return (request.ImportUsers, "ImportUserInEventSuccess");
        }

    }
}
