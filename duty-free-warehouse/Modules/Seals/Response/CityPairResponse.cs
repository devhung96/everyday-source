using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Request;
using Project.Modules.Sells.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Seals.Response
{
    public class CityPairResponse
    {
        public int Id { get; set; }
        public string Route { get; set; }
        public bool Copy { get; set; } = false;
        public object FlightTime { get; set; }
        public int Status { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateSeal { get; set; } = DateTime.Now;
        public List<ScheduleResponse> Schedule { get; set; } 
        public List<SealResponse> Seals { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int NumberCarTransfer { get; set; }
        public int NumberCarExport { get; set; }
        public int NumberCarReturn { get; set; }
        public int QuantityExport { get; set; }
        public int QuantitySell { get; set; }

        public CityPairResponse(Citypair citypair)
        {
            Id = citypair.Id;
            Route = citypair.Route;
            Status = (int)citypair.Status;
            DateStart = citypair.DateStartObj;
            Schedule = JsonConvert.DeserializeObject<List<ScheduleResponse>>(citypair.Schedule);
            CreatedAt = citypair.CreatedAt;
            UpdatedAt = citypair.UpdatedAt;

            citypair.Seals = citypair.Seals.Where(x => x.FlightDate.Date == DateTime.Now.Date).ToList();

            Copy = citypair.CopySeal != null;

            if (citypair.CopySeal != null && citypair.Seals.Count == 0)
            {
                List<SealResponse> dataCopy = JsonConvert.DeserializeObject<List<SealResponse>>(citypair.CopySeal.DataCopy);
                dataCopy = dataCopy.Where(x => !citypair.Schedule.Contains(x.FlightNumber)).ToList();
                Seals = dataCopy;
            }
            else 
            {
                Seals = citypair.Seals
                    .Select(x => new SealResponse(x))
                    .ToList();
            }
            if (Schedule.Count > 0)
            {
                FlightTime = Schedule[0].flightTime;
            }

            foreach (var item in Schedule)
            {
                item.route = item.departure + "-" + item.arrival;
            }

            NumberCarTransfer = Seals.Count;
            NumberCarExport = Seals.Count(x => x.Status == 1);
            NumberCarReturn = Seals.Count(x => x.Status == 2);
            if(citypair.Seals.Count != 0)
            {
                QuantityExport = citypair.Seals.Sum(x => x.SealDetails.Sum(v => v.QuantityExport));
                QuantitySell = citypair.Seals.Sum(x => x.SealDetails.Sum(v => v.QuantitySell));
            }
        }
    }

    public class ScheduleResponse : Schedule
    {
        public string AcReg { get; set; } = null; 
    }
}
