using Microsoft.Extensions.Configuration;
using Project.App.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Project.Modules.Organizes.Entities;
using Project.Modules.Events.Entities;

namespace Project.Modules.Reports.Services
{

    public interface IEndMeetingReportService
    {
        (ReportEndMeeting, string) ReportEndMeeting(string eventId);
    }
    public class EndMeetingReportService : IEndMeetingReportService
    {
        private readonly MariaDBContext dBContext;
        private readonly IConfiguration _config;
        private readonly IReportService reportService;


        public EndMeetingReportService(MariaDBContext mariaDBContext, IConfiguration config, IReportService ReportService)
        {
            dBContext = mariaDBContext;
            _config = config;
            reportService = ReportService;
        }

        public (ReportEndMeeting, string) ReportEndMeeting(string eventId)
        {
            Event events = dBContext.Events.Where(x => x.EventId.Equals(eventId)).FirstOrDefault();

            if(events is null)
            {
                return (null, "EventNotFound");
            }

            int eventUser = dBContext.EventUsers.Count(x => x.EventId.Equals(events.EventId) && x.UserLoginStatus.Equals(USER_LOGIN_STATUS.ON));
            List<SessionReport> sessions = dBContext.Sessions.Where(x => x.EventId.Equals(eventId)).OrderBy(x => x.SessionSort)
                .Select(x => new SessionReport
                {
                    Description = x.SessionDescription,
                    Title = x.SessionTitle,
                    SessionId = x.SessionId
                })
                .ToList();

            foreach (var item in sessions)
            {
                item.Question = reportService.SupportGetChartWithSession(item.SessionId);
            }

            SupportExportReport supportExportReport = reportService.SupportGetInfo(eventId);
            ReportEndMeeting reportEndMeeting = new ReportEndMeeting
            {
                TimeBegin = events.EventTimeBegin,
                TimeEnd = events.EventTimeEnd,
                CountUser = eventUser,
                Session = sessions,
                SumStock = supportExportReport.shareholderAttending,
                PercentStock = supportExportReport.percent
            };

            return (reportEndMeeting, "Success");
        }
    }

    public class ReportEndMeeting
    {
        public DateTime TimeBegin { get; set; }
        public DateTime TimeEnd { get; set; }
        public int CountUser { get; set; }
        public long SumStock { get; set; }
        public double PercentStock { get; set; }
        public List<SessionReport> Session {get;set;}
    }

    public class SessionReport
    {
        public string SessionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SupportExportReportQuestion> Question { get; set; }
    }
}
