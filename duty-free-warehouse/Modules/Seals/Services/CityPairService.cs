using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.App.Helpers;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Request;
using Project.Modules.Seals.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Project.Modules.Seals.Services
{
    public interface ICityPairService
    {
        (CityPairResponse, string) AddCityPair(AddCityPairRequest request);
        TableResponse ListCityPair(int limit, int page, string search);
        Citypair DetailCityPair(int cityPairId);
        CityPairResponse UpdateCityPair(AddCityPairRequest request, Citypair citypair);
        object ListSealOnDay(DateTime date);
        (object, string) UpdateSealNumber(int cityPairId, List<SealOnDayRequest> requests, int copy);
        List<string> ListDeparture(string departure);
        void GenerateCityPair();
        void UpdateTypeSchedule();
    }
    public class CityPairService : ICityPairService
    {
        public readonly MariaDBContext dBContext;
        public readonly ISealImportService sealImportService;
        public readonly IConfiguration configuration;
        public CityPairService(MariaDBContext DbContext, ISealImportService SealImportService, IConfiguration Configuration)
        {
            dBContext = DbContext;
            sealImportService = SealImportService;
            configuration = Configuration;
        }

        public void UpdateTypeSchedule()
        {
            List<string> airportVn = new List<string> { "SGN", "HAN", "CXR", "VCA", "DAD", "HPH", "PQC", "DLI" };
            List<Citypair> citypairs = dBContext.Citypairs.ToList();
            foreach (var item in citypairs)
            {
                // departure == VN +> xuất
                JArray jArray = JArray.Parse(item.Schedule);
                foreach (var item2 in jArray)
                {
                    item2["route"] = item2["departure"] + "-" + item2["arrival"];
                    if(airportVn.Contains(item2["departure"].ToString()))
                    {
                        item2["typeSchedule"] = 0;
                    }    
                    else
                    {
                        item2["typeSchedule"] = 1;
                    }    
                }

                item.Schedule = JsonConvert.SerializeObject(jArray);
                dBContext.Citypairs.Update(item);
                dBContext.SaveChanges();
            }
        }

        public List<string> ListDeparture(string departure)
        {
            List<string> arrivals = dBContext.Citypairs
                .Select(x => x.Route)
                .Distinct()
                .ToList();
            
            return arrivals;
        }

        private void AddNewCityPair(IGrouping<string, FlightCraw> item, string dayOfWeek)
        {
            List<Schedule> schedules = new List<Schedule>();
            foreach (var item1 in item)
            {
                int typeSchedule = 0;
                if (dBContext.Airports
                    .FirstOrDefault(x => x.AirportCode.Equals(item1.Dep) && !x.CountryCode.Equals("VV")) != null)
                {
                    typeSchedule = 1;
                }

                Schedule schedule = new Schedule
                {
                    departure = item1.Dep,
                    arrival = item1.Arr,
                    route = item1.Dep + "-" + item1.Arr,
                    flightNumber = item1.Flt,
                    status = 1,
                    flightTime = new List<string> { dayOfWeek },
                    typeSchedule = typeSchedule
                };
                schedules.Add(schedule);
            }
            int order = 0;
            if (item.Key.Substring(0, 3).ToLower() == "han")
            {
                order = 1;
            }
            if (item.Key.Substring(0, 3).ToLower() == "sgn")
            {
                order = 2;
            }
            Citypair citypair = new Citypair
            {
                Route = item.Key,
                Schedule = JsonConvert.SerializeObject(schedules),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                DateStartObj = DateTime.Now,
                Status = CityPairStatus.STARTING,
                Order = order
            };
            dBContext.Citypairs.Add(citypair);
            dBContext.SaveChanges();
        }
        void AddCityPairExist(IGrouping<string, FlightCraw> item, string dayOfWeek, Citypair cityPair)
        {
            foreach (var item1 in item)
            {
                JArray jArraySchedule = JArray.Parse(cityPair.Schedule);
                JToken jTest = jArraySchedule
                    .FirstOrDefault(x => x.SelectToken("flightNumber").ToString().Contains(item1.Flt));
                if (jTest == null)
                {
                    int typeSchedule = 0;
                    if (dBContext.Airports
                    .FirstOrDefault(x =>
                        x.AirportCode.Equals(item1.Dep)
                        && !x.CountryCode.Equals("VV")
                    ) != null)
                    {
                        typeSchedule = 1;
                    }

                    Schedule schedule = new Schedule
                    {
                        departure = item1.Dep,
                        arrival = item1.Arr,
                        route = item1.Dep + "-" + item1.Arr,
                        flightNumber = item1.Flt,
                        status = 1,
                        flightTime = new List<string> { dayOfWeek },
                        typeSchedule = typeSchedule
                    };
                    jArraySchedule.Add(JObject.FromObject(schedule));
                }
                else
                {
                    JArray test = JArray.FromObject(jTest["flightTime"]);
                    test.Add(JToken.FromObject(dayOfWeek));
                    jTest["flightTime"] = test;
                }
                cityPair.Schedule = JsonConvert.SerializeObject(jArraySchedule);
            }
            dBContext.Citypairs.Update(cityPair);
            dBContext.SaveChanges();
        }

        private void AddCityPair(string url, long dayMon, Dictionary<string, object> header, List<string> week, int i)
        {
            List<FlightCraw> flightCraws = new List<FlightCraw>();
            string dataRequest = JsonConvert.SerializeObject(new { flight_day = dayMon + (i * 86400000) });
            Console.WriteLine(dataRequest);
            (string data, int? statusCode) = HttpMethod.Post.SendRequestWithStringContent(url, dataRequest, header);
            if (statusCode != (int)HttpStatusCode.OK)
            {
                Console.WriteLine(data);
                return;
            }
            JObject jObject = JObject.Parse(data);
            JArray jData = JArray.FromObject(jObject["data"]);
            foreach (var item in jData)
            {
                if (!item["route_type"].ToString().ToLower().Equals("int"))
                {
                    continue;
                }
                List<string> route = new List<string>()
                    {
                        item["flight_dep"].ToString(),
                        item["flight_arr"].ToString()
                    };
                route.Sort();

                Airport airport = dBContext.Airports
                    .FirstOrDefault(x =>
                        x.AirportCode.Equals(route[0])
                        && !x.CountryCode.Equals("VV")
                    );

                List<string> routeSwap = new List<string>();
                if (airport != null)
                {
                    routeSwap.AddRange(route.Swap(0, 1));
                }
                else
                {
                    routeSwap.AddRange(route);
                }
                FlightCraw flightCraw = new FlightCraw
                {
                    Dep = item["flight_dep"].ToString(),
                    Arr = item["flight_arr"].ToString(),
                    Flt = item["flight_desc"].ToString(),
                    Route = string.Join("-", routeSwap),
                };
                flightCraws.Add(flightCraw);
            }
            IEnumerable<IGrouping<string, FlightCraw>> flightCrawsGroup = flightCraws.GroupBy(x => x.Route);

            foreach (var item in flightCrawsGroup)
            {
                Citypair cityPairCheck = dBContext.Citypairs.Where(x => x.Route.Equals(item.Key)).FirstOrDefault();
                if (cityPairCheck is null)
                {
                    AddNewCityPair(item, week[i]);
                }
                else
                {
                    AddCityPairExist(item, week[i], cityPairCheck);
                }
            }
        }

        public void GenerateCityPair()
        {
            string url = configuration["ExternalSystems:AllFlightDay:Url"];
            long dayMon = 1581935846000;

            Dictionary<string, object> header = new Dictionary<string, object>
            {
                { "Token", "4d9efd250f0079ec27cb358ee908a2dc337cf43f8148241b894796b7d43c29d2014bf4cbc04ebb367feef5a55968323595b546690b7951a1bc4128f00cb9f947" }
            };

            List<string> week = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            for (int i = 0; i < 7; i++)
            {
                AddCityPair(url, dayMon, header, week, i);
            }
        }
        public (CityPairResponse, string) AddCityPair(AddCityPairRequest request)
        {
            CityPairStatus cityPairStatus = request.Status;
            if (request.Status == CityPairStatus.DEFAULT)
            {
                cityPairStatus = CityPairStatus.STOPPED;
                if (request.DateStartObj < DateTime.Now)
                {
                    cityPairStatus = CityPairStatus.STARTING;
                }
            }
            List<Schedule> schedules = new List<Schedule>();
            List<string> flightCheck = new List<string>();
            foreach (var item in request.Schedule)
            {
                if (flightCheck.Contains(item.flightNumber))
                {
                    return (null, "FlightNumber bị trùng lặp");
                }
                else
                {
                    flightCheck.Add(item.flightNumber);
                }
                item.route = item.departure + "-" + item.arrival;
                schedules.Add(item);
            }
            int order = 0;
            if (request.Route.Substring(0, 3).ToLower() == "han")
            {
                order = 1;
            }
            if (request.Route.Substring(0, 3).ToLower() == "sgn")
            {
                order = 2;
            }
            Citypair citypair = new Citypair
            {
                Route = request.Route,
                Schedule = JsonConvert.SerializeObject(schedules),
                DateStartObj = request.DateStartObj.GetValueOrDefault(),
                Status = cityPairStatus,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Order = order
            };
            dBContext.Citypairs.Add(citypair);
            dBContext.SaveChanges();
            return (new CityPairResponse(citypair), "Thêm chuyến bay thành công");
        }

        public TableResponse ListCityPair(int limit, int page, string search)
        {
            List<CityPairResponse> cityPairResponses = dBContext.Citypairs
                .Include(x => x.Seals)
                .ThenInclude(x => x.SealDetails)
                .OrderByDescending(x => x.Order)
                .Select(x => new CityPairResponse(x))
                .ToList();
            if (!string.IsNullOrEmpty(search))
            {
                cityPairResponses = cityPairResponses
                    .Where(x => x.Route.ToLower().Contains(search.ToLower()) ||
                        x.Schedule.FirstOrDefault(v =>
                            v.flightNumber.ToLower().Contains(search.ToLower())) != null)
                    .ToList();
            }
            int countData = cityPairResponses.Count;

            if (limit != 0 && page != 0)
            {
                cityPairResponses = cityPairResponses.Skip(limit * (page - 1))
                    .Take(limit)
                    .ToList();
            }

            TableResponse tableResponse = new TableResponse
            {
                TotalRecord = countData,
                Limit = limit,
                Page = page,
                Data = cityPairResponses
            };

            return tableResponse;
        }

        public Citypair DetailCityPair(int cityPairId)
        {
            Citypair cityPairResponse = dBContext.Citypairs
                .Include(x => x.Seals)
                .ThenInclude(x => x.SealDetails)
                .FirstOrDefault(x => x.Id.Equals(cityPairId));
            return cityPairResponse;
        }

        public CityPairResponse UpdateCityPair(AddCityPairRequest request, Citypair citypair)
        {
            if(request.Status != CityPairStatus.DEFAULT)
            {
                citypair.Status = request.Status;
            }
            if(request.Schedule!= null)
            {
                citypair.Schedule = JsonConvert.SerializeObject(request.Schedule);
            }

            if (request.DateStartObj != null)
            {
                citypair.DateStartObj = request.DateStartObj.GetValueOrDefault();
            }
            if (request.Route != null)
            {
                citypair.Route = request.Route;
            }
            dBContext.Citypairs.Update(citypair);
            dBContext.SaveChanges();
            return new CityPairResponse(citypair);
        }

        public object ListSealOnDay(DateTime date)
        {
            string currentDateOfWeek = DateTime.Now.DayOfWeek.ToString().Substring(0, 3).ToLower();
            List<CityPairResponse> citypairs = dBContext.Citypairs
                .Include(x => x.CopySeal)
                .Include(x => x.Seals)
                .ThenInclude(x => x.SealDetails)
                //.Where(x => x.Seals.Any(v => v.FlightDate.Date.Equals(DateTime.Now.Date)))
                .OrderByDescending(x => x.Order)
                .Where(x => x.Status == CityPairStatus.STARTING)
                .Where(x => x.Schedule.ToLower().Contains(currentDateOfWeek))
                .Select(x => new CityPairResponse(x))
                .ToList();

            return citypairs;
        }

        public (object, string) UpdateSealNumber(int cityPairId, List<SealOnDayRequest> requests, int copy)
        {
            List<SealImportRequest> sealImportRequests = new List<SealImportRequest>();
            List<Seal> sealDelete = dBContext.Seals
                .Where(x => x.CityPairId.Equals(cityPairId) && x.FlightDate.Equals(DateTime.Now.Date))
                .ToList();

            if(sealDelete.Any(x => x.Status != (int)StatusSeals.NEW))
            {
                return (null, $"Điều chuyển của chặng bay {requests.FirstOrDefault().Segment} trong ngày hôm nay đã xuất kho hoặc chốt seal");
            }
                
            List<string> sealList = sealDelete.Select(x => x.SealNumber).ToList();
            List<SealProduct> sealDetailDelete = dBContext.SealProducts.Where(x => sealList.Contains(x.SealNumber)).ToList();
            dBContext.Seals.RemoveRange(sealDelete);
            dBContext.SealProducts.RemoveRange(sealDetailDelete);
            dBContext.SaveChanges();
            bool checkSealOnDay = false;
            foreach (var item in requests)
            {
                if(string.IsNullOrEmpty(item.SealNumber))
                {
                    continue;
                }
                checkSealOnDay = true;
                // check seal exists
                Seal sealCheck = dBContext.Seals
                    .FirstOrDefault(x => 
                        x.Route.Equals(item.Segment) && 
                        x.FlightDate.Equals(DateTime.Now.Date) && 
                        x.FlightNumber.Equals(item.FlightNumber));
                if(sealCheck != null)
                {
                    continue;
                }
                    
                sealImportRequests.Add(
                    new SealImportRequest
                    {
                        FlightDate = DateTime.Now.Date,
                        SealNumber = item.SealNumber,
                        FlightNumber = item.FlightNumber,
                        CityPairId = cityPairId,
                        SealNumberReturn = item.SealNumberReturn,
                        Route = item.Segment,
                        AcReg = item.AcReg
                    });
            }

            if (!checkSealOnDay)
            {
                return (null, "Số Seal không được để trống");
            }
                
            (List<SealImportRequest> data, string message) = sealImportService.Import(sealImportRequests);
            if(data is null)
            {
                return (null, message);
            }

            CopySeal copySealUpdate = dBContext.CopySeals.FirstOrDefault(x => x.CitypairId == cityPairId);
            if (copy != 0)
            {
                List<SealResponse> sealResponses = data.Select(x => new SealResponse(x)).ToList();
                sealResponses.ForEach(x => { x.SealNumber = ""; x.Return = ""; });
                if (copySealUpdate != null)
                {
                    copySealUpdate.DataCopy = JsonConvert.SerializeObject(sealResponses);
                    dBContext.CopySeals.Update(copySealUpdate);
                    dBContext.SaveChanges();
                }
                else
                {
                    CopySeal copySeal = new CopySeal
                    {
                        CitypairId = cityPairId,
                        DataCopy = JsonConvert.SerializeObject(sealResponses)
                    };
                    dBContext.CopySeals.Add(copySeal);
                    dBContext.SaveChanges();
                }
            }
            
            return (data, "Cập nhật SealNumber thành công");
        }
    }

    public class FlightCraw
    {
        public string Dep { get; set; }
        public string Arr { get; set; }
        public string Flt { get; set; }
        public string Route { get; set; }
        public string Date { get; set; }
    }
}
