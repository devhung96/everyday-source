using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Holidays.Entities;
using Project.Modules.Holidays.Requests;
using Project.Modules.Holidays.Services;
using System.Linq.Dynamic.Core;
using Project.Modules.SchoolYears.Extension;
using System.Text;

namespace Project.Modules.Holidays.Controllers
{
    [Route("api/holiday")]
    [ApiController]
    public class HolidayController : BaseController
    {
        public readonly IHolidayServices HolidayServices;
        public readonly IConfiguration Configuration;
        public HolidayController(IConfiguration configuration, IHolidayServices holidayServices)
        {
            HolidayServices = holidayServices;
            Configuration = configuration;
        }

        [HttpPost("get")]
        public IActionResult GetAll([FromBody] RequestTable requestTable)
        {
            List<Holiday> holidays = HolidayServices.GetAll();
            #region Request Table
            holidays = holidays.Where(m => String.IsNullOrEmpty(requestTable.Search) ||
                                                    (m.Name != null && m.Name.RemoveUnicode().ToLower().Contains(requestTable.Search.RemoveUnicode().ToLower()))
                                                                                       ||
                                                    (m.Reason != null && m.Reason.RemoveUnicode().ToLower().Contains(requestTable.Search.RemoveUnicode().ToLower())))
                                           .ToList();
            if (!String.IsNullOrEmpty(requestTable.SortField) || !String.IsNullOrEmpty(requestTable.SortOrder))
            {
                var query = requestTable.SortField + " " + requestTable.SortOrder;
                if (!holidays.FieldExists<Holiday>(requestTable.SortField))
                {
                    return ResponseBadRequest("FieldNameError");
                }
                holidays = holidays.AsQueryable().OrderBy(query).ToList();
            }
            //else
            //{
            //    var query = "CreatedAt desc";
            //    semesters = semesters.AsQueryable().OrderBy(query).ToList();
            //}


            ResponseTable response = new ResponseTable
            {
                Data = holidays,
                Info = new Info
                {
                    Page = requestTable.Page,
                    Limit = requestTable.Page == -1 ? holidays.Count() : requestTable.Limit,
                    TotalRecord = holidays.Count(),
                }
            };
            #endregion
            return Ok(response);
        }

        [HttpGet("{holidayId}")]
        public IActionResult GetById(string holidayId)
        {
            Holiday holiday = HolidayServices.GetById(holidayId);
            if (holiday is null)
            {
                return ResponseBadRequest("HolidayNotFound");
            }
            return ResponseOk(holiday, "GetSuccess");
        }

        [HttpPost]
        public IActionResult Insert([FromBody] InsertHolidayRequest insertHoliday)
        {
            Holiday holiday = new Holiday
            {
                Name = insertHoliday.HolidayName,
                Reason = insertHoliday.Reason,
                TimeStart = DateTime.ParseExact(insertHoliday.TimeStart, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                TimeEnd = DateTime.ParseExact(insertHoliday.TimeEnd, "dd-MM-yyyy", CultureInfo.InvariantCulture)
            };
            HolidayServices.Insert(holiday);
            return ResponseOk(holiday, "InsertSuccess");
        }

        [HttpDelete("{holidayId}")]
        public IActionResult Remove(string holidayId)
        {
            Holiday holiday = HolidayServices.GetById(holidayId);
            if (holiday is null)
            {
                return ResponseBadRequest("HolidayNotFound");
            }
            HolidayServices.Remove(holiday);
            return ResponseOk("RemoveSuccess");
        }

        [HttpPut("{holidayId}")]
        public IActionResult Update([FromBody]UpdateHolidayRequest updateHoliday, string holidayId)
        {
            Holiday holiday = HolidayServices.GetById(holidayId);
            if (holiday is null)
            {
                return ResponseBadRequest("HolidayNotFound");
            }
            holiday = HolidayServices.Update(holiday, updateHoliday);
            if (holiday is null)
            {
                return ResponseBadRequest("TimeInvalid");
            }
            return ResponseOk(holiday, "UpdateSuccess");
        }
    }
}
