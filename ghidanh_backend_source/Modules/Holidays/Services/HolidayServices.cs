using Microsoft.Extensions.Configuration;
using Project.App.DesignPatterns.Repositories;
using Project.Modules.Holidays.Entities;
using Project.Modules.Holidays.Requests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Holidays.Services
{
    public interface IHolidayServices
    {
        Holiday Insert(Holiday holiday);
        List<Holiday> GetAll();
        Holiday GetById(string holidayId);
        void Remove(Holiday holiday);
        Holiday Update(Holiday holiday, UpdateHolidayRequest updateHoliday);
    }
    public class HolidayServices : IHolidayServices
    {
        public readonly IConfiguration Configuration;
        public readonly IRepositoryMariaWrapper RepositoryMaria;
        public HolidayServices(IConfiguration configuration, IRepositoryMariaWrapper repositoryMaria)
        {
            Configuration = configuration;
            RepositoryMaria = repositoryMaria;
        }

        public Holiday Insert(Holiday holiday)
        {
            RepositoryMaria.Holidays.Add(holiday);
            RepositoryMaria.SaveChanges();
            return holiday;
        }

        public List<Holiday> GetAll()
        {
            return RepositoryMaria.Holidays.FindAll().ToList();
        }

        public Holiday GetById(string holidayId)
        {
            return RepositoryMaria.Holidays.GetById(holidayId);
        }

        public void Remove(Holiday holiday)
        {
            RepositoryMaria.Holidays.RemoveMaria(holiday);
            RepositoryMaria.SaveChanges();
        }

        public Holiday Update(Holiday holiday, UpdateHolidayRequest updateHolidayRequest)
        {
            Holiday holidayUpdate = new Holiday
            {
                HolidayId = holiday.HolidayId,
                Name = updateHolidayRequest.HolidayName,
                Reason = updateHolidayRequest.Reason,
                TimeStart = DateTime.ParseExact(updateHolidayRequest.TimeStart, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                TimeEnd = DateTime.ParseExact(updateHolidayRequest.TimeEnd, "dd-MM-yyyy", CultureInfo.InvariantCulture),
            };
            holiday = holidayUpdate.MergeData(holiday);
            RepositoryMaria.Holidays.UpdateMaria(holiday);
            RepositoryMaria.SaveChanges();
            return holiday;
        }
    }
}
