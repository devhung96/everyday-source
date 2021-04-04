using Project.App.Helpers;
using System;

namespace Project.Modules.ClassSchedules.Requests
{
    public class ShowClassScheduleRequest : RequestTable
    {
        public string Class { get; set; }
    }
}
