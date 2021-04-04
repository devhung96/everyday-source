using Newtonsoft.Json;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Request;
using Project.Modules.Sells.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Seals.Response
{
    public class SealDetailResponse
    {
        public string SealNumber { get; set; }
        public string Route { get; set; }

        public string FlightNumber { get; set; }
        public DateTime FlightDate { get; set; }
        public string AcReg { get; set; }
        public int Status { get; set; }
        public DateTime? ExportDate { get; set; }
        public string Return { get; set; }
        public DateTime? ImportDate { get; set; }
        public object SealDetail { get; set; }
        public List<FlightNumberArr> FlightNumberArr { get; set; }

        public SealDetailResponse(Seal seal, List<Citypair> citypair = null)
        {
            SealNumber = seal.SealNumber;
            Route = seal.Route;
            FlightNumber = seal.FlightNumber;
            FlightDate = seal.FlightDate;
            AcReg = seal.AcReg;
            Status = seal.Status;
            ExportDate = seal.ExportDate;
            Return = seal.Return;
            ImportDate = seal.ImportDate;

            FlightNumberArr = FlightNumber
                .Replace(" ", "")
                .Split(",")
                .Select(x => 
                    new FlightNumberArr(x, null, citypair.FirstOrDefault(v => v.Schedule.Contains(x)))
                ).ToList();

            if(seal.SealDetails.Count != 0)
            {
                SealDetail = seal.SealDetails.GroupBy(x => x.ProductCode).Select(x => new
                {
                    productCode =  x.Key,
                    sealNumber = x.FirstOrDefault().SealNumber,
                    deClaNumber = x.FirstOrDefault().DeClaNumber,
                    quantitySell = x.Sum(y => y.QuantitySell),
                    quantityExport = x.Sum(y => y.QuantityExport),
                    quantityInventory = x.Sum(y => y.QuantityInventory) ,
                    quantityReal = x.Sum(y => y.QuantityReal),
                    product = x.FirstOrDefault().Product
                }).ToList();
            } 
            else
            {
                SealDetail = seal.SealProduct.Select(x => new
                {
                    productCode = x.ProductCode,
                    sealNumber = x.SealNumber,
                    deClaNumber = "",
                    quantitySell = 0,
                    quantityExport = x.QuantityExport,
                    quantityInventory = 0,
                    quantityReal = 0,
                    product = x.product
                }).ToList();
            }
        }
    }

    public class FlightNumberArr
    {
        public string FlightNumber { get; set; }
        public int? TypeInvoice { get; set; } = null;

        public FlightNumberArr(string flightNumber, int? typeInvoice, Citypair citypair)
        {
            FlightNumber = flightNumber;
            if(citypair != null)
            {
                List<Schedule> schedules = JsonConvert.DeserializeObject<List<Schedule>>(citypair.Schedule);
                TypeInvoice = schedules.FirstOrDefault(x => x.flightNumber.Equals(flightNumber))?.typeSchedule;
            }    
        }
    }
}
