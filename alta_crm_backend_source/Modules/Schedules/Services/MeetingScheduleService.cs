using AutoMapper;
using Confluent.Kafka;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using Project.App.DesignPatterns.ObserverPatterns;
using Project.App.Helpers;
using Project.App.Paginations;
using Project.App.Requests;
using Project.Modules.Kafka.Producer;
using Project.Modules.Schedules.Entities;
using Project.Modules.Schedules.Models;
using Project.Modules.Schedules.Requests;
using Project.Modules.Tags.Enities;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Service;
using Project.Modules.UsersModes.Entities;
using Project.Modules.UsersModes.Services;
using Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Services
{
    public interface IMeetingScheduleService
    {
        (object data, string message) ImportScheduleCustomer(ImportScheduleCustomerRequest request);
        (object data, string message) StoreMettingSchedule(StoreScheduleCustomerRequest request);
        (object data, string message) SendKafkaForRegisterFaceId(string userId, string modeId = "");
        (object data, string message) DeleteMeetingScheduleByUser(string userId);
        (object data, string message) Delete(string scheduleId);
        (PaginationRes<Schedule> data, string message) GetSchedule(string userId, GetScheduleRequest request);
        PaginationRes<Schedule> GetAllSchedule(GetScheduleRequest request);
        PaginationRes<Schedule> GetAllScheduleByType(GetScheduleByType request);
        (object data, string message) RegisterMeetingWithTagMode(RegisterMeetingWithTagMode request);
        (object data, string message) RegisterMeetingWithTagModeImport(RegisterMeetingWithTagMode request);
        (object data, string message) DeleteMeetingWithTagMode(RegisterMeetingWithTagMode request);
        void deliveryReportHandleString(DeliveryReport<string, string> deliveryReport);
        public (object data, string message) DeleteRegisterDetect(string userId);

        public (object data, string message) RegisterMeetingScheduleByUserId(string userId, List<string> modeIds);
    }
    public class MeetingScheduleService : IMeetingScheduleService
    {
        private readonly IConfiguration Configuration;
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;
        private readonly KafkaDependentProducer<string, string> Producer;
        private readonly HandleTaskSchedule<RegisterUserResponse> RegisterUserResponseTasks;
        private readonly IUserService UserService;
        private readonly HelperService HelperService;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly int TimeOut;
        private readonly int WaitingKafka;

        private readonly IMapper _mapper;

        public MeetingScheduleService(IRepositoryWrapperMariaDB repositoryWrapperMariaDB, KafkaDependentProducer<string, string> producer, HandleTaskSchedule<RegisterUserResponse> registerUserResponseTasks, IUserService userService, IConfiguration configuration, HelperService helperService, IServiceScopeFactory scopeFactory , IMapper mapper)
        {
            Configuration = configuration;
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;
            Producer = producer;
            RegisterUserResponseTasks = registerUserResponseTasks;
            UserService = userService;
            HelperService = helperService;
            this.scopeFactory = scopeFactory;
            TimeOut = int.Parse(Configuration["Kafka:TimeoutCache"]);
            WaitingKafka = int.Parse(Configuration["Kafka:WaitingKafka"]);

            _mapper = mapper;
        }

        #region For MEETING_SYSTEM

        public (object data, string message) ImportScheduleCustomer(ImportScheduleCustomerRequest request)
        {
            string webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string path = request.FileExcel.UploadFile(webRootPath).Result;
            FileInfo fileExcel = new FileInfo(webRootPath + "/" + path);

            Tag tag = RepositoryWrapperMariaDB.Tags.FindByCondition(x => x.TagCode.Equals("default_tag")).FirstOrDefault();
            if (tag is null) return (null, "TagDefaultIsNull");

            Dictionary<string, string> emailDB = RepositoryWrapperMariaDB.Users.FindAll().Select(x => x.UserEmail).ToDictionary(x => x, x => x);
            Dictionary<string, string> codeDb = RepositoryWrapperMariaDB.UserCodeds.FindAll().Select(x => x.UserCodeActive).ToDictionary(x => x, x => x);
            (List<User> users, string message) = ReadExcel(fileExcel, emailDB, codeDb, tag.TagId);

            if (users is null || !users.Any(x => x.IsImportSuccess == true)) { return (null, "ImportCustomerFaild"); }
            string ScheduleValue = ((request.ScheduleRepeatType != ScheduleRepeatType.NoRepeat && request.ScheduleRepeatType != ScheduleRepeatType.Daily) && request.ScheduleValues.Count > 0)
               ? JsonConvert.SerializeObject(request.ScheduleValues)
               : null;


            IDbContextTransaction transaction = RepositoryWrapperMariaDB.BeginTransaction();
            try
            {
                List<string> userEmailOld = users.Where(x => x.IsImportSuccess == true && x.UserOld == true).Select(x => x.UserEmail).ToList();
                List<User> oldUsers = RepositoryWrapperMariaDB.Users.FindByCondition(x => userEmailOld.Contains(x.UserEmail)).ToList();
                foreach (var item in users.Where(x => x.IsImportSuccess && x.UserOld))
                {
                    var tmpOldUser = oldUsers.FirstOrDefault(x => x.UserEmail.Equals(item.UserEmail));
                    var tmpUserId = tmpOldUser.UserId;
                    tmpOldUser = _mapper.Map<User,User>(item, tmpOldUser);
                    tmpOldUser.UserId = tmpUserId;
                }
                

                RepositoryWrapperMariaDB.Users.AddRange(users.Where(x => x.IsImportSuccess && !x.UserOld).ToList());

                RepositoryWrapperMariaDB.SaveChanges();

                var newUserCodes= users.Where(x => x.IsImportSuccess).Select(x => new UserCodes.Enities.UserCode
                {
                    UserCodeActive = x.UserCode,
                    UserCodeExpire = DateTime.UtcNow.AddDays(30),
                    UserId = x.UserId
                }).ToList();

                RepositoryWrapperMariaDB.UserCodeds.AddRange(newUserCodes);
                #region Create Schedule Customer  
                var newSchedules = users.Where(x => x.IsImportSuccess).Select(x => new Schedule
                {
                    ScheduleName = request.ScheduleName,
                    ScheduleDescription = request.ScheduleDescription,
                    TagId = tag.TagId,
                    TicketId = request.TicketId,
                    UserId = x.UserId,
                    ScheduleDateStart = request.ScheduleDateStart.ParseStringGMTToDatime(),
                    ScheduleDateEnd = request.ScheduleDateEnd.ParseStringGMTToDatime(),
                    ScheduleTimeStart = TimeSpan.Parse(request.ScheduleTimeStart),
                    ScheduleTimeEnd = TimeSpan.Parse(request.ScheduleTimeEnd),
                    ScheduleValue = ScheduleValue,
                    ScheduleRepeatType = request.ScheduleRepeatType
                }).ToList();
                RepositoryWrapperMariaDB.Schedules.AddRange(newSchedules);
                RepositoryWrapperMariaDB.SaveChanges();
                #endregion


                List<UserMode> userModes = RepositoryWrapperMariaDB.UserModes.FindByCondition(x => oldUsers.Select(x => x.UserId).ToList().Contains(x.UserId)).ToList();

                #region Lịch họp
                foreach (var item in oldUsers)
                {
                    (object registerMettingScheduleTmp, string messageMettingScheduleTmp) = this.RegisterMeetingScheduleByUserId(item.UserId, userModes.Where(x=> x.UserId.Equals(item.UserId)).Select(x=> x.UserModeId).ToList());
                    if (registerMettingScheduleTmp is null)
                    {
                        transaction.Rollback();
                        return (null, " Đăng ký user tag mode cho user error");
                    }
                }
               
                #endregion
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (null, $"Error:{ex.Message}");
            }



            #region Send mail
            foreach (User user in users.Where(x => x.IsImportSuccess && !x.UserOld).ToList())
            {
                
                    ObserverPattern.Instance.Emit("SendEmail", new SendMailRequest
                    {
                        MessageSubject = "Altamedia Calendar",
                        MessageContent = $"Code : {user.UserCode}"
                                                           + $"<br/> Hi {user.UserFirstName} {user.UserLastName} this is your schedule to visit Altamedia"
                                                           + $"<br/> Starting on : {request.ScheduleDateStart} at {request.ScheduleTimeStart}"
                                                           + $"<br/> Ends on : {request.ScheduleDateEnd} at {request.ScheduleTimeEnd}",
                        Contacts = new List<SendMailContact>
                                    {
                                        new SendMailContact{ContactEmail = user.UserEmail}
                                    }
                    });
                
               
            }
            GeneralHelper.DeleteFile(path);
            #endregion

            return (users, "ImportSuccess");
        }

        public (object data, string message) StoreMettingSchedule(StoreScheduleCustomerRequest request)
        {
            string ScheduleValue = (
                (request.ScheduleRepeatType != ScheduleRepeatType.NoRepeat && request.ScheduleRepeatType != ScheduleRepeatType.Daily) && request.ScheduleValues.Count > 0)
                ? JsonConvert.SerializeObject(request.ScheduleValues)
                : null;

            Tag tag = RepositoryWrapperMariaDB.Tags.FirstOrDefault(x => x.TagCode.Equals("default_tag"));
            if (tag is null) return (null, "TagDefaultIsNull");


            #region Handling Customer
            User user = RepositoryWrapperMariaDB.Users.FindByCondition(x => x.UserEmail.Equals(request.UserEmail)).FirstOrDefault();
            List<string> customerCodes = RepositoryWrapperMariaDB.Users.FindAll().Select(x => x.UserCode).ToList();
            if (user is null)
            {
                user = new User
                {
                    GroupId = "group_default",
                    UserAddress = request.UserAddress,
                    UserFirstName = request.UserFirstName,
                    UserLastName = request.UserLastName,
                    UserEmail = request.UserEmail,
                    UserGender = request.UserGender,
                    UserPhone = request.UserPhone,
                    UserCode = GeneralHelper.RandomCode(6, customerCodes),
                    UserTagIds = JsonConvert.SerializeObject(new List<string> { tag.TagId })
                };
                RepositoryWrapperMariaDB.Users.Add(user);
                RepositoryWrapperMariaDB.SaveChanges();
            }
            #endregion

            #region Handling Schedule

            Schedule schedule = new Schedule
            {
                ScheduleName = request.ScheduleName,
                ScheduleDescription = request.ScheduleDescription,
                TagId = tag.TagId,
                ModeId = request.ModeId,
                TicketId = request.TicketId,
                UserId = user.UserId,
                ScheduleDateStart = request.ScheduleDateStart.ParseStringGMTToDatime(),
                ScheduleDateEnd = request.ScheduleDateEnd.ParseStringGMTToDatime(),
                ScheduleTimeStart = TimeSpan.Parse(request.ScheduleTimeStart),
                ScheduleTimeEnd = TimeSpan.Parse(request.ScheduleTimeEnd),
                ScheduleValue = ScheduleValue,
                ScheduleRepeatType = request.ScheduleRepeatType
            };
            RepositoryWrapperMariaDB.Schedules.Add(schedule);
            RepositoryWrapperMariaDB.SaveChanges();


            #endregion

            #region REGISTER_USER_REQUEST

            string keyCode = HelperService.GetKeyCode(user.UserId, request.ModeId);

            if (!string.IsNullOrEmpty(keyCode))
            {
                JArray jArray = new JArray();
                jArray.Add(JObject.FromObject(new
                {
                    rgDectectDetailDateBegin = request.ScheduleDateStart,
                    rgDectectDetailDateEnd = request.ScheduleDateEnd,
                    rgDectectDetailTimeBegin = request.ScheduleTimeStart,
                    rgDectectDetailTimeEnd = request.ScheduleTimeEnd,
                    rgDectectDetailRepeat = schedule.ScheduleRepeatType,
                    rgDectectDetailRepeatValueData = ScheduleValue
                }));

                object requestDataKafka = new
                {
                    transactionId = schedule.ScheduleId,
                    rgDectectUserId = schedule.UserId,
                    rgDectectKey = keyCode,
                    modeId = schedule.ModeId,
                    tagId = schedule.TagId,
                    tagCode = tag.TagCode,
                    ticketTypeId = schedule.TicketId,
                    registerDettectDetailRequests = jArray,
                    rgDectectExtension = JsonConvert.SerializeObject(new { UserFirstName = user.UserFirstName, UserLastName = user.UserLastName })
                };

                Message<string, string> message = new Message<string, string> { Key = DateTime.UtcNow.Ticks.ToString(), Value = JsonConvert.SerializeObject(requestDataKafka) };
                this.Producer.Produce("REGISTER_USER_REQUEST", message, deliveryReportHandleString);
            }

            #endregion

            #region Send Mail
            if (!string.IsNullOrEmpty(keyCode))
            {
                ObserverPattern.Instance.Emit("SendEmail", new SendMailRequest
                {
                    MessageSubject = "HDBank Calendar",
                    MessageContent = $"Hi {user.UserLastName} {user.UserFirstName} this is your schedule to visit Altamedia" + $"<br/> Starting on : {request.ScheduleDateStart} at {request.ScheduleTimeStart}"
                                                                             + $"<br/> Ends on : {request.ScheduleDateEnd} at {request.ScheduleTimeEnd}",
                    Contacts = new List<SendMailContact>
                {
                    new SendMailContact{ContactEmail = request.UserEmail}
                }
                });
            }
            else
            {
                ObserverPattern.Instance.Emit("SendEmail", new SendMailRequest
                {
                    MessageSubject = "Altamedia Calendar",
                    MessageContent = $"Code : {user.UserCode}"
                                                                             + $"<br/> Hi {user.UserFirstName} {user.UserLastName} this is your schedule to visit Altamedia"
                                                                             + $"<br/> Starting on : {request.ScheduleDateStart} at {request.ScheduleTimeStart}"
                                                                             + $"<br/> Ends on : {request.ScheduleDateEnd} at {request.ScheduleTimeEnd}",
                    Contacts = new List<SendMailContact>
                {
                    new SendMailContact{ContactEmail = request.UserEmail}
                }
                });
            }

            #endregion

            return (schedule, "Success");
        }


        public (object data, string message) SendKafkaForRegisterFaceId(string userId, string modeId = "")
        {
            User user = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            if (user is null)
            {
                return (null, "UserNotFound");
            }

            List<Schedule> schedules = RepositoryWrapperMariaDB.Schedules.FindByCondition(x => x.UserId.Equals(userId) && (String.IsNullOrEmpty(modeId) || x.ModeId.Equals(modeId))).ToList();
            List<object> requestDatasKafka = new List<object>();

            string key = DateTime.UtcNow.Ticks.ToString();
            //RegisterUserResponseTasks.HandleTasks.Add(key, false);

            foreach (Schedule schedule in schedules)
            {
                Tag tag = RepositoryWrapperMariaDB.Tags.FirstOrDefault(x => x.TagId.Equals(schedule.TagId));


                object requestDataKafka = new
                {
                    rgDectectUserId = userId,
                    rgDectectExtension = JsonConvert.SerializeObject(new { UserFirstName = user.UserFirstName, UserLastName = user.UserLastName }),
                    rgDectectKey = HelperService.GetKeyCode(userId, modeId),
                    modeId = modeId,
                    tagId = tag?.TagId,
                    tagCode = tag?.TagCode,
                    ticketTypeId = schedule.TicketId,
                    registerDettectDetailRequests = new List<object>
                    {
                        new
                        {
                            rgDectectDetailDateBegin = schedule.ScheduleDateStart.ToString("yyyy-MM-dd HH:mm:ss"),
                            rgDectectDetailDateEnd = schedule.ScheduleDateEnd.ToString("yyyy-MM-dd HH:mm:ss"),
                            rgDectectDetailTimeBegin = schedule.ScheduleTimeStart.ToString(),
                            rgDectectDetailTimeEnd = schedule.ScheduleTimeEnd.ToString(),
                            rgDectectDetailRepeat = schedule.ScheduleRepeatType,
                            rgDectectDetailRepeatValueData = schedule.ScheduleValue is null ? new List<string>() : JsonConvert.DeserializeObject(schedule.ScheduleValue)
                        }
                    }
                };
                requestDatasKafka.Add(requestDataKafka);
            }

            object requestRegisterScheduleKafka = new
            {
                RegisterUserDetects = requestDatasKafka
            };

            #region REGISTER_USER_REQUEST_MULTIPLES

            if (schedules.Count > 0)
            {
                Message<string, string> message = new Message<string, string> { Key = key, Value = JsonConvert.SerializeObject(requestRegisterScheduleKafka) };
                this.Producer.Produce("REGISTER_USER_MUTIL_REQUEST", message, deliveryReportHandleString);

                //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                //cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(TimeOut));


                //while (!RegisterUserResponseTasks.Get(key) && !cancellationTokenSource.IsCancellationRequested)
                //{
                //    Console.WriteLine("Register User Request Task Running");
                //    Thread.Sleep(WaitingKafka);
                //}
                //var data = RegisterUserResponseTasks.GetData(key);
                //RegisterUserResponseTasks.Remove(key);
                //if (cancellationTokenSource.IsCancellationRequested)
                //{
                //    return (null, "TimeOut");
                //}

                //if (!data.IsSuccess)
                //{
                //    return (null, data.Message);
                //}

                return (schedules, "Success");
            }

            #endregion

            return (schedules, "Success");
        }

        public (object data, string message) Delete(string scheduleId)
        {
            Schedule schedule = RepositoryWrapperMariaDB.Schedules.FirstOrDefault(x => x.ScheduleId.Equals(scheduleId));

            if (schedule is null)
            {
                return (null, "ScheduleNotFound");
            }

            User user = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(schedule.UserId));

            Tag tag = RepositoryWrapperMariaDB.Tags.FirstOrDefault(x => x.TagId.Equals(schedule.TagId));

            RepositoryWrapperMariaDB.Schedules.Remove(schedule);

            #region UN_REGISTER_USER_REQUEST

            string key = DateTime.UtcNow.Ticks.ToString();

            object requestDataKafka = new
            {
                rgDectectUserId = schedule.UserId,
                rgDectectKey = HelperService.GetKeyCode(schedule.UserId, schedule.ModeId),
                modeId = schedule.ModeId,
                tagId = schedule.TagId,
                tagCode = tag?.TagCode,
                ticketTypeId = schedule.TicketId,
                transactionId = key,
                rgDectectDetailDateBegin = schedule.ScheduleDateStart,
                rgDectectDetailDateEnd = schedule.ScheduleDateEnd,
                rgDectectDetailTimeBegin = schedule.ScheduleTimeStart,
                rgDectectDetailTimeEnd = schedule.ScheduleDateEnd,
                rgDectectDetailRepeat = schedule.ScheduleRepeatType,
                rgDectectDetailRepeatValueData = schedule.ScheduleValue,
                rgDectectExtension = JsonConvert.SerializeObject(new { UserFirstName = user.UserFirstName, UserLastName = user.UserLastName })
            };

            RegisterUserResponseTasks.HandleTasks.Add(key, false);

            Message<string, string> message = new Message<string, string> { Key = key, Value = JsonConvert.SerializeObject(requestDataKafka) };
            this.Producer.Produce("UN_REGISTER_USER_REQUEST", message, deliveryReportHandleString);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(TimeOut));

            while (!RegisterUserResponseTasks.Get(key) && !cancellationTokenSource.IsCancellationRequested)
            {
                Console.WriteLine("Un-Register User Request Task Running");
                Thread.Sleep(500);
            }

            var data = RegisterUserResponseTasks.GetData(key);
            RegisterUserResponseTasks.Remove(key);

            if (cancellationTokenSource.IsCancellationRequested)
            {
                return (null, "TimeOut");
            }

            if (data.IsSuccess)
            {
                RepositoryWrapperMariaDB.SaveChanges();
                return ("Success", "Success");
            }

            #endregion

            return (null, "DeleteFail");
        }

        public (PaginationRes<Schedule> data, string message) GetSchedule(string userId, GetScheduleRequest request)
        {
            var schedules = RepositoryWrapperMariaDB.Schedules.FindByCondition(x => x.UserId.Equals(userId)).ToList();

            if (schedules.Count == 0)
            {
                return (null, "UserDontHaveSchedule");
            }

            var datas = PaginationSort<Schedule>.ApplySort(schedules.AsQueryable(), request.OrderByQuery);
            Pagination<Schedule> result = Pagination<Schedule>.ToPagedList(datas, request.PageNumber, request.PageSize);
            PaginationRes<Schedule> response = new PaginationRes<Schedule>(result, result.PageInfo);
            return (response, "Success");
        }

        public PaginationRes<Schedule> GetAllSchedule(GetScheduleRequest request)
        {
            var schedules = RepositoryWrapperMariaDB.Schedules.FindAll().ToList();
            var datas = PaginationSort<Schedule>.ApplySort(schedules.AsQueryable(), request.OrderByQuery);
            Pagination<Schedule> result = Pagination<Schedule>.ToPagedList(datas, request.PageNumber, request.PageSize);
            PaginationRes<Schedule> response = new PaginationRes<Schedule>(result, result.PageInfo);
            return (response);
        }

        public PaginationRes<Schedule> GetAllScheduleByType(GetScheduleByType request)
        {
            List<string> userIds = new List<string>();

            if (request.UserType == 1)
            {
                userIds.AddRange(RepositoryWrapperMariaDB.Users.FindByCondition(x => !x.GroupId.Equals("customer")).Select(x => x.UserId));
            }

            if (request.UserType == 0)
            {
                userIds.AddRange(RepositoryWrapperMariaDB.Users.FindByCondition(x => x.GroupId.Equals("customer")).Select(x => x.UserId));
            }

            var schedules = RepositoryWrapperMariaDB.Schedules.FindByCondition(x => userIds.Contains(x.UserId)).ToList();
            var datas = PaginationSort<Schedule>.ApplySort(schedules.AsQueryable(), request.OrderByQuery);
            Pagination<Schedule> result = Pagination<Schedule>.ToPagedList(datas, request.PageNumber, request.PageSize);
            PaginationRes<Schedule> response = new PaginationRes<Schedule>(result, result.PageInfo);
            return (response);
        }

        #endregion

        #region For USER_TAG_MODE

        public (object data, string message) RegisterMeetingScheduleByUserId(string userId, List<string> modeIds)
        {
            User user = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            if (user is null) return (null, "UserNotFound");

            List<object> requestDatasKafka = new List<object>();
            List<Schedule> schedules = RepositoryWrapperMariaDB.Schedules.FindByCondition(x => x.UserId.Equals(userId)).ToList();
            foreach (var schedule in schedules)
            {
                Tag tag = RepositoryWrapperMariaDB.Tags.FirstOrDefault(x => x.TagId.Equals(schedule.TagId));

                foreach (var modeId in modeIds)
                {
                    object requestDataKafka = new
                    {
                        rgDectectUserId = userId,
                        rgDectectExtension = JsonConvert.SerializeObject(new { UserFirstName = user.UserFirstName, UserLastName = user.UserLastName }),
                        rgDectectKey = HelperService.GetKeyCode(userId, modeId),
                        modeId = modeId,
                        tagId = tag.TagId,
                        tagCode = tag.TagCode,
                        ticketTypeId = schedule.TicketId,
                        registerDettectDetailRequests = new List<object>
                        {
                            new
                            {
                                rgDectectDetailDateBegin = schedule.ScheduleDateStart.ToString("yyyy-MM-dd HH:mm:ss"),
                                rgDectectDetailDateEnd = schedule.ScheduleDateEnd.ToString("yyyy-MM-dd HH:mm:ss"),
                                rgDectectDetailTimeBegin = schedule.ScheduleTimeStart.ToString(),
                                rgDectectDetailTimeEnd = schedule.ScheduleTimeEnd.ToString(),
                                rgDectectDetailRepeat = schedule.ScheduleRepeatType,
                                rgDectectDetailRepeatValueData = schedule.ScheduleValue is null ? new List<string>() : JsonConvert.DeserializeObject(schedule.ScheduleValue)

                            }
                        }
                    };
                    requestDatasKafka.Add(requestDataKafka);
                }

            }
            object requestRegisterScheduleKafka = new
            {
                RegisterUserDetects = requestDatasKafka
            };

            #region REGISTER_USER_REQUEST_MULTIPLES

            if (schedules.Count > 0)
            {
                Message<string, string> message = new Message<string, string> { Key = DateTime.UtcNow.Ticks.ToString(), Value = JsonConvert.SerializeObject(requestRegisterScheduleKafka) };
                this.Producer.Produce("REGISTER_USER_MUTIL_REQUEST", message, deliveryReportHandleString);
            }
            return ("Success", "Success");
            #endregion
        }



        public (object data, string message) RegisterMeetingWithTagMode(RegisterMeetingWithTagMode request)
        {
            User user = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(request.UserId));
            if (user is null)
            {
                return (null, "UserNotFound");
            }

            List<object> requestDatasKafka = new List<object>();

            string key = DateTime.UtcNow.Ticks.ToString();
            RegisterUserResponseTasks.HandleTasks.Add(key, false);

            foreach (var tagMode in request.TagModes)
            {
                Tag tag = RepositoryWrapperMariaDB.Tags.FirstOrDefault(x =>
                   x.TagId.Equals(tagMode.TagId)
                && !string.IsNullOrEmpty(x.TicketTypeId)
                && x.TagDateStart.HasValue
                && x.TagDateEnd.HasValue
                && x.TagTimeStart.HasValue
                && x.TagTimeEnd.HasValue
                && x.TagRepeat.HasValue
                );
                if (tag is null)
                {
                    continue;
                }

                object requestDataKafka = new
                {
                    rgDectectUserId = request.UserId,
                    rgDectectExtension = JsonConvert.SerializeObject(new { UserFirstName = user.UserFirstName, UserLastName = user.UserLastName }),
                    rgDectectKey = HelperService.GetKeyCode(request.UserId, tagMode.ModeId),
                    modeId = tagMode.ModeId,
                    tagId = tagMode.TagId,
                    tagCode = tag.TagCode,
                    ticketTypeId = tag.TicketTypeId,
                    registerDettectDetailRequests = new List<object>
                    {
                        new
                        {
                            rgDectectDetailDateBegin = tag.TagDateStart.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            rgDectectDetailDateEnd = tag.TagDateEnd.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            rgDectectDetailTimeBegin = tag.TagTimeStart.Value,
                            rgDectectDetailTimeEnd = tag.TagTimeEnd.Value,
                            rgDectectDetailRepeat = tag.TagRepeat.Value,
                            rgDectectDetailRepeatValueData = !string.IsNullOrEmpty(tag.TagRepeatString) ? JsonConvert.DeserializeObject<List<string>>(tag.TagRepeatString) : null,
                        }
                    }
                };
                requestDatasKafka.Add(requestDataKafka);
            }
            object requestRegisterScheduleKafka = new
            {
                RegisterUserDetects = requestDatasKafka
            };

            #region REGISTER_USER_REQUEST_MULTIPLES

            if (request.TagModes.Count > 0)
            {
                Message<string, string> message = new Message<string, string> { Key = key, Value = JsonConvert.SerializeObject(requestRegisterScheduleKafka) };
                this.Producer.Produce("REGISTER_USER_MUTIL_REQUEST", message, deliveryReportHandleString);

                //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                //cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(TimeOut));


                //while (!RegisterUserResponseTasks.Get(key) && !cancellationTokenSource.IsCancellationRequested)
                //{
                //    Console.WriteLine("Register User Request Task Running");
                //    Thread.Sleep(WaitingKafka);
                //}

                //var data = RegisterUserResponseTasks.GetData(key);
                //RegisterUserResponseTasks.Remove(key);

                //if (cancellationTokenSource.IsCancellationRequested)
                //{
                //    return (null, "TimeOut");
                //}

                //if (!data.IsSuccess)
                //{
                //    return (null, data.Message);
                //}

                return ("Success", "Success");
            }

            #endregion
            return ("Success", "Success");

        }
        public (object data, string message) RegisterMeetingWithTagModeImport(RegisterMeetingWithTagMode request)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var repositoryNew = scope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

                User user = repositoryNew.Users.FirstOrDefault(x => x.UserId.Equals(request.UserId));
                if (user is null)
                {
                    return (null, "UserNotFound");
                }

                List<object> requestDatasKafka = new List<object>();

                string key = DateTime.UtcNow.Ticks.ToString();
                RegisterUserResponseTasks.HandleTasks.Add(key, false);

                foreach (var tagMode in request.TagModes)
                {
                    Tag tag = repositoryNew.Tags.FirstOrDefault(x =>
                       x.TagId.Equals(tagMode.TagId)
                    && !string.IsNullOrEmpty(x.TicketTypeId)
                    && x.TagDateStart.HasValue
                    && x.TagDateEnd.HasValue
                    && x.TagTimeStart.HasValue
                    && x.TagTimeEnd.HasValue
                    );
                    if (tag is null)
                    {
                        continue;
                    }


                    UserMode userMode = repositoryNew.UserModes.FirstOrDefault(x => x.ModeId.Equals(tagMode.ModeId) && x.UserId.Equals(request.UserId));
                    object requestDataKafka = new
                    {
                        rgDectectUserId = request.UserId,
                        rgDectectExtension = JsonConvert.SerializeObject(new { UserFirstName = user.UserFirstName, UserLastName = user.UserLastName }),
                        rgDectectKey = userMode == null ? "" : userMode.UserModeKeyCode,
                        modeId = tagMode.ModeId,
                        tagId = tagMode.TagId,
                        tagCode = tag.TagCode,
                        ticketTypeId = tag.TicketTypeId,
                        registerDettectDetailRequests = new List<object>
                    {
                        new
                        {
                            rgDectectDetailDateBegin = tag.TagDateStart.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            rgDectectDetailDateEnd = tag.TagDateEnd.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                            rgDectectDetailTimeBegin = tag.TagTimeStart.Value,
                            rgDectectDetailTimeEnd = tag.TagTimeEnd.Value,
                            rgDectectDetailRepeat = tag.TagRepeat.Value,
                            rgDectectDetailRepeatValueData = !string.IsNullOrEmpty(tag.TagRepeatString) ? JsonConvert.DeserializeObject<List<string>>(tag.TagRepeatString) : null,
                        }
                    }
                    };
                    requestDatasKafka.Add(requestDataKafka);
                }
                object requestRegisterScheduleKafka = new
                {
                    RegisterUserDetects = requestDatasKafka
                };

                #region REGISTER_USER_REQUEST_MULTIPLES

                if (request.TagModes.Count > 0)
                {
                    Message<string, string> message = new Message<string, string> { Key = key, Value = JsonConvert.SerializeObject(requestRegisterScheduleKafka) };
                    this.Producer.Produce("REGISTER_USER_MUTIL_REQUEST", message, deliveryReportHandleString);



                    //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                    //cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(TimeOut));


                    //while (!RegisterUserResponseTasks.Get(key) && !cancellationTokenSource.IsCancellationRequested)
                    //{
                    //    Console.WriteLine("Register User Request Task Running");
                    //    Thread.Sleep(WaitingKafka);
                    //}

                    //var data = RegisterUserResponseTasks.GetData(key);
                    //RegisterUserResponseTasks.Remove(key);

                    //if (cancellationTokenSource.IsCancellationRequested)
                    //{
                    //    return (null, "TimeOut");
                    //}

                    //if (!data.IsSuccess)
                    //{
                    //    return (null, data.Message);
                    //}
                }

                #endregion
                return ("Success", "Success");
            }
        }

        public (object data, string message) DeleteMeetingWithTagMode(RegisterMeetingWithTagMode request)
        {
            User user = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(request.UserId));
            if (user is null)
            {
                return (null, "UserNotFound");
            }

            List<object> requestDatasKafka = new List<object>();

            string key = DateTime.UtcNow.Ticks.ToString();
            RegisterUserResponseTasks.HandleTasks.Add(key, false);

            foreach (var tagMode in request.TagModes)
            {
                Tag tag = RepositoryWrapperMariaDB.Tags.FirstOrDefault(x =>
                   x.TagId.Equals(tagMode.TagId)
                && !string.IsNullOrEmpty(x.TicketTypeId)
                && x.TagDateStart.HasValue
                && x.TagDateEnd.HasValue
                && x.TagTimeStart.HasValue
                && x.TagTimeEnd.HasValue
                );
                if (tag is null)
                {
                    continue;
                }

                object requestDataKafka = new
                {
                    rgDectectUserId = request.UserId,
                    rgDectectExtension = JsonConvert.SerializeObject(new { UserFirstName = user.UserFirstName, UserLastName = user.UserLastName }),
                    rgDectectKey = HelperService.GetKeyCode(request.UserId, tagMode.ModeId),
                    modeId = tagMode.ModeId,
                    tagId = tagMode.TagId,
                    tagCode = tag.TagCode,
                    ticketTypeId = tag.TicketTypeId,
                    rgDectectDetailDateBegin = tag.TagDateStart.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    rgDectectDetailDateEnd = tag.TagDateEnd.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    rgDectectDetailTimeBegin = tag.TagTimeStart.Value,
                    rgDectectDetailTimeEnd = tag.TagTimeEnd.Value,
                    rgDectectDetailRepeat = tag.TagRepeat.Value,
                    rgDectectDetailRepeatValueData = !string.IsNullOrEmpty(tag.TagRepeatString) ? JsonConvert.DeserializeObject<List<string>>(tag.TagRepeatString) : null,
                };
                requestDatasKafka.Add(requestDataKafka);
            }

            object requestDeleteScheduleKafka = new
            {
                UnRegisterUserDetects = requestDatasKafka
            };

            #region UN_REGISTER_USER_REQUEST_MULTIPLES

            Message<string, string> message = new Message<string, string> { Key = key, Value = JsonConvert.SerializeObject(requestDeleteScheduleKafka) };
            this.Producer.Produce("UN_REGISTER_USER_MUTIL_REQUEST", message, deliveryReportHandleString);


            //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            //cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(TimeOut));


            //while (!RegisterUserResponseTasks.Get(key) && !cancellationTokenSource.IsCancellationRequested)
            //{
            //    Console.WriteLine("Register User Request Task Running");
            //    Thread.Sleep(WaitingKafka);
            //}

            //var data = RegisterUserResponseTasks.GetData(key);
            //RegisterUserResponseTasks.Remove(key);

            //if (cancellationTokenSource.IsCancellationRequested)
            //{
            //    return (null, "TimeOut");
            //}

            //if (!data.IsSuccess)
            //{
            //    return (null, data.Message);
            //}

            #endregion
            return ("Success", "Success");
        }

        #endregion

        public (object data, string message) DeleteRegisterDetect(string userId)
        {
            string key = DateTime.UtcNow.Ticks.ToString();
            Message<string, string> message = new Message<string, string> { Key = key, Value = userId };
            this.Producer.Produce("UN_REGISTER_USER_WITH_USERID_REQUEST", message);
            return ("Success", "Success");
        }

        public (object data, string message) DeleteMeetingScheduleByUser(string userId)
        {
            //User user = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            //if (user is null)
            //{
            //    return (null, "UserNotFound");
            //}
            IDbContextTransaction transaction = RepositoryWrapperMariaDB.BeginTransaction();
            try
            {
                List<Schedule> schedules = RepositoryWrapperMariaDB.Schedules.FindByCondition(x => x.UserId.Equals(userId)).ToList();
                RepositoryWrapperMariaDB.Schedules.RemoveRange(schedules);

                string key = DateTime.UtcNow.Ticks.ToString();
                //RegisterUserResponseTasks.HandleTasks.Add(key, false);

                Message<string, string> message = new Message<string, string> { Key = key, Value = userId };
                this.Producer.Produce("UN_REGISTER_USER_WITH_USERID_REQUEST", message, deliveryReportHandleString);
                //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                //cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(TimeOut));


                //while (!RegisterUserResponseTasks.Get(key) && !cancellationTokenSource.IsCancellationRequested)
                //{
                //    Console.WriteLine("Un-Register UserId Request Task Running");
                //    Thread.Sleep(WaitingKafka);
                //}

                //var data = RegisterUserResponseTasks.GetData(key);
                //RegisterUserResponseTasks.Remove(key);

                //if (cancellationTokenSource.IsCancellationRequested)
                //{
                //    transaction.Rollback();
                //    return (null, "TimeOut");
                //}

                //if (!data.IsSuccess)
                //{
                //    transaction.Rollback();
                //    return (null, data.Message);
                //}

                RepositoryWrapperMariaDB.SaveChanges();
                transaction.Commit();
                return ("Success", "Success");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return (null, ex.Message);
            }

        }

        public void deliveryReportHandleString(DeliveryReport<string, string> deliveryReport)
        {
            if (deliveryReport.Status == PersistenceStatus.NotPersisted)
            {
                Console.WriteLine($"Message delivery failed: {deliveryReport.Message.Value}");
            }
        }

        private (List<User> users, string message) ReadExcel(FileInfo fileInfo, Dictionary<string, string> emailDB, Dictionary<string, string> codeDB, string tagId, string groupId = null)
        {
            // mở file excel
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var package = new ExcelPackage(fileInfo);

            // lấy ra sheet đầu tiên để thao tác
            ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
            List<string> codeExist = codeDB.Keys.ToList();

            List<User> employees = new List<User>();

            // duyệt tuần tự từ dòng thứ 2 đến dòng cuối cùng của file. lưu ý file excel bắt đầu từ số 1 không phải số 0
            for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
            {
                try
                {
                    string UserFirstName = workSheet.Cells[i, 1].Value?.ToString();
                    string UserLastName = workSheet.Cells[i, 2].Value?.ToString();
                    string UserGender = workSheet.Cells[i, 3].Value?.ToString();
                    string UserEmail = workSheet.Cells[i, 4].Value?.ToString();
                    string UserPhone = workSheet.Cells[i, 5].Value?.ToString();


                    if (String.IsNullOrEmpty(UserEmail))
                    {
                        continue;
                    }
                    string code = GeneralHelper.RandomCode(6, codeExist);
                    codeExist.Add(code);
                    codeDB.Add(code, code);
                    User newEmployee = new User()
                    {
                        UserFirstName = UserFirstName,
                        UserLastName = UserLastName,
                        UserCode = code,
                        UserPhone = UserPhone,
                        UserEmail = UserEmail,
                        UserGender = !string.IsNullOrEmpty(UserGender) ? UserGender.ParseInt() : 0,
                        UserTagIds = JsonConvert.SerializeObject(new List<string> { tagId }),
                        GroupId = groupId != null ? groupId : "group_default",
                        IsImportSuccess = true,
                    };

                    //validate Email

                    if (emailDB.ContainsKey(newEmployee.UserEmail))
                    {
                        newEmployee.UserId = "";
                        newEmployee.UserOld = true;
                        //newEmployee.IsImportSuccess = false;
                        //newEmployee.ErrorImport.Add("UserEmailDuplicate");
                        employees.Add(newEmployee);
                    }
                    else
                    {
                        emailDB.Add(UserEmail, UserEmail);
                        employees.Add(newEmployee);
                    }

                }
                catch (Exception exe)
                {
                    return (null, exe.Message);
                }
            }
            return (employees, "success");
        }


    }
}
