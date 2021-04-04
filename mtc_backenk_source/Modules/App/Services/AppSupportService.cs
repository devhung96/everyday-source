using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.App.Entities;
using Project.Modules.App.Requests;
using Project.Modules.Schedules.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.App.Services
{

    public interface IAppSupportService
    {
        (List<Entities.ScheduleResponse> data, string message) GetScheduleByTime(GetScheduleByTimeRequest request, string deviceId);
    }

    public class AppSupportService : IAppSupportService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        private readonly IServiceScopeFactory _servirceScopeFatory;


        public AppSupportService(IConfiguration configuration, IMapper mapper, IServiceScopeFactory servirceScopeFatory)
        {
            _configuration = configuration;
            _mapper = mapper;


            _servirceScopeFatory = servirceScopeFatory;

        }

        public (List<Entities.ScheduleResponse> data, string message) GetScheduleByTime(GetScheduleByTimeRequest request, string deviceId)
        {
            using IServiceScope serviceScope = _servirceScopeFatory.CreateScope();
            IRepositoryWrapperMariaDB _repositoryWrapperMariaDB = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

            List<string> scheduleIds = _repositoryWrapperMariaDB.ScheduleDevices.FindByCondition(x => x.DeviceId.Equals(deviceId)).Select(x => x.ScheduleId).ToList();
            List<Schedule> schedules = _repositoryWrapperMariaDB.Schedules.FindByCondition(x =>
                                                                                                (!(request.timeBegin < x.ScheduleDateTimeBegin && request.timeEnd < x.ScheduleDateTimeBegin)
                                                                                                && !(request.timeBegin > x.ScheduleDateTimeBegin && request.timeEnd > x.ScheduleDateTimeEnd))
                                                                                                && scheduleIds.Contains(x.ScheduleId)
                                                                                                )
                                                                                            .Include(x => x.PlayList)
                                                                                           .ThenInclude(x => x.PlayListDetail)
                                                                                           .ThenInclude(x => x.Template)
                                                                                           .ThenInclude(x => x.TemplateDetails)
                                                                                           .ThenInclude(x => x.Media)
                                                                                           .ToList();




            #region Bước 1:  Xữ lý lấy lịch theo từng ngày.

            List<ScheduleResponsePrepare> prepareScheduleByDay = new List<ScheduleResponsePrepare>();

            DateTime tmpDate = request.timeBegin.Date;
            while (tmpDate <= request.timeEnd.Date)
            {
                ScheduleResponsePrepare scheduleResponsePrepare = new ScheduleResponsePrepare
                {
                    Key = tmpDate,
                    Schedules = new List<Schedule>(),
                };

                List<Schedule> scheduleWithNonAndDayly = schedules.Where(x => x.ScheduleRepeat == ScheduleRepeatEnum.Daily || x.ScheduleRepeat == ScheduleRepeatEnum.Non).ToList();
                scheduleResponsePrepare.Schedules.AddRange(scheduleWithNonAndDayly);

                foreach (var item in schedules.Where(x => x.ScheduleRepeat != ScheduleRepeatEnum.Daily && x.ScheduleRepeat != ScheduleRepeatEnum.Non).ToList())
                {
                    var isSuccess = item.ScheduleRepeatValueDetailData.Contains(scheduleResponsePrepare.Key.ToString("yyyyy-MM-dd"));
                    if (isSuccess) scheduleResponsePrepare.Schedules.Add(item);
                }
                prepareScheduleByDay.Add(scheduleResponsePrepare);
                tmpDate = tmpDate.AddDays(1);
            }

            #endregion



            List<Entities.ScheduleResponse> allData = new List<Entities.ScheduleResponse>();

            #region Xử lý nội dung cho từng lịch.
            foreach (var item in prepareScheduleByDay)
            {
                


                foreach (var itemSchedule in item.Schedules.OrderBy(x=> x.ScheduleTimeBegin))
                {
                    #region Loai bỏ lịch không đúng ngày repeat 
                    if (itemSchedule.ScheduleRepeat != ScheduleRepeatEnum.Daily && itemSchedule.ScheduleRepeat != ScheduleRepeatEnum.Non)
                    {
                        if (!itemSchedule.ScheduleRepeatValueDetailData.Contains(item.Key.ToString("yyyyy-MM-dd"))) continue;
                    }
                    #endregion

                    Entities.ScheduleResponse scheduleResponse = new Entities.ScheduleResponse
                    {
                        Date = item.Key,
                        TimeBegin = itemSchedule.ScheduleTimeBegin,
                        TimeEnd = itemSchedule.ScheduleTimeEnd,
                        Templates = new List<TemplateResponse>(),
                        PlaylistLoop = PlayLists.Entities.PlayListLoopEnum.NOLOOP,
                        ScheduleIds = new List<string> { itemSchedule.ScheduleId, itemSchedule.ScheduleName },
                        PlayListId = itemSchedule.PlayList.PlayListId,
                        PlayListName = itemSchedule.PlayList.PlayListName,
                        ScheduleSequential = itemSchedule.ScheduleSequential,
                    };
                

                    #region Nếu :  Xử lý lặp tuần tự
                    if (itemSchedule.ScheduleSequential == ScheduleSequential.On)
                    {
                        TimeSpan tmpTimeStart = scheduleResponse.TimeBegin;
                        TimeSpan tmpDuration = new TimeSpan(0, 0, 0);

                        foreach (var itemPlaylist in itemSchedule.PlayList.PlayListDetail)
                        {
                            if (itemPlaylist.TemplateDuration != null) tmpDuration = itemPlaylist.TemplateDuration.Value;

                            TemplateResponse templateResponse = new TemplateResponse
                            {
                                TimeBegin = tmpTimeStart,
                                TimeEnd   = tmpTimeStart.Add(tmpDuration),

                                TempRatioX = itemPlaylist.Template.TemplateRatioX,
                                TempRatioY = itemPlaylist.Template.TemplateRatioY,

                                TemplateDuration = tmpDuration,

                                TemplateId = itemPlaylist.Template.TemplateId,
                                TemplateName = itemPlaylist.Template.TemplateName,

                                Medias = itemPlaylist.Template.TemplateDetails.Select(x =>
                                new Entities.MediaResponse
                                {
                                    MediaId = x.MediaId,
                                    MediaUrl = x.Media.MediaUrl,
                                    MediaMd5 = x.Media.MediaMd5,
                                    MediaTypeCode = x.Media.MediaTypeCode,

                                    TempRatioX = x.TempRatioX,
                                    TempRatioY = x.TempRatioY,
                                    TempPointTop = x.TempRatioY,
                                    TempPointLeft = x.TempRatioX,
                                    TempPointHeight = x.TempPointHeight,
                                    TempPointWidth = x.TempPointWidth,
                                    TempDetailZindex = x.Zindex,
                                    

                                    PlaylistLoop = itemPlaylist.PlayList.PlayListLoop,
                                    ScheduleId = itemSchedule.ScheduleId

                                }).ToList(),
                            };



                            // time end  = itemPlaylist.TimeBegin.Add(tmpDuration)
                            if (itemPlaylist.TimeBegin.Add(tmpDuration) > scheduleResponse.TimeEnd)
                            {
                                templateResponse.TimeEnd = scheduleResponse.TimeEnd;
                            }

                            tmpTimeStart = templateResponse.TimeEnd;

                            // Gắn loop cho thằng cha 
                            scheduleResponse.PlaylistLoop = itemPlaylist.PlayList.PlayListLoop;
                            scheduleResponse.Templates.Add(templateResponse);


                        }

                    }

                    #endregion

                    #region Xử lý phát theo thời gian playlist
                    else
                    {
                        foreach (var itemPlaylist in itemSchedule.PlayList.PlayListDetail)
                        {

                            TemplateResponse templateResponse = new TemplateResponse
                            {
                                TimeBegin = itemPlaylist.TimeBegin,
                                TimeEnd = itemPlaylist.TimeEnd,

                                TempRatioX = itemPlaylist.Template.TemplateRatioX,
                                TempRatioY = itemPlaylist.Template.TemplateRatioY,

                                TemplateDuration = itemPlaylist.TemplateDuration,

                                TemplateId = itemPlaylist.Template.TemplateId,
                                TemplateName = itemPlaylist.Template.TemplateName,

                                Medias = itemPlaylist.Template.TemplateDetails.Select(x =>
                                new Entities.MediaResponse
                                {
                                    MediaId = x.MediaId,
                                    MediaUrl = x.Media.MediaUrl,
                                    MediaMd5 = x.Media.MediaMd5,
                                    MediaTypeCode = x.Media.MediaTypeCode,

                                    TempRatioX = x.TempRatioX,
                                    TempRatioY = x.TempRatioY,
                                    TempPointTop = x.TempRatioY,
                                    TempPointLeft = x.TempRatioX,
                                    TempPointHeight = x.TempPointHeight, 
                                    TempPointWidth = x.TempPointWidth,
                                    TempDetailZindex = x.Zindex,

                                    PlaylistLoop = itemPlaylist.PlayList.PlayListLoop,
                                    ScheduleId = itemSchedule.ScheduleId
                                }).ToList(),
                            };


                            if (itemPlaylist.TimeBegin < scheduleResponse.TimeBegin)
                            {
                                templateResponse.TimeBegin = scheduleResponse.TimeBegin;
                            }

                            if (itemPlaylist.TimeEnd > scheduleResponse.TimeEnd)
                            {
                                templateResponse.TimeEnd = scheduleResponse.TimeEnd;
                            }

                            //Truong hop loi
                            //"timeBegin": "08:00:00",
                            //"timeEnd": "12:00:00",
                            //"playlistLoop": 0,
                            //"templates": [
                            //{
                            //                    "templateId": "8c05572f-657c-4c50-8b6d-0affad2fd7d8",
                            //    "templateName": "[Hung - test] Template device 192.168.10.01",
                            //    "tempRatioX": 1
                            //    "tempRatioY": 1,
                            //    "timeBegin": "08:00:00"
                            //    "timeEnd": "05:00:00"
                            if (templateResponse.TimeEnd > templateResponse.TimeBegin)
                            {
                                scheduleResponse.Templates.Add(templateResponse);
                            }


                            // Gắn loop cho thằng cha 
                            scheduleResponse.PlaylistLoop = itemPlaylist.PlayList.PlayListLoop;
                        }
                    }
                    #endregion

                    scheduleResponse.Templates = scheduleResponse.Templates.OrderBy(x => x.TimeBegin).ToList();
                    allData.Add(scheduleResponse);
                }

            }
            #endregion

            return (allData, "GetScheduleSuccess");
        }

    }
}
