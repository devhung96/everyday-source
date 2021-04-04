using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Devices.Entities;
using Project.Modules.Schedules.Entities;
using Project.Modules.Schedules.Models;
using Project.Modules.Schedules.Requests;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Project.Modules.Schedules.Services
{
    public interface IScheduleService
    {
        (Schedule data, string message) StoreScheduleDevice(StoreScheduleRequest request);
        (Schedule data, string message) Store(StoreScheduleNonDeviceRequest request);
        (Schedule data, string message) UpdateOld(string idSchedule, Schedule schedule);
        (bool result, string message) Destroy(string idSchedule);
        (ResponseSchedule chedule, string message) Show(string idSchedule);
        (PaginationResponse<Schedule> schedules, string message) ShowAll(PaginationRequest request, CheckUser checkUser);
        (Schedule data, string message) Update(UpdateScheduleRequest request);
        (PaginationResponse<Calendar> data, string message) GetCalendarForMonth(CalendarRequest request, CheckUser checkUser);
        (GetScheduleByDeviceResponse data, string message) GetScheduleByDevice(GetScheduleByDevice request);
        (object data, string message) Delete(List<string> scheduleIds);
        (object data, string message) ShowDevice(string scheduleId);
    }
    public class ScheduleService : IScheduleService
    {
        private readonly IMapper Mapper;
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDb;
        public ScheduleService(IRepositoryWrapperMariaDB repositoryWrapperMariaDB, IMapper mapper)
        {
            RepositoryWrapperMariaDb = repositoryWrapperMariaDB;
            Mapper = mapper;
        }

        public (Schedule data, string message) Store(StoreScheduleNonDeviceRequest request)
        {
            request.ScheduleRepeatValue = (request.ScheduleRepeatValues != null && request.ScheduleRepeatValues.Count > 0) ? JsonConvert.SerializeObject(request.ScheduleRepeatValues) : null;
            request.ScheduleDateTimeBegin = request.ScheduleDateTimeBeginStr.ParseStringGMTToDatime();
            request.ScheduleDateTimeEnd = request.ScheduleDateTimeEndStr.ParseStringGMTToDatime();
            request.ScheduleTimeBegin = TimeSpan.Parse(request.ScheduleTimeBeginStr);
            request.ScheduleTimeEnd = TimeSpan.Parse(request.ScheduleTimeEndStr);
            Schedule schedule = Mapper.Map<Schedule>(request);
            RepositoryWrapperMariaDb.Schedules.Add(schedule);
            RepositoryWrapperMariaDb.SaveChanges();
            return (schedule, "CreateScheduleSuccess");
        }

        public (Schedule data, string message) StoreScheduleDevice(StoreScheduleRequest request)
        {
            request.ScheduleRepeatValue = (request.ScheduleRepeatValues != null && request.ScheduleRepeatValues.Count > 0) ? JsonConvert.SerializeObject(request.ScheduleRepeatValues) : null;
            request.ScheduleRepeatValueDetail = (request.ScheduleRepeatValueDetails != null && request.ScheduleRepeatValueDetails.Count > 0) ? JsonConvert.SerializeObject(request.ScheduleRepeatValueDetails) : null;
            request.ScheduleDateTimeBegin = request.ScheduleDateTimeBeginStr.ParseStringGMTToDatime();
            request.ScheduleDateTimeEnd = request.ScheduleDateTimeEndStr.ParseStringGMTToDatime();
            Schedule schedule = Mapper.Map<Schedule>(request);
            RepositoryWrapperMariaDb.Schedules.Add(schedule);

            List<ScheduleDevice> scheduleDevices = new List<ScheduleDevice>();

            List<ScheduleDevice> deleteScheduleDevice = RepositoryWrapperMariaDb.ScheduleDevices
                .FindByCondition(x => request.DeviceIds.Contains(x.DeviceId))
                .ToList();
            RepositoryWrapperMariaDb.ScheduleDevices.RemoveRange(deleteScheduleDevice);

            foreach (var deviceId in request.DeviceIds)
            {
                ScheduleDevice scheduleDevice = new ScheduleDevice
                {
                    DeviceId = deviceId,
                    ScheduleId = schedule.ScheduleId,
                };
                scheduleDevices.Add(scheduleDevice);
            }

            RepositoryWrapperMariaDb.SaveChanges();
            return (schedule, "CreateScheduleDeviceSuccess");
        }

        public (Schedule data, string message) Update(UpdateScheduleRequest request)
        {
            Schedule schedule = RepositoryWrapperMariaDb.Schedules.FirstOrDefault(x => x.ScheduleId.Equals(request.ScheduleId));

            if (schedule is null)
            {
                return (null, "ScheduleNotFound");
            }

            List<ScheduleDevice> deleteScheduleDevice = RepositoryWrapperMariaDb.ScheduleDevices
                .FindByCondition(x => x.ScheduleId.Equals(request.ScheduleId))
                .ToList();
            RepositoryWrapperMariaDb.ScheduleDevices.RemoveRange(deleteScheduleDevice);

            if (request.DeviceIds.Count > 0)
            {
                List<ScheduleDevice> scheduleDevices = new List<ScheduleDevice>();
                foreach (var deviceId in request.DeviceIds)
                {
                    ScheduleDevice scheduleDevice = new ScheduleDevice
                    {
                        DeviceId = deviceId,
                        ScheduleId = request.ScheduleId,
                    };
                    scheduleDevices.Add(scheduleDevice);
                }
                RepositoryWrapperMariaDb.ScheduleDevices.AddRange(scheduleDevices);
            }

            request.ScheduleRepeatValue = (request.ScheduleRepeatValues != null && request.ScheduleRepeatValues.Count > 0) ? JsonConvert.SerializeObject(request.ScheduleRepeatValues) : null;
            request.ScheduleRepeatValueDetail = (request.ScheduleRepeatValueDetails != null && request.ScheduleRepeatValueDetails.Count > 0) ? JsonConvert.SerializeObject(request.ScheduleRepeatValueDetails) : null;
            request.ScheduleDateTimeBegin = request.ScheduleDateTimeBeginStr.ParseStringGMTToDatime();
            request.ScheduleDateTimeEnd = request.ScheduleDateTimeEndStr.ParseStringGMTToDatime();
            request.ScheduleTimeBegin = TimeSpan.Parse(request.ScheduleTimeBeginStr);
            request.ScheduleTimeEnd = TimeSpan.Parse(request.ScheduleTimeEndStr);
            schedule = Mapper.Map<UpdateScheduleRequest, Schedule>(request, schedule);
            RepositoryWrapperMariaDb.SaveChanges();
            return (schedule, "UpdateScheduleSuccess");
        }

        public (PaginationResponse<Calendar> data, string message) GetCalendarForMonth(CalendarRequest request, CheckUser checkUser)
        {
            DateTime from = request.DateFrom.ParseStringGMTToDatime();
            DateTime to = request.DateTo.ParseStringGMTToDatime();

            var schedules = RepositoryWrapperMariaDb.Schedules
                .FindByCondition(x => (x.ScheduleDateTimeBegin.Date >= from.Date && x.ScheduleDateTimeBegin.Date <= to.Date) || (x.ScheduleDateTimeEnd.Date >= from.Date && x.ScheduleDateTimeEnd.Date <= to.Date))
                .Include(x => x.User)
                .Include(x => x.PlayList)
                .AsQueryable();

            DEVICESTATUS deviceStatus = DEVICESTATUS.ACTIVED;

            switch (request.DeviceStatus)
            {
                case 1:
                    {
                        deviceStatus = DEVICESTATUS.ACTIVED;
                        break;
                    }

                case 2:
                    {
                        deviceStatus = DEVICESTATUS.DEACTIVATED;
                        break;
                    }
                default: break;
            }

            List<string> deviceIds = RepositoryWrapperMariaDb.Devices
                .FindByCondition(x =>
                (string.IsNullOrEmpty(checkUser.GroupId) || x.GroupId.Equals(checkUser.GroupId))
                &&
                (request.DeviceStatus == 0 || x.DeviceStatus == deviceStatus)
                && (string.IsNullOrEmpty(request.SearchContent)
                || x.DeviceSku.ToLower().Contains(request.SearchContent.ToLower()) || x.DeviceName.ToLower().Contains(request.SearchContent.ToLower())))
                .Select(x => x.DeviceId)
                .ToList();

            var deviceSchedules = RepositoryWrapperMariaDb.ScheduleDevices
                .FindByCondition(x => deviceIds.Contains(x.DeviceId))
                .ToList()
                .GroupBy(x => x.DeviceId)
                .Select(x => new { DeviceId = x.Key, ScheduleIds = x.Select(y => y.ScheduleId).ToList() })
                .ToList();

            schedules = SortHelper<Schedule>.ApplySort(schedules, request.OrderByQuery);

            List<Calendar> calendars = new List<Calendar>();

            foreach (var deviceSchedule in deviceSchedules)
            {
                Calendar calendar = new Calendar();
                List<ScheduleItem> scheduleItems = new List<ScheduleItem>();

                foreach (var scheduleId in deviceSchedule.ScheduleIds)
                {
                    if (schedules.Count() > 0)
                    {
                        Schedule schedule = schedules.FirstOrDefault(x => x.ScheduleId.Equals(scheduleId));

                        if (schedule != null)
                        {
                            scheduleItems.Add(new ScheduleItem
                            {
                                ScheduleId = schedule.ScheduleId,
                                User = schedule.User,
                                PlayList = schedule.PlayList,
                                ScheduleRepeat = schedule.ScheduleRepeat,
                                ScheduleDateTimeBegin = schedule.ScheduleDateTimeBegin,
                                ScheduleDateTimeEnd = schedule.ScheduleDateTimeEnd,
                                ScheduleTimeBegin = schedule.ScheduleTimeBegin.ToString(),
                                ScheduleTimeEnd = schedule.ScheduleTimeEnd.ToString(),
                                ScheduleRepeatValues = schedule.ScheduleRepeatValueData,
                                ScheduleRepeatValueDetails = schedule.ScheduleRepeatValueDetailData,
                                ScheduleLoop = schedule.ScheduleLoop,
                                ScheduleCreatedAt = schedule.ScheduleCreatedAt,
                            });
                        }

                    }
                    calendar.Device = RepositoryWrapperMariaDb.Devices.FirstOrDefault(x => x.DeviceId.Equals(deviceSchedule.DeviceId));
                    calendar.ScheduleItems = scheduleItems;
                }

                calendars.Add(calendar);
            }

            PaginationHelper<Calendar> result = PaginationHelper<Calendar>.ToPagedList(calendars.AsQueryable(), request.PageNumber, request.PageSize);
            PaginationResponse<Calendar> response = new PaginationResponse<Calendar>(result, result.PageInfo);

            return (response, "Success");
        }

        public (bool result, string message) Destroy(string idSchedule)
        {
            Schedule checkSchedule = RepositoryWrapperMariaDb.Schedules
                .FindAll()
                .Include(x => x.User)
                .FirstOrDefault();

            if (checkSchedule is null)
            {
                return (false, "ScheduleNotFound");
            }
            RepositoryWrapperMariaDb.Schedules.Remove(checkSchedule);
            RepositoryWrapperMariaDb.SaveChanges();
            return (true, "DeleteScheduleSuccess");
        }

        public (ResponseSchedule chedule, string message) Show(string idSchedule)
        {
            Schedule schedule = RepositoryWrapperMariaDb.Schedules
                .FindByCondition(x => x.ScheduleId.Equals(idSchedule))
                .Include(x => x.User)
                .Include(x => x.PlayList)
                .FirstOrDefault();

            if (schedule is null)
            {
                return (null, "ScheduleNotFound");
            }

            ResponseSchedule scheduleResponse = new ResponseSchedule
            {
                ScheduleId = schedule.ScheduleId,
                ScheduleName = schedule.ScheduleName,
                PlaylistId = schedule.PlaylistId,
                UserId = schedule.UserId,
                ScheduleDateTimeBegin = schedule.ScheduleDateTimeBegin,
                ScheduleDateTimeEnd = schedule.ScheduleDateTimeEnd,
                ScheduleTimeBegin = schedule.ScheduleTimeBegin,
                ScheduleTimeEnd = schedule.ScheduleTimeEnd,
                ScheduleLoop = schedule.ScheduleLoop,
                ScheduleRepeat = schedule.ScheduleRepeat,
                ScheduleRepeatValueData = schedule.ScheduleRepeatValueData,
                ScheduleRepeatValueDetailData = schedule.ScheduleRepeatValueDetailData,
                ScheduleSequential = schedule.ScheduleSequential,
                ScheduleCreatedAt = schedule.ScheduleCreatedAt,
                User = schedule.User,
                PlayList = schedule.PlayList,
                DeviceIds = RepositoryWrapperMariaDb.ScheduleDevices.FindByCondition(x => x.ScheduleId.Equals(schedule.ScheduleId)).Select(x => x.DeviceId).ToList()
            };

            return (scheduleResponse, "Success");
        }


        public (PaginationResponse<Schedule> schedules, string message) ShowAll(PaginationRequest request, CheckUser checkUser)
        {
            List<string> userIds = RepositoryWrapperMariaDb.Users.FindByCondition(x => x.GroupId.Equals(checkUser.GroupId)).Select(x => x.UserId).ToList();

            var schedules = RepositoryWrapperMariaDb.Schedules
                .FindByCondition(x => userIds.Contains(x.UserId))
                .Include(x => x.User)
                .Include(x => x.PlayList)
                .AsQueryable();

            schedules = SortHelper<Schedule>.ApplySort(schedules, request.OrderByQuery);
            PaginationHelper<Schedule> result = PaginationHelper<Schedule>.ToPagedList(schedules, request.PageNumber, request.PageSize);
            PaginationResponse<Schedule> response = new PaginationResponse<Schedule>(result, result.PageInfo);
            return (response, "ShowAllScheduleSuccess");
        }

        public (Schedule data, string message) UpdateOld(string idSchedule, Schedule inSchedule)
        {
            Schedule checkSchedule = RepositoryWrapperMariaDb.Schedules
                .FindAll()
                .Include(x => x.User)
                .FirstOrDefault();

            if (checkSchedule == null)
            {
                return (null, $"Schedule with id : {idSchedule} not found");
            }

            Mapper.Map(inSchedule, checkSchedule);
            RepositoryWrapperMariaDb.SaveChanges();
            return (checkSchedule, "Updated success");

        }

        public (GetScheduleByDeviceResponse data, string message) GetScheduleByDevice(GetScheduleByDevice request)
        {
            List<string> scheduleIds = RepositoryWrapperMariaDb.ScheduleDevices
                .FindByCondition(x => x.DeviceId.Equals(request.DeviceId))
                .Select(x => x.ScheduleId)
                .ToList();

            DateTime from = request.ScheduleDateBegin.ParseStringGMTToDatime();
            DateTime to = request.ScheduleDateEnd.ParseStringGMTToDatime();

            List<Schedule> schedules = RepositoryWrapperMariaDb.Schedules
                .FindByCondition(x => scheduleIds.Contains(x.ScheduleId) && ((x.ScheduleDateTimeBegin.Date >= from.Date && x.ScheduleDateTimeBegin.Date <= to.Date) || (x.ScheduleDateTimeEnd.Date >= from.Date && x.ScheduleDateTimeEnd.Date <= to.Date)))
                .Include(x => x.User)
                .Include(x => x.PlayList)
                .ToList();

            Device device = RepositoryWrapperMariaDb.Devices.FirstOrDefault(x => x.DeviceId.Equals(request.DeviceId));

            GetScheduleByDeviceResponse result = new GetScheduleByDeviceResponse()
            {
                Device = device,
                Schedules = schedules
            };

            return (result, "Success");
        }

        public (object data, string message) Delete(List<string> scheduleIds)
        {
            List<Schedule> schedules = RepositoryWrapperMariaDb.Schedules.FindByCondition(x => scheduleIds.Contains(x.ScheduleId)).ToList();

            RepositoryWrapperMariaDb.Schedules.RemoveRange(schedules);
            RepositoryWrapperMariaDb.SaveChanges();

            return ("Success", "DeleteScheduleSuccess");
        }

        public (object data, string message) ShowDevice(string scheduleId)
        {
            List<string> deviceIds = RepositoryWrapperMariaDb.ScheduleDevices
                .FindByCondition(x => x.ScheduleId.Equals(scheduleId))
                .Select(x => x.DeviceId)
                .ToList();

            // Bỏ này vì Lịch không có device thì trả về [] để frontend biết.

            //if(deviceIds.Count == 0)
            //{
            //    return (null, "ScheduleIdNotFound");
            //}

            List<Device> devices = RepositoryWrapperMariaDb.Devices.FindByCondition(x => deviceIds.Contains(x.DeviceId)).ToList();

            //if(devices.Count == 0)
            //{
            //    return (null, "DeviceNotFound");
            //}

            return (devices, "Success");
        }
    }

    public class CheckUser
    {
        public int Level { get; set; }
        public string GroupId { get; set; }
    }
}
